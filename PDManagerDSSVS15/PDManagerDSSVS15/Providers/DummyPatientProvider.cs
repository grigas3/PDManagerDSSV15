using PDManager.Common.Interfaces;
using PDManager.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PDManagerDSSVS15.Providers
{
    /// <summary>
    /// Dummy Implementation of IPatientProvider
    /// TODO: Replace with an implementation retrieving data from the repository
    /// </summary>
    public class DummyPatientProvider : IPatientProvider
    {
        /// <summary>
        ///  Get Patient Contacts
        /// </summary>
        /// <param name="patId"></param>
        /// <returns></returns>
        public IEnumerable<NotificationContact> GetPatientContacts(string patId)
        {//TODO: Add some test credentials
            return new List<NotificationContact>(){new NotificationContact()
            {


            }};
        }
        /// <summary>
        /// Get Patient Ids
        /// </summary>
        /// <param name="take"></param>
        /// <param name="skip"></param>
        /// <returns></returns>

        public IEnumerable<string> GetPatientIds(int take = 0, int skip = 0)
        {
            return new List<string>() { "5900aa2a2f2cd563c4ae3027" };
        }
    }
}
