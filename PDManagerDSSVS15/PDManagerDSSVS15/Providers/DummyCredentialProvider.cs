using PDManager.Common.Interfaces;
using System.Configuration;
using System.Text;

namespace PDManagerDSSVS15
{
    /// <summary>
    /// Dymmy Credential Provider
    /// In Case Data Proxy is used where data are collected from a web api with authentication
    /// then credentials are provided by IProxyCredientialsProvider
    /// </summary>
    public class DummyCredentialProvider : IProxyCredientialsProvider
    {

      
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="configuration"></param>
        public DummyCredentialProvider()
        {
         


        }
        /// <summary>
        /// password
        /// </summary>
        /// <returns></returns>
        public string GetPassword()
        {
            return ConfigurationManager.AppSettings["PDPassword"];
        }

        /// <summary>
        /// User name
        /// </summary>
        /// <returns></returns>
        public string GetUserName()
        {
            return ConfigurationManager.AppSettings["PDUserName"];
        }
    }
    
}
