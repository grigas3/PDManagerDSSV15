using Newtonsoft.Json;
using PDManager.Common.Models;
using System.Collections.Generic;

namespace PDManager.Common.Extensions
{
    /// <summary>
    /// Common Model Extensions
    /// </summary>
    public static class CommonModelExtensions
    {

        #region Helpers
        /// <summary>
        /// Get Clinical Information List. This is an extension of the PDPatient class.
        /// The basic info are the Code and the Value
        /// </summary>
        /// <param name="patient">PDPatient</param>
        /// <returns>List of Clinical info <see cref="ClinicalInfo"/> </returns>
        public static IEnumerable<ClinicalInfo> GetClinicalInfoList(this PDPatient patient)
        {
            if (string.IsNullOrEmpty(patient.ClinicalInfo))
            {
                //If CLinical info string is null or emtpy return empty lsit

                return new List<ClinicalInfo>();
            }
            else
            {
                try
                {
                    return JsonConvert.DeserializeObject<IEnumerable<ClinicalInfo>>(patient.ClinicalInfo);
                }
                catch
                {
                    return new List<ClinicalInfo>();
                }
            }
        }
        #endregion
    }
}
