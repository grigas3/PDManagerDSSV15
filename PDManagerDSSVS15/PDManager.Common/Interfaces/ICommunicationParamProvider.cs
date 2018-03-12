using PDManager.Common.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace PDManager.Common.Interfaces
{
    /// <summary>
    /// Communication Param Provider
    /// </summary>
    public interface  ICommunicationParamProvider
    {
        /// <summary>
        /// Get Parameters
        /// </summary>
        /// <returns>CommunicationParameters</returns>
        CommunicationParameters GetParameters();
    }
}
