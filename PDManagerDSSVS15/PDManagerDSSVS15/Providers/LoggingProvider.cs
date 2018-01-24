using PDManager.Common.Interfaces;
using System;
using System.Diagnostics;
namespace PDManagerDSSVS15.Providers
{

    /// <summary>
    /// Implentation of IGenericLogger based on ILogger of Microsoft.Extensions.Logging
    /// </summary>
    public class LoggingProvider:IGenericLogger
    {

   
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="logger">Microsoft.Extensions.Logging Logger</param>
        public LoggingProvider()
        {


        }

        /// <summary>
        /// Log Error
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="message"></param>
        public void LogError(Exception ex, string message)
        {
            Trace.WriteLine($"{DateTime.Now.ToString()}: {message} Exception: {ex.ToString()}");
        }
    }
}
