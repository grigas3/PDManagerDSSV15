using System;
using System.Collections.Generic;
using System.Text;
using PDManager.Common.Models;

namespace PDManager.Common.Interfaces
{

    /// <summary>
    /// Patient Provider
    /// </summary>
    public interface IPatientProvider
    {

        /// <summary>
        /// Get Patient Ids
        /// The method supports take and skip in order to take and handle patients from a large repository
        /// </summary>
        /// <param name="take">Take from list</param>
        /// <param name="skip">Skip from list</param>
        /// <returns>List of Patient Ids as string</returns>
        IEnumerable<string> GetPatientIds(int take = 0, int skip = 0);
        /// <summary>
        /// Get Patient Contacts
        /// </summary>
        /// <param name="patId">Patient Id</param>
        /// <returns>List of Notification Contact <see cref="NotificationContact"/> </returns>
        IEnumerable<NotificationContact> GetPatientContacts(string patId);
    }
}
