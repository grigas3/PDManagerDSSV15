using PDManager.Common.Extensions;
using PDManager.Models;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace PDManagerDSSVS15.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Title = "Home Page";

            return View();
        }

        //        [{
        //		"Name": "bradykinesia",
        //		"Code": "STBRAD30",
        //		"Value": "moderate",
        //		"Notes": null,
        //		"Category": "Motor",
        //		"Priority": "Low",
        //		"CreatedBy": "Test",
        //		"Timestamp": 1517309212

        //    }, {
        //		"Name": "tremor at hands",
        //		"Code": "STTRMR30",
        //		"Value": "mild",
        //		"Notes": null,
        //		"Category": "Motor",
        //		"Priority": "Low",
        //		"CreatedBy": "Test",
        //		"Timestamp": 1517309212
        //	}, {
        //		"Name": "tremor at rest",
        //		"Code": "STTRMP30",
        //		"Value": "mild",
        //		"Notes": null,
        //		"Category": "Motor",
        //		"Priority": "Low",
        //		"CreatedBy": "Test",
        //		"Timestamp": 1517309212
        //	}, {
        //		"Name": "dyskinesia intensity",
        //		"Code": "STDYSS30",
        //		"Value": "mild",
        //		"Notes": null,
        //		"Category": "Motor",
        //		"Priority": "Low",
        //		"CreatedBy": "Test",
        //		"Timestamp": 1517309212
        //	}, {
        //		"Name": "dyskinesia duration",
        //		"Code": "STDYSD30",
        //		"Value": "mild",
        //		"Notes": null,
        //		"Category": "Motor",
        //		"Priority": "Low",
        //		"CreatedBy": "Test",
        //		"Timestamp": 1517309212
        //	}, {
        //		"Name": "gait",
        //		"Code": "STUPDRSG",
        //		"Value": "moderate",
        //		"Notes": null,
        //		"Category": "Motor",
        //		"Priority": "High",
        //		"CreatedBy": "Test",
        //		"Timestamp": 1517309212
        //	}, {
        //		"Name": "freezing of gait",
        //		"Code": "STFOG",
        //		"Value": "moderate",
        //		"Notes": null,
        //		"Category": "Motor",
        //		"Priority": "High",
        //		"CreatedBy": "Test",
        //		"Timestamp": 1517309212
        //	}, {
        //		"Name": "offs duration",
        //		"Code": "STOFFDUR",
        //		"Value": "moderate",
        //		"Notes": null,
        //		"Category": "Motor",
        //		"Priority": "High",
        //		"CreatedBy": "Test",
        //		"Timestamp": 1517309212
        //	}, {
        //		"Name": "hallucinations",
        //		"Code": "HALLUC",
        //		"Value": "mild",
        //		"Notes": null,
        //		"Category": "Non-Motor",
        //		"Priority": "Low",
        //		"CreatedBy": "Test",
        //		"Timestamp": 1517309212
        //	}, {
        //		"Name": "BIS-11",
        //		"Code": "BIS11",
        //		"Value": "mild",
        //		"Notes": null,
        //		"Category": "Non-Motor",
        //		"Priority": "Low",
        //		"CreatedBy": "Test",
        //		"Timestamp": 1517309212
        //	}, {
        //		"Name": "Mood",
        //		"Code": "MOOD",
        //		"Value": "mild",
        //		"Notes": null,
        //		"Category": "Non-Motor",
        //		"Priority": "Low",
        //		"CreatedBy": "Test",
        //		"Timestamp": 1517309212
        //	}, {
        //		"Name": "Cognition",
        //		"Code": "COGNITION",
        //		"Value": "mild",
        //		"Notes": null,
        //		"Category": "Non-Motor",
        //		"Priority": "Low",
        //		"CreatedBy": "Test",
        //		"Timestamp": 1517309212
        //	}, {
        //		"Name": "NMSS",
        //		"Code": "NMSS",
        //		"Value": "severe",
        //		"Notes": null,
        //		"Category": "Non-Motor",
        //		"Priority": "Low",
        //		"CreatedBy": "Test",
        //		"Timestamp": 1517309212
        //	}, {
        //		"Name": "Activity",
        //		"Code": "activity",
        //		"Value": "high",
        //		"Notes": null,
        //		"Category": "Non-Motor",
        //		"Priority": "Low",
        //		"CreatedBy": "Test",
        //		"Timestamp": 1517309212
        //	}
        //]

        private Dictionary<string, string> codeDict = new Dictionary<string, string>()
        {
            { "bradykinesia","STBRAD30" },
            {"tremor at hands","STTRMR30" },
            {"tremor at rest","STTRMP30" },
            {"action tremor","STTRMA30" },
            {"posture tremor","STTRMP30" },
            {"postural tremor","STTRMP30" },
            {"dyskinesia intensity","STDYSS30" },
            {"dyskinesia duration","STDYSD30" },
            
            { "Low Blood Pressure","LBD"},
            {"cardiovascular","cardio"},
            //"usingMAOI", "usingDA", "usingLD", "maxDA", "maxLD"
            { "using MAOI","usingMAOI"},
            { "using DA","usingDA"},
            { "using LD","usingLD"},
            { "Max DA","maxDA"},
            { "Max LD","maxLD"},
            {"gait","STUPDRSG" },
            {"freezing of gait", "STFOG"},
            {"fog", "STFOG"},
            {"offs duration", "STOFFDUR"},
            {"hallucinations", "HALLUC"},
            {"bis-11", "BIS11"},
            {"mood", "MOOD"},
            {"cognition","COGNITION" },
            { "nmss","NMSS" },
            {"activity","activity" }
        };
        private Dictionary<string, string> catDict = new Dictionary<string, string>()
        {

            { "rigidity","Motor" },
            { "bradykinesia","Motor" },
            {"tremor at hands","Motor" },
            {"tremor at rest","Motor" },
              {"posture tremor","Motor" },
                {"action tremor","Motor" },
            {"dyskinesia intensity","Motor" },
            {"dyskinesias duration","Motor" },
            {"dyskinesia duration","Motor" },
            {"gait","Motor" },
            {"freezing of gait", "Motor"},
            {"fog", "Motor"},
            {"offs duration", "Motor"},
            {"hallucinations", "Non-motor"},
            {"bis-11", "Non-motor"},
            {"mood", "Non-motor"},
            {"cognition","Non-motor" },
            { "nmss","Non-motor" },
              { "Low Blood Pressure","Non-motor"},
              { "using MAOI","Non-motor"},
            { "using DA","Non-motor"},
            { "using LD","Non-motor"},
            { "Max DA","Non-motor"},
            { "Max LD","Non-motor"},
            {"activity","Motor" }
        };
        [HttpPost]
        public JsonResult Convert(string data)
        {
            ClinicalInfoCollection ret = new ClinicalInfoCollection();
            if (!string.IsNullOrEmpty(data))
            {
                var lines = data.Split('\n');

                foreach (var c in lines)
                {
                    var vals = c.Split('\t', ':', ',');
                    if (vals.Length >= 2)
                    {
                        var v = vals[0].ToLower().Trim();

                       
                        var cat = "Non-Motor";
                        if (catDict.ContainsKey(v))
                            cat = catDict[v];
                        var priority = "Normal";
                        if(vals.Length==3)
                        {

                            if(vals[2].Trim().ToLower().Equals("high")|| vals[2].Trim().ToLower().Equals("low"))
                            priority = vals[2].Trim().ToLower();
                        }

                        if (codeDict.ContainsKey(v))
                        {
                            ret.Add(new PDManager.Models.ClinicalInfo()
                            {
                                Name = v,
                                Code = codeDict[v],
                                Value = vals[1].Trim(),
                                Priority = priority,
                                Category= cat,
                                CreatedBy = "admin",
                                Timestamp = DateTime.Now.ToUnixTimestamp()
                            });
                        }
                        else
                        {


                            ret.Add(new PDManager.Models.ClinicalInfo()
                            {
                                Name = v,
                                Code =v.ToUpperInvariant().Trim(),
                                Value = vals[1].Trim(),
                                Priority = priority,
                                Category = cat,
                                CreatedBy = "admin",
                                Timestamp = DateTime.Now.ToUnixTimestamp()
                            });

                        }
                    }
                }
            }

            return Json(ret);
        }
    }
}