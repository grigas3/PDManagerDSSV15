﻿using PDManager.Common.Enums;
using PDManager.Common.Extensions;
using PDManager.Common.Interfaces;
using PDManager.Common.Models;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
namespace PDManager.DSS
{

    /// <summary>
    /// Alert Evaluator
    /// </summary>
    public class AlertEvaluator : IAlertEvaluator
    {

        private readonly IDSSRunner _dssRunner;
        private readonly IDataProxy _dataProxy;
        private readonly IAggregator _aggregator;
        private readonly IAggrDefinitionProvider _aggrDefinitionProvider;
        private readonly IDSSDefinitionProvider _dssDefinitionProvider;
        /// <summary>
        /// Alert Evaluator
        /// </summary>
        /// <param name="dssRunner">DSSRunner</param>
        /// <param name="aggregator">Aggregator</param>
        /// <param name="dataProxy">Data proxy</param>
        /// <param name="aggrDefinitionProvider">Aggregation Definition Provider</param>
        /// <param name="dSSDefinitionProvider">DSS definition Provider</param>
        public AlertEvaluator(IDSSRunner dssRunner, IAggregator aggregator,IDataProxy dataProxy, IAggrDefinitionProvider aggrDefinitionProvider, IDSSDefinitionProvider dSSDefinitionProvider)
        {
            this._dssRunner = dssRunner;
            this._aggregator = aggregator;
            this._dataProxy = dataProxy;
            this._aggrDefinitionProvider = aggrDefinitionProvider;
            this._dssDefinitionProvider = dSSDefinitionProvider;

        }

        /// <summary>
        /// Apply Filter for numeric values
        /// </summary>
        /// <param name="alert">Alert Input</param>
        /// <param name="value">Numeric Value</param>
        /// <returns>AlertLevel</returns>
        private AlertLevel ApplyFilter(IAlertInput alert, double value)
        {

            return (alert.HighPriorityValue != null && double.Parse(alert.HighPriorityValue)>value)? AlertLevel.High :
               (alert.MediumPriorityValue != null && double.Parse(alert.MediumPriorityValue) > value) ? AlertLevel.Medium :
             (alert.LowPriorityValue != null && double.Parse(alert.LowPriorityValue) > value)? AlertLevel.Low : AlertLevel.None;


        }


        /// <summary>
        /// Apply Filter for numeric values
        /// </summary>
        /// <param name="alert">Alert input</param>
        /// <param name="value">Categorical Value</param>
        /// <returns>AlertLevel</returns>
        private AlertLevel ApplyFilter(IAlertInput alert, string value)
        {

            return alert.HighPriorityValue != null && alert.HighPriorityValue.Split(';').ToList().Contains(value) ? AlertLevel.High :
                alert.MediumPriorityValue != null&& alert.MediumPriorityValue.Split(';').ToList().Contains(value) ? AlertLevel.Medium :
              alert.LowPriorityValue != null && alert.LowPriorityValue.Split(';').ToList().Contains(value) ? AlertLevel.Low : AlertLevel.None;
                        
          }



        private const string ObservationType = "observation";
        private const string MetaObservationType = "metaobservation";
        private const string ClinicalInfoType = "clinical";
        private const string DSSInfoType = "dss";

        /// <summary>
        /// Get Alert Level
        /// </summary>
        /// <param name="alert">Alert Input</param>
        /// <param name="patientId">Patient Id</param>
        /// <returns>0: No Alert, 1: Low Priority Alert, 2: Medium Priority Alert, 3: High Priority Alert</returns>
        public async Task<AlertLevel> GetAlertLevel(IAlertInput alert,string patientId)
        {
            
            if (alert.TargetValueSource.ToLower() == ObservationType)
            {
                if (!alert.TargetValueNumeric)
                    throw new InvalidCastException("Target value should be numeric");
            
                try
                {
                    var observations = await _dataProxy.Get<PDObservation>(10, 0, String.Format("{{patientid:\"{0}\",datefrom:\"{2}\",dateto:\"{3}\",codeid:\"{1}\",aggr:\"total\"}}", patientId, alert.TargetValueSource, (DateTime.Now.AddDays(-alert.AggregationPeriodDays).ToUnixTimestampMilli()), (DateTime.Now.ToUnixTimestampMilli())), null);
                    var value = observations.Select(e => e.Value).DefaultIfEmpty(0).Average();
                    return ApplyFilter(alert, value);
                    
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                    throw ex;
                }

            }
            else if (alert.TargetValueSource.ToLower() == MetaObservationType)
            {
                //Get Aggregated observation
                var aggrObservation = await _aggregator.Run(patientId, alert.TargetValueCode, DateTime.Now.AddDays(-alert.AggregationPeriodDays));                
                var value=aggrObservation.Select(e => e.Value).Average();
                return ApplyFilter(alert, value);

            }
            
            else if (alert.TargetValueSource.ToLower() == ClinicalInfoType)
            {
                //Clinical Info Input
                try
                {
                    var patient = await _dataProxy.Get<PDPatient>(patientId);

                    var clinicalInfoList =patient.GetClinicalInfoList();
                    var clinicalInfo = clinicalInfoList.FirstOrDefault(e => e.Code.ToLower() == alert.TargetValueCode.ToLower());

                    return ApplyFilter(alert, clinicalInfo.Value);
                    
                }
                catch (Exception ex)
                {
                    throw ex;
                }

            }           
            else if (alert.TargetValueSource.ToLower() == DSSInfoType)
            { 
                //DSS Input
                try
                {
                    var observations = await _dssRunner.Run(patientId,_dssDefinitionProvider.GetJsonConfigFromCode(alert.TargetValueCode));                   
                    var value = observations.Where(e => e.Code == alert.TargetValueCode).Select(e => e.Value).FirstOrDefault();
                    return ApplyFilter(alert, value);

                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                    throw ex;
                }
            }
            else
            {

                throw new NotSupportedException($"Source type  not supported from PDManager Alert Evaluator");

            }

          


        }
    }
}
