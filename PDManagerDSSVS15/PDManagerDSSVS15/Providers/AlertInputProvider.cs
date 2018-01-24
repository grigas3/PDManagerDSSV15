using PDManager.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PDManagerDSSVS15.Providers
{
    public class AlertInputProvider:IAlertInputProvider
    {
        private readonly Context.DSSContext _context = new Context.DSSContext();
        public AlertInputProvider()
        {
            
        }

        public IEnumerable<IAlertInput> GetAlertInputs()
        {
            return _context.AlertModels.ToList();
        }
    }
}
