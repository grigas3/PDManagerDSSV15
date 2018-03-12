using Newtonsoft.Json;
using PDManager.Common.Extensions;
using PDManager.Common.Interfaces;
using PDManager.Common.Models;
using PDManager.DSS.Dexi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace PDManager.DSS
{
    /// <summary>
    /// DSS Runner
    /// </summary>
    public class DSSRunner : IDSSRunner
    {
        private readonly IDataProxy _dataProxy;
        private readonly IAggregator _aggregator;
        

        private const string ObservationType = "observation";
        private const string MetaObservationType = "metaobservation";
        private const string ClinicalInfoType = "clinical";
        private const string DemographicsType = "demographics";
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="aggregator">Aggregator</param>
        /// <param name="dataProxy">Data proxy</param>
        public DSSRunner(IAggregator aggregator,IDataProxy dataProxy)
        {
            this._dataProxy = dataProxy;
            this._aggregator = aggregator;
        }

        #region Private Declaration

        //private Dictionary<string, int> _valueMapping = new Dictionary<string, int>();
        //private Model _dssModel;
        //private DSSConfig _config;

        #endregion Private Declaration


        /// <summary>
        /// Load Model from a file
        /// TODO: We could cache models
        /// </summary>
        /// <param name="modelFileName"></param>
        /// <returns></returns>
        private Model LoadModel(string modelFileName)
        {
            //TODO: Test that this works
            var filePath=Path.Combine(AppDomain.CurrentDomain.BaseDirectory, modelFileName);

            return new Model(filePath);
        }

        /// <summary>
        /// Get Clinical Information List
        /// The basic info are the Code and the Value
        /// </summary>
        /// <param name="clinicalInfo">Clinical info String from Patient</param>
        /// <returns>List of Clinical info <see cref="ClinicalInfo"/></returns>
        public IEnumerable<ClinicalInfo> GetClinicalInfoList(string clinicalInfo)
        {
            if (string.IsNullOrEmpty(clinicalInfo))
            {
                //If CLinical info string is null or emtpy return empty lsit

                return new List<ClinicalInfo>();
            }
            else
            {
                try
                {
                    return JsonConvert.DeserializeObject<IEnumerable<ClinicalInfo>>(clinicalInfo);
                }
                catch
                {
                    return new List<ClinicalInfo>();
                }
            }
        }

        /// <summary>
        /// Get Demographics Info List
        /// </summary>
        /// <param name="patient"></param>
        /// <returns>List of ClinicalInfo <see cref="ClinicalInfo"/> </returns>
        public IEnumerable<ClinicalInfo> GetDemographicsInfoList(PDPatient  patient)
        {

            List<ClinicalInfo> list = new List<ClinicalInfo>();
            if (patient.BirthDate.HasValue)
                list.Add(new ClinicalInfo() {Name="Age", Code = "age", Value = ((int)((DateTime.Now - patient.BirthDate.Value).TotalDays / 365)).ToString() });
            if (patient.PDAppearance.HasValue)
                list.Add(new ClinicalInfo() { Name = "Years with PD", Code = "pdy", Value = ((int)((DateTime.Now - patient.PDAppearance.Value).TotalDays / 365)).ToString() });
            if(!string.IsNullOrEmpty(patient.Gender))
                list.Add(new ClinicalInfo() { Name = "Gender", Code = "gender", Value = patient.Gender});

            return list;
        }

        /// <summary>
        /// Run using specific values
        /// </summary>        
        /// <param name="configJson">Dss config in json format</param>
        /// <param name="values">Value Dictionary</param>
        /// <returns>List of DSS Values <see cref="DSSValue"/> </returns>
        public IEnumerable<DSSValue> Run(string configJson, Dictionary<string,string> values)       
        { 
            //Dictionary<string, int> valueMapping = new Dictionary<string, int>();
            var config = JsonConvert.DeserializeObject<DSSConfig>(configJson);

            //TODO: Handle Exceptions
            var model = LoadModel(config.DexiFile);
            return Evaluate(model, config, values);

        }


        /// <summary>
        /// Evaluate
        /// </summary>
        /// <param name="model">Dexi Model</param>
        /// <param name="config">DSS configuration model</param>
        /// <param name="values">Dictionary values</param>
        /// <returns>List of DSS Values <see cref="DSSValue"/> </returns>
        private IEnumerable<DSSValue> Evaluate(Model model,DSSConfig config, Dictionary<string, string> values)
        {

            foreach (var parameterInfo in config.Input)
            {
                var key = parameterInfo.Name;
                try
                {
                    if (values.ContainsKey(key) && !string.IsNullOrEmpty(values[key]))
                    {


                        if (parameterInfo.Numeric)
                        {

                            double v = double.Parse(values[key]);
                            model.SetInputValue(parameterInfo.Name, parameterInfo.GetNumericMapping(v));
                        }
                        else
                        {
                            int? v = parameterInfo.GetCategoryMapping(values[key]);
                            if (v.HasValue)
                                model.SetInputValue(parameterInfo.Name, v.Value);
                            else
                                model.SetInputValue(parameterInfo.Name, parameterInfo.DefaultValue);

                        }



                    }
                    else
                    {

                        model.SetInputValue(parameterInfo.Name, parameterInfo.DefaultValue);
                    }

                }
                catch(Exception ex)
                {

                    Debug.WriteLine($"Error setting param {parameterInfo.Name} with ex {ex.Message}");
                }

            }

            model.Evaluate(Model.Evaluation.SET, true);
            var ret = model.Aggregate.Select(e => new DSSValue() { Name = e.Name, Code=e.Name,Value = e.ValuesString });
            return ret;
        }
        /// <summary>
        /// Get Input Values for DSS model of specific patient
        /// </summary>
        /// <param name="patientId">Patient Id</param>
        /// <param name="configJson">DSS Mapping file</param>
        /// <returns>List of DSS Values <see cref="DSSValue"/> </returns>
        public async Task<IEnumerable<DSSValue>> GetInputValues(string patientId, string configJson)
        {

           
            Dictionary<string, int> valueMapping = new Dictionary<string, int>();
            var config = JsonConvert.DeserializeObject<DSSConfig>(configJson);
            var ret = await GetInputValues(patientId, config);
            return ret;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="patientId">Patient Id</param>
        /// <param name="config">DSS Configuration file</param>
        /// <returns>List of DSS Values <see cref="DSSValue"/> </returns>
        private async Task<IEnumerable<DSSValue>> GetInputValues(string patientId, DSSConfig config)
        {
            List<DSSValue> values = new List<DSSValue>();
            //Codes from observations
            List<string> observationCodes = new List<string>();

            //Codes from Meta o
            List<string> metaObservationCodes = new List<string>();

            //Codes from Patient Clinical Info
            List<string> clinicalInfoCodes = new List<string>();


            //Codes from Patient Clinical Info
            List<string> demographicCodes = new List<string>();


            foreach (var c in config.Input)
            {


                if (string.IsNullOrEmpty(c.Source))
                {
                    //TODO: Add some error to response
                    continue;
                }


                if (c.Source.ToLower() == ObservationType)
                {
                    if(!observationCodes.Contains(c.Code))
                    observationCodes.Add(c.Code);
                }
                else if (c.Source.ToLower() == MetaObservationType)
                {
                    if (!metaObservationCodes.Contains(c.Code))
                        metaObservationCodes.Add(c.Code);

                }
                else if (c.Source.ToLower() == ClinicalInfoType)
                {
                    if (!clinicalInfoCodes.Contains(c.Code))
                        clinicalInfoCodes.Add(c.Code);
                }
                else if (c.Source.ToLower() == DemographicsType)
                {
                    if (!demographicCodes.Contains(c.Code))
                        demographicCodes.Add(c.Code);
                }
                else
                {

                    throw new NotSupportedException($"Source type {c.Source} not supported from PDManager DSS");

                }
            }

            #region Observation Codes


            //TODO: Join observations codes in a single request in case 

            foreach (var code in observationCodes)
            {
                try
                {
                    var observations = await _dataProxy.Get<PDObservation>(10, 0, String.Format("{{patientid:\"{0}\",datefrom:\"{2}\",dateto:\"{3}\",codeid:\"{1}\",aggr:\"total\"}}", patientId, code, (DateTime.Now.AddDays(-config.AggregationPeriodDays).ToUnixTimestampMilli()), (DateTime.Now.ToUnixTimestampMilli())), null);

                  
                        foreach (var observation in config.Input.Where(e => e.Code == code))
                        {
                            values.Add(new DSSValue() { Name = observation.Name, Code = observation.Code, Value = observations.Select(e => e.Value).DefaultIfEmpty(0).Average().ToString() });

                        }

                   // }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                }
            }

            #endregion Meta Observation Codes

            #region Meta Observation Codes


            foreach (var code in metaObservationCodes)
            {
                try
                {
                    //Get Aggregated observation
                    var aggrObservation = await _aggregator.Run(patientId, code, DateTime.Now.AddDays(-config.AggregationPeriodDays));

                    //Find Corresponding observation in config
                    

                    foreach(var metaObservation in config.Input.Where(e => e.Code == code))
                    {
                        //Meta Observations are only numeric
                        values.Add(new DSSValue() { Name = metaObservation.Name, Code = metaObservation.Code, Value = aggrObservation.Select(e => e.Value).DefaultIfEmpty(0).Average().ToString() });


                    }

                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

            #endregion Observation Codes


            #region Clinical Info

            try
            {
                var patient = await _dataProxy.Get<PDPatient>(patientId);

                var clinicalInfoList = GetClinicalInfoList(patient.ClinicalInfo);
                foreach (var c in clinicalInfoList)
                {
                    var clinicalInfo = config.Input.FirstOrDefault(e => e.Code.ToLower() == c.Code.ToLower());

                    if (clinicalInfo != null)
                    {

                        values.Add(new DSSValue() { Name = clinicalInfo.Name, Code = clinicalInfo.Code, Value = c.Value });

                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            #endregion Clinical Info
            #region Demographics Info

            try
            {
                var patient = await _dataProxy.Get<PDPatient>(patientId);

                var demographicInfoList = GetDemographicsInfoList(patient);
                foreach (var c in demographicInfoList)
                {
                    var clinicalInfo = config.Input.FirstOrDefault(e => e.Code.ToLower() == c.Code.ToLower());

                    if (clinicalInfo != null)
                    {

                        values.Add(new DSSValue() { Name = clinicalInfo.Name, Code = clinicalInfo.Code, Value = c.Value });

                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            #endregion Clinical Info


            return values;

        }

        /// <summary>
        /// Run DSS
        /// </summary>
        /// <param name="patientId">Patient Id</param>
        /// <param name="configJson">DSS Config as Json String file</param>
        /// <returns>List of DSS Values <see cref="DSSValue"/> </returns>
        public async Task<IEnumerable<DSSValue>> Run(string patientId, string configJson)
        {
            try
            {
                Dictionary<string, int> valueMapping = new Dictionary<string, int>();
                var config = DSSConfig.FromString(configJson);

                //TODO: Handle Exceptions
                var model = LoadModel(config.DexiFile);

                // Set initial values
                foreach (var c in config.Input)
                {
                  
                    model.SetInputValue(c.Name, c.DefaultValue);
                }

                //Get DSS input values
                var values = await GetInputValues(patientId, configJson);

                //Assing new values
                Dictionary<string, string> dict = new Dictionary<string, string>();
                foreach (var c in values)
                {
                    if (!dict.ContainsKey(c.Name))
                        dict.Add(c.Name, c.Value);

                }
                return Evaluate(model, config, dict);
            }
            catch(Exception ex)
            {

                //TODO: Log

                throw; 

            }
        }

      
    }
}