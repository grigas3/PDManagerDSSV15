using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace PDManager.Common.Interfaces
{

    /// <summary>
    /// Recurring Job
    /// </summary>
    public interface IRecurringJob
    {

      
        /// <summary>
        /// Run Job
        /// </summary>
        /// <returns>True if no error occured otherwise false</returns>
        Task<bool> Run();
        /// <summary>
        /// Get Job Id
        /// </summary>
        /// <returns>Id as string</returns>
        string GetId();
    }
}
