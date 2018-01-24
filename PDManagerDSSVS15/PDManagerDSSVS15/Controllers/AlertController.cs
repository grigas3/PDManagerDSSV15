using PDManager.Common.Enums;
using PDManager.Common.Interfaces;
using PDManagerDSSVS15.Entities;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;

namespace PDManagerDSSVS15.Controllers
{
    /// <summary>
    /// DSS Controller
    /// </summary>
    
    public class AlertController : ApiController
    {
        #region Private Declarations        
        private readonly Context.DSSContext _context = new Context.DSSContext(); 
        private readonly IAlertEvaluator _alertEvaluator;
        #endregion

        #region Controllers
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="context">Data Context</param>
        /// <param name="alertEvaluator"></param>        
        /// <param name="logger">Logger</param>
        public AlertController(IAlertEvaluator alertEvaluator)
        {          
            
            _alertEvaluator = alertEvaluator;
            
        }
        #endregion

        #region Methods

        /// <summary>
        /// Get DSS Config
        /// </summary>
        /// <param name="id"></param>
        /// <param name="patientId"></param>
        /// <returns></returns>


        public async Task<IHttpActionResult> Get(string id,string patientId)
        {

          

                var model = _context.Set<AlertModel>().Find(int.Parse(id));

                if (model == null)
                    return NotFound();

                var ret = await this._alertEvaluator.GetAlertLevel(model,patientId);

           

                return Ok(new {

                    Message=model.Message,
                    Level=ret,
                    Color=GetColor(ret)

                });

            

        }

        private string GetColor(AlertLevel ret)
        {

            switch(ret)
            {

                case AlertLevel.None:return "success";
                case AlertLevel.Low: return "info";
                    case AlertLevel.Medium: return "warning";
                case AlertLevel.High: return "danger";
                default:return "default";

            }

        }

      
        #endregion



    }
}