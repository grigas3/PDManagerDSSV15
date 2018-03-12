using System;
using System.Collections.Generic;
using System.Text;

namespace PDManager.Common.Interfaces
{
    /// <summary>
    /// Alert Input Provider
    /// </summary>
    public interface IAlertInputProvider
    {
        /// <summary>
        /// Get Alert Inputs
        /// </summary>
        /// <returns>List of Alert Inputs <see cref="IAlertInput"/> </returns>
        IEnumerable<IAlertInput> GetAlertInputs();
    }
}
