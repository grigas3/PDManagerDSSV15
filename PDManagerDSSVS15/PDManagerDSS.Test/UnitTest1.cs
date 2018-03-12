using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using PDManager.DSS;
using Newtonsoft.Json;
using System.IO;

namespace PDManagerDSS.Test
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void CreateDSS()
        {

            var inputVariables = new List<string>() { "rigidity", "tremor at rest", "action tremor", "postural tremor", "bradykinesia", "impulsivity", "cognition", "hallucinations", "paranoia", "cardiovascular", "low blood pressure", "hypertension", "offs duration", "dyskinesia intensity", "dyskinesia duration", "age", "activity", "usingMAOI", "usingDA", "usingLD", "maxDA", "maxLD" };
            var inputVariableCodes = new List<string>() { "RIGIDITY", "STTRMR30", "STTRMA30", "STTRMP30", "STBRAD30", "IMPULSIVITY", "COGNITION", "HALLUC", "PARANOIA", "CARDIO", "LBP", "HYPERTENSION", "STOFFDUR", "STDYSS30", "STDYSD30", "Age", "activity", "usingMAOI", "usingDA", "usingLD", "maxDA", "maxLD" };
            var binaryCodes = new List<string>() { "HALLUC", "PARANOIA", "CARDIO", "LBP", "HYPERTENSION", "usingMAOI", "usingDA", "usingLD", "maxDA", "maxLD" };
            DSSConfig config = new DSSConfig()
            {
                Version = "1.0.0",
                Name = "Medication Change",
                AggregationPeriodDays = 30,
                DexiFile = "DexiModels\\ModelHow.dxi",

            };

            config.Input = new List<DSSValueMapping>();
            int i = 0;
            foreach (var c in inputVariables)
            {
                var code = inputVariableCodes[i++];
                if (c.ToLowerInvariant() == "age")
                {
                    config.Input.Add(new DSSValueMapping()
                    {

                        DefaultValue = 0,
                        Source = "Demographics",
                        Code = code.ToUpperInvariant(),
                        Name = c,
                        ValueType = "Numeric",
                        NumericMapping = null,
                        NumericBins = new DSSNumericBinCollection()
                        {

                            new DSSNumericBin()
                            {
                                MinValue=0,
                                MaxValue=65,
                                Value=0,
                                ValueMeaning="Lower than 65"

                            },
                            new DSSNumericBin()
                            {
                                MinValue=65,
                                MaxValue=75,
                                Value=1,
                                ValueMeaning="Between 65 and 75"

                            },
                             new DSSNumericBin()
                            {
                                MinValue=75,
                                MaxValue=1000,
                                Value=2,
                                ValueMeaning="Above 75"

                            }



                        }
                    });

                }
                else if (binaryCodes.Contains(c))
                {

                    config.Input.Add(new DSSValueMapping()
                    {

                        DefaultValue = 0,
                        Source = "Clinical",
                        Code = code.ToUpperInvariant(),
                        Name = c,
                        ValueType = "Categorical",
                        CategoryMapping = new DSSCategoricalValueMappingList()
                    {

                        new DSSCategoricalValueMapping()
                        {
                            Name="yes",
                            ValueMeaning="yes",
                            Value=0
                        },
                      
                          new DSSCategoricalValueMapping()
                        {
                            Name="no",
                              ValueMeaning="no",
                            Value=1
                        }
                    }
                    });

                }
                else
                { 
                    config.Input.Add(new DSSValueMapping()
                    {

                        DefaultValue = 0,
                        Source = "Clinical",
                        Code = code.ToUpperInvariant(),
                        Name = c,
                        ValueType = "Categorical",
                        CategoryMapping = new DSSCategoricalValueMappingList()
                    {

                        new DSSCategoricalValueMapping()
                        {
                            Name="severe",
                            ValueMeaning="yes",
                            Value=0
                        },
                         new DSSCategoricalValueMapping()
                        {
                            Name="moderate",
                            ValueMeaning="yes",
                            Value=0
                        },
                          new DSSCategoricalValueMapping()
                        {
                            Name="mild",
                              ValueMeaning="no",
                            Value=1
                        }
                    }
                    });

                }

            }

            var s=JsonConvert.SerializeObject(config);

            StreamWriter str = new StreamWriter("modelhow.json");


            str.WriteLine(s);

            str.Close();

            Assert.Inconclusive();

        }
    }
}
