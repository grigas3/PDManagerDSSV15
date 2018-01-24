using System;
using System.Collections.Generic;
using System.Text;

namespace PDManager.Common.Interfaces
{
    /// <summary>
    /// DSS Definition Provider
    /// </summary>
    public interface  IDSSDefinitionProvider
    {

        /// <summary>
        /// Get Aggregation Confing in Json format for specific observation code
        /// </summary>
        /// <param name="code">Meta observation code</param>
        /// <returns>Path of definiton file</returns>
        string GetJsonConfigFromCode(string code);
    }
}
