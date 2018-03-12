﻿using System;
using System.Collections.Generic;
using System.Text;

namespace PDManager.Common.Interfaces
{
    /// <summary>
    /// Generic Logger
    /// Main Logging is based on Microsoft Extensions Logging
    /// However in order to avoid adding reference to Extensions in Unit Testing we create a wrapper over that
    /// </summary>
    public interface IGenericLogger
    {

        /// <summary>
        /// Log Error
        /// </summary>
        /// <param name="ex">Exception</param>
        /// <param name="message">Additional message</param>
        void LogError(Exception ex, string message);
    }
}
