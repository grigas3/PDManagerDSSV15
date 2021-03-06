﻿using PDManager.Common.Interfaces;
using System.Collections.Generic;
using System.Linq;
using PDManager.Common.Enums;
using System.Threading.Tasks;
using PDManager.Common.Models;

namespace PDManager.DSS
{



    /// <summary>
    /// Alert Evaluation
    /// </summary>
    public class AlertEvaluationJob: IRecurringJob
    {

        #region Private Fields

        private readonly IAlertEvaluator _alertEvaluator;
        private readonly IAlertInputProvider _alertInputProvider;
        private readonly INotificationService _communicationManager;
        //Patient Provider
        private readonly IPatientProvider _patientProvider;
        //Logger
        private readonly IGenericLogger _logger;
        private const int MAXPATIENTS = 100;
        #endregion


        /// <summary>
        /// Alert Evaluation Job
        /// </summary>
        /// <param name="alertEvaluator">Alert Evaluator</param>
        /// <param name="alertInputProvider">Alert Input Provider</param>
        /// <param name="patientProvider">Patient Id Provider</param>
        /// <param name="commManager">Communication Manager</param>
        /// <param name="logger">Logger</param>
        public AlertEvaluationJob(IAlertEvaluator alertEvaluator, IAlertInputProvider alertInputProvider,IPatientProvider patientProvider,INotificationService commManager,IGenericLogger logger)
        {
            this._alertEvaluator = alertEvaluator;
            this._patientProvider = patientProvider;
            this._logger = logger;
            this._alertInputProvider = alertInputProvider;
            this._communicationManager = commManager;
        }

        /// <summary>
        /// Run Job
        /// </summary>
        /// <returns>True if job succeeds, otherwise false</returns>
        public async Task<bool> Run()
        {
            int take = MAXPATIENTS;
            int currentNumberOfPatients = 0;
            int n = 0;

            var alertInputs = _alertInputProvider.GetAlertInputs();
            do
            {


                var patientList = _patientProvider.GetPatientIds(MAXPATIENTS, currentNumberOfPatients);
                n = patientList.Count();              

                // Alert Patient List
                foreach(var patId in patientList)
                {

                    //Iterate Alert Input Models
                    foreach (var alertInput in alertInputs)
                    {

                        var alertLevel=await _alertEvaluator.GetAlertLevel(alertInput, patId);

                        if (alertLevel != AlertLevel.High)
                            continue;

                          IEnumerable<NotificationContact> contacts=  _patientProvider.GetPatientContacts(patId);
                        foreach (var contact in contacts)
                        {
                            _communicationManager.SendMessage(new PDMessage()
                            {

                                Sender = "PDManager",
                                Subject = alertInput.Name,
                                Body = alertInput.Message,
                                ReceiverUri = contact.Uri,
                                MessageType=contact.PreferredMessageType,
                                Receiver=contact.Name


                            });

                        }

                        
                    }
                }

            } while (n == take);

            return true;
        }

        /// <summary>
        /// Get Job Id
        /// </summary>
        /// <returns>Job Id</returns>
        public string GetId()
        {
            return "A8230448-3BAB-4B49-94D2-733975DD43DD";
    }
    }
}
