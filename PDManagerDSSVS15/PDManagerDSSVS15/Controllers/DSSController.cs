using Newtonsoft.Json;
using PDManager.Common.Interfaces;
using PDManagerDSSVS15.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;

namespace PDManagerDSSVS15.Controllers
{
    /// <summary>
    /// DSS Value
    /// This Controller expose two methods
    /// one get method to run DSS evaluation for a specific patient
    /// and one post method to get evaluation based on DSSInput values from a form
    /// </summary>
   
    public class DSSController : ApiController
    {
        #region Private  declaration
        private readonly IDSSRunner _dssRunner;
        private readonly Context.DSSContext _context = new Context.DSSContext();
        #endregion
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="context">DSS Context</param>
        /// <param name="dSSRunner">DSS Runner</param>
        public DSSController(IDSSRunner dSSRunner)
        {
            _dssRunner = dSSRunner;
            
        }

        /// <summary>
        /// Get DSS Evaluation. Based on the DSS definition it will use the last N day observations
        /// to get a DSS output.
        /// Sample call
        /// GET api/dss/execute/5?patientId=2
        /// </summary>
        /// <param name="uid">DSS model by id</param>
        /// <param name="code">DSS model by code If ID is provided it has priority againsty code</param>
        /// <param name="patientId">Patient id</param>
        /// <returns></returns>

        [HttpGet]
        public async Task<IHttpActionResult> Get(string patientId,string uid = null, string code=null)
        {
            DSSModel item = null;

            if (!string.IsNullOrEmpty(uid))
            {
                int key = int.Parse(uid);
                item = _context.Set<DSSModel>().Find(key);
                if (item == null)
                    return NotFound();
            }
            else if(!string.IsNullOrEmpty(code))
            {
                
                item = _context.Set<DSSModel>().FirstOrDefault(e => e.Code == code);
                if (item == null)
                    return NotFound();

            }
            else
            {
                return NotFound();
            }

            //Run DSS
            var res = await _dssRunner.Run(patientId, item.Config);
            //_dssRunner.Run(id)
            return Ok(res);
        }


       




    }
}