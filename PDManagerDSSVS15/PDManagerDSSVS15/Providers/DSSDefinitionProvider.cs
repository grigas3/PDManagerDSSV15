using PDManager.Common.Exceptions;
using PDManager.Common.Interfaces;
using PDManagerDSSVS15.Entities;
using System;
using System.Linq;

namespace PDManagerDSSVS15.Context
{

    /// <summary>
    /// Aggregation Definition Provider
    /// </summary>
    public class DSSDefinitionProvider: IDSSDefinitionProvider
    {

        #region Private Declarations
        private readonly Context.DSSContext _context = new DSSContext();
        #endregion
        /// <summary>
        /// Constructor
        /// </summary>
        
        public DSSDefinitionProvider()
        {
            
            
        }

        /// <summary>
        /// Get Config in JSON format from meta-observation code
        /// </summary>
        /// <param name="code">Meta-observation code</param>
        /// <returns></returns>
        public string GetJsonConfigFromCode(string code)
        {
            if (code == null)
            {
                throw new ArgumentNullException(nameof(code));
            }


            var model=_context.Set<DSSModel>().FirstOrDefault(e => e.Code == code);

            if (model == null)
                throw new DSSDefinitionNotFoundException(code);


            return model.Config;

        }
    }
}
