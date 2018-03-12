using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
namespace PDManager.DSS.Dexi
{
    /// <summary>
    /// Distribution consists of:
    /// - double[] distribution
    /// - double cumulative: holds some cumulative value, interpretation-dependent
    /// </summary>

    public class Distribution

    {
        private double[] distribution = null;
        private double cumulative = 0.0;


        /// <summary>
        /// Distribution Constructor
        /// </summary>
        /// <param name="size">Size</param>
        public Distribution(int size)

        {
            distribution = new double[size];
            Clear();
        }


        /// <summary>
        /// Distribution
        /// </summary>
        /// <param name="cum">Cumulative </param>
        /// <param name="distr">Distribution</param>
        public Distribution(double cum, double[] distr)
        {
            cumulative = cum;
            distribution = distr;
        }

        /// <summary>
        /// Distribution 
        /// </summary>
        /// <param name="distr">Distribution</param>
        public Distribution(Distribution distr)
        {
            distribution = new double[distr.Count];
            Array.Copy(distr.distribution, distribution, Count);
            cumulative = distr.cumulative;
        }

        public int Count
        {
            get { return distribution.Length; }
        }

        /// <summary>
        /// Access to individual distribution elements
        /// </summary>
        /// <param name="index"></param>
        /// <returns>distribution[index]</returns>
        public double this[int index]
        {
            get { return distribution[index]; }
            set { distribution[index] = value; }
        }

        public double Cumulative
        {
            get { return cumulative; }
            set { cumulative = value; }
        }

        /// <summary>
        /// Clear distribution: All elements and cumulative are set to 0.0.
        /// </summary>
        public void Clear()
        {
            for (int i = 0; i < Count; i++)
            {
                distribution[i] = 0.0;
            }
            cumulative = 0.0;
        }

        /// <summary>
        /// Fully populate distribution: All elements and cumulatve are set to 1.0.
        /// </summary>
        public void Full()
        {
            for (int i = 0; i < Count; i++)
            {
                distribution[i] = 1.0;
            }
            cumulative = 1.0;
        }

        /// <summary>
        /// Create a uniform distribution with respect to the number of values. Cumulative is set to 1.0.
        /// </summary>
        public void Uniform()
        {
            cumulative = 1.0;
            for (int i = 0; i < Count; i++)
            {
                distribution[i] = 1.0 / Count;
            }
        }

        /// <summary>
        /// Get/Set: Distribution interpreted as a single integer number: All elements are 0.0, except distribution[Single]==1.0.
        /// Get: Returns -1 if distribution does not contain a single integer.
        /// </summary>
        public int Single
        {
            get
            {
                int cnt = CountMembers;
                if (cnt == 1)
                {
                    for (int i = 0; i < Count; i++)
                    {
                        if (distribution[i] != 0.0)
                        {
                            return i;
                        }
                    }
                }
                return -1;
            }
            set
            {
                Clear();
                distribution[value] = 1.0;
                cumulative = 1.0;
            }
        }

        /// <summary>
        /// Counts the number of distribution elements not equal to 0.0.
        /// </summary>
        public int CountMembers
        {
            get
            {
                int cnt = 0;
                for (int i = 0; i < Count; i++)
                {
                    if (distribution[i] != 0.0)
                    {
                        cnt++;
                    }
                }
                return cnt;
            }
        }

        /// <summary>
        /// Get/Set: Distribution interpreted as a set of numbers
        /// </summary>
        public int[] Set
        {
            get
            {
                int cnt = CountMembers;
                if (cnt == 0)
                {
                    return null;
                }
                else
                {
                    int[] set = new int[cnt];
                    int x = 0;
                    for (int i = 0; i < Count; i++)
                    {
                        if (distribution[i] != 0.0)
                        {
                            set[x++] = i;
                        }
                    }
                    return set;
                }
            }
            set
            {
                Clear();
                cumulative = 1.0;
                for (int i = 0; i < value.Length; i++)
                {
                    distribution[i] = 1.0;
                }
            }
        }

        /// <summary>
        /// Get the sum of distribution values.
        /// </summary>
        public double Sum
        {
            get
            {
                double s = 0.0;
                for (int i = 0; i < Count; i++)
                {
                    s += distribution[i];
                }
                return s;
            }
        }

        /// <summary>
        /// Get the maximum of distribution values.
        /// </summary>
        public double Max
        {
            get
            {
                double m = 0.0;
                for (int i = 0; i < Count; i++)
                {
                    m = Math.Max(m, distribution[i]);
                }
                return m;
            }
        }

        /// <summary>
        /// Get the average of distribution values.
        /// </summary>
        public double Average
        {
            get { return Sum / Count; }
        }

        /// <summary>
        /// Get the average index of distribution values.
        /// In general, this is not equal to Average.
        /// </summary>
        public double AverageIndex
        {
            get
            {
                double a = 0.0;
                for (int i = 0; i < Count; i++)
                {
                    a += i * distribution[i];
                }
                return a;
            }
        }

        /// <summary>
        /// Get the weighted sum of distribution values.
        /// </summary>
        public double SumMul
        {
            get { return cumulative * Sum; }
        }

        /// <summary>
        /// Get Sum divided by cumulative. Returns 0.0 if cumulative==0.0.
        /// </summary>
        public double SumDiv
        {
            get
            {
                if (cumulative == 0.0) { return 0.0; }
                else { return Sum / cumulative; }
            }
        }

        /// <summary>
        /// Add distr to current distribution values and cumulative.
        /// </summary>
        /// <param name="distr">Add Distribution</param>
        public void Add(Distribution distr)
        {
            cumulative += distr.cumulative;
            for (int i = 0; i < Count; i++)
            {
                distribution[i] += distr.distribution[i];
            }
        }

        /// <summary>
        /// Add val to distribution[index], add cum to Cumulative.
        /// </summary>
        /// <param name="index">Index</param>
        /// <param name="val">Value</param>
        /// <param name="cum">Cumulation</param>
        public void Add(int index, double val, double cum)
        {
            cumulative += cum;
            distribution[index] += val;
        }

        /// <summary>
        /// Add val to distribution[index], add 1.0 to Cumulative.
        /// </summary>
        /// <param name="index">Index</param>
        /// <param name="val">Value</param>
        public void Add(int index, double val)
        {
            Add(index, val, 1.0);
        }

        /// <summary>
        /// Add 1.0 to distribution[index] and Cumulative.
        /// </summary>
        /// <param name="index">Index</param>
        public void Add(int index)
        {
            Add(index, 1.0, 1.0);
        }

        /// <summary>
        /// Divide all elements and cumulative by by.
        /// </summary>
        /// <param name="by">Divided by</param>
        public void DivBy(double by)
        {
            cumulative /= by;
            for (int i = 0; i < Count; i++)
            {
                distribution[i] /= by;
            }
        }

        /// <summary>
        /// Multiply all elements and cumulative by by.
        /// </summary>
        /// <param name="by">Multiplied by</param>
        public void MulBy(double by)
        {
            cumulative *= by;
            for (int i = 0; i < Count; i++)
            {
                distribution[i] *= by;
            }
        }

        /// <summary>
        /// Normalize distribution by dividing all elements by n. Cumulative is set to 1.0.
        /// </summary>
        public void Normalize(double n)
        {
            DivBy(n);
            cumulative = 1.0;
        }

        /// <summary>
        /// Normalize distribution by dividing all elements by cumulative.
        /// </summary>
        public void Normalize()
        {
            Normalize(cumulative);
        }

        /// <summary>
        /// Normalize distribution so that Sum==1.0.
        /// </summary>
        public void NormalizeSum()
        {
            Normalize(Sum);
        }

        /// <summary>
        /// Normalize distribution so that Max==1.0.
        /// </summary>
        public void NormalizeMax()
        {
            Normalize(Max);
        }

        /// <summary>
        /// Convert distribution to a set by setting all non-zero elements and cumulative to 1.0.
        /// </summary>
        public void NormalizeSet()
        {
            cumulative = 1.0;
            for (int i = 0; i < Count; i++)
            {
                if (distribution[i] != 0.0)
                {
                    distribution[i] = 1.0;
                }
            }
        }

        /// <summary>
        /// Increment distribution size by one. Insert new 0.0 element at distribution[0].
        /// </summary>
        public void Increment()
        {
            double[] newdistr = new double[Count + 1];
            Array.Copy(distribution, 0, newdistr, 1, Count);
            newdistr[0] = 0.0;
            distribution = newdistr;
        }

        /// <summary>
        /// To string. Distribution to string
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            bool empty = true;
            if (cumulative != 1.0)
                sb.Append("[" + Model.FormatDouble(cumulative) + "]");
            for (int i = 0; i < Count; i++)
            {
                if (distribution[i] != 0.0)
                {
                    if (!empty) sb.Append(";");
                    empty = false;
                    sb.Append(i);
                    if (distribution[i] != 1.0)
                    {
                        sb.Append("/" + Model.FormatDouble(distribution[i]));
                    }
                }
            }
            return sb.ToString();
        }
    }

    /// <summary>
    /// Value class. Represents a single (non-distributed) value that can be assigned to an attribute.
    /// Contains elements:
    /// - string name: value name;
    /// - int ordinal: corresponding ordinal value.
    /// </summary>
    public class Value
    {
        /// <summary>
        /// Name
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Ordinal
        /// </summary>
        public int Ordinal { get; set; }

        /// <summary>
        /// Value
        /// </summary>
        /// <param name="name">Name</param>
        /// <param name="ordinal">Ordinal</param>
        public Value(string name, int ordinal)
        {
            Name = name;
            Ordinal = ordinal;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="value">Value</param>
        public Value(Value value)
        {
            Name = value.Name;
            Ordinal = value.Ordinal;
        }

        /// <summary>
        /// To String
        /// </summary>
        /// <returns>Value as string with format "{Name}={Ordinal}"</returns>
        public override string ToString()
        {
            return string.Format("{0}={1}", Name, Ordinal);
        }

        public void Write(StreamWriter writer)
        {
            writer.WriteLine("Value:");
            writer.WriteLine("Name: " + Name);
            writer.WriteLine("Ordinal: " + Ordinal);
        }
    }

    /// <summary>
    /// Variable class. Represents a named variable with assigned string value.
    /// </summary>
    public class Variable
    {

        /// <summary>
        /// Variable Name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Variable Value
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">Name</param>
        /// <param name="value">Value</param>
        public Variable(string name, string value)
        {
            Name = name;
            Value = value;
        }
        /// <summary>
        /// Variable
        /// </summary>
        /// <param name="hash">Has split with =</param>
        public Variable(string hash)
        {
            string[] str = hash.Split('=');
            if (str.Length != 2)
            {
                throw new ArgumentException("Variable: Invalid value " + hash);
            }
            if (String.IsNullOrEmpty(str[0]))
            {
                throw new ArgumentException("Variable: Name is null");
            }
            if (String.IsNullOrEmpty(str[1]))
            {
                throw new ArgumentException("Variable: Value is null");
            }
            Name = str[0];
            Value = str[1];
        }
    }

    /// <summary>
    /// VariableList class. Contains a list of Variable objects.
    /// </summary>
    public class VariableList
    {
        /// <summary>
        /// List of variables
        /// </summary>
        public List<Variable> Variables;


        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="list">List of vairables</param>
        public VariableList(string list)
        {
            string[] hash = null;
            Variables = new List<Variable>();

            if (!String.IsNullOrEmpty(list))
            {
                hash = list.Split(';');
                for (int i = 0; i < hash.Length; i++)
                {
                    var namevalue = hash[i];
                    var name = namevalue.Split('=')[0];
                    if ((i > 0) && (FindVariable(name) != null))
                    {
                        throw new ArgumentException("VariableList: Variable with this name already exists: " + name);
                    }
                    Variable variable = new Variable(namevalue);
                    Variables.Add(variable);
                }
            }
        }

        /// <summary>
        /// Find variable by name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns>Variable; returns null if not found.</returns>
        public Variable FindVariable(string name)
        {
            foreach (Variable var in Variables)
            {
                if (var.Name == name)
                {
                    return var;
                }
            }
            return null;
        }

        /// <summary>
        /// Variables Count
        /// </summary>
        public int Count
        {
            get { return Variables.Count; }
        }

        /// <summary>
        /// Provides access to individual variables.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public Variable this[int index]
        {
            get { return Variables[index]; }
            set { Variables[index] = value; }
        }
    }

    /// <summary>
    ///  ScaleValue class. Represents a single (non-distributed) scale value.
    ///  Contains elements:
    ///  - string name: value name;
    ///  - string description: value description;
    ///  - string group: typically "BAD", "GOOD", or "".
    /// </summary>
    public class ScaleValue
    {
        /// <summary>
        /// Name
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Description
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// Group
        /// </summary>
        public string Group { get; set; }

        /// <summary>
        /// Scale Value Constructor
        /// </summary>
        /// <param name="scl">Xml</param>
        public ScaleValue(XmlNode scl)
        {
            if (!scl.HasChildNodes)
            {
                throw new ArgumentException("ScaleValue: Does not have child nodes");
            }
            foreach (XmlNode node in scl.ChildNodes)
            {
                if (node.Name.Equals("NAME"))
                {
                    Name = node.FirstChild.Value;
                }
                else if (node.Name.Equals("DESCRIPTION"))
                {
                    Description = node.FirstChild.Value;
                }
                else if (node.Name.Equals("GROUP"))
                {
                    Group = node.FirstChild.Value;
                }
            }
            if (String.IsNullOrEmpty(Name))
            {
                throw new ArgumentException("ScaleValue: Name is undefined");
            }
        }

        public void Write(StreamWriter writer)
        {
            writer.WriteLine("ScaleValue:");
            writer.WriteLine("Name: " + Name);
            writer.WriteLine("Description: " + Description);
            writer.WriteLine("Group: " + Group);
        }
    }

    /// <summary>
    /// Scale class. Scale contains a list of ScaleValues.
    /// </summary>
    public class Scale
    {
        private List<ScaleValue> ScaleValues;

        /// <summary>
        /// Scale
        /// </summary>
        /// <param name="scl">Xml</param>
        public Scale(XmlNode scl)
        {
            ScaleValues = new List<ScaleValue>();
            foreach (XmlNode node in scl)
            {
                if (node.Name.Equals("SCALEVALUE") && node.HasChildNodes)
                {
                    ScaleValue scaleValue = new ScaleValue(node);
                    ScaleValues.Add(scaleValue);
                }
            }
        }

        /// <summary>
        /// Find Value by name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns>If not found: null.</returns>
        public Value FindValue(string name)
        {
            Value value = null;
            for (int i = 0; i < ScaleValues.Count; i++)
            {
                ScaleValue scalevalue = ScaleValues[i];
                if (scalevalue.Name.Equals(name))
                {
                    value = new Value(scalevalue.Name, i);
                }
            }
            return value;
        }

        /// <summary>
        /// Find Value by ordinal number.
        /// </summary>
        /// <param name="ordinal"></param>
        /// <returns>If out of bounds: null.</returns>
        public Value FindValue(int ordinal)
        {
            Value value = null;
            if (ordinal < ScaleValues.Count)
            {
                ScaleValue scalevalue = ScaleValues[ordinal];
                value = new Value(scalevalue.Name, ordinal);
            }
            return value;
        }

        /// <summary>
        /// Count of Scale Values
        /// </summary>

        public int Count
        {
            get { return ScaleValues.Count; }
        }

        /// <summary>
        /// Scale Value Accessor
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public ScaleValue this[int index]
        {
            get { return ScaleValues[index]; }
            set { ScaleValues[index] = value; }
        }

        /// <summary>
        /// Write to stream
        /// </summary>
        /// <param name="writer">Output stream</param>
        public void Write(StreamWriter writer)
        {
            writer.WriteLine("Scale:");
            writer.WriteLine("Count: " + Count);
            for (int i = 0; i < Count; i++)
            {
                writer.WriteLine("ScaleValue[" + i + "]:");
                this[i].Write(writer);
            }
        }
    }

    /// <summary>
    /// Rule class. Represents a single rule from a DEXi utility function.
    /// Rule consists of two ordinal values, Low and High, which represent the lower and
    /// upper bound of the rule value interval. Rule is also:
    /// - Entered: explicitly entered by a DEXi user (otherwise is calculated by DEXi),
    /// - Explicit: when Low==High).
    /// </summary>
    public class Rule
    {
        /// <summary>
        /// Low
        /// </summary>
        public int Low { get; set; }
        /// <summary>
        /// High
        /// </summary>
        public int High { get; set; }
        /// <summary>
        /// Entered
        /// </summary>
        public bool Entered { get; set; }
        /// <summary>
        /// Explicit
        /// </summary>
        public bool Explicit { get; set; }

        /// <summary>
        /// Rule Constructor
        /// </summary>
        /// <param name="low">Low</param>
        public Rule(int low) : this(low, low, true)
        {
        }

        /// <summary>
        /// Rule Constructor
        /// </summary>
        /// <param name="low">Low</param>
        /// <param name="high">High</param>
        public Rule(int low, int high) : this(low, high, true)
        {
        }
        /// <summary>
        /// Rule Constructor
        /// </summary>
        /// <param name="low">Low</param>
        /// <param name="entered">Entered. explicitly entered by a DEXi user</param>
        public Rule(int low, bool entered) : this(low, low, entered)
        {
        }

        /// <summary>
        /// Rule Constructor
        /// </summary>
        /// <param name="low">Low</param>
        /// <param name="high">High</param>
        /// <param name="entered">Entered. explicitly entered by a DEXi user</param>
        public Rule(int low, int high, bool entered)
        {
            if (low < 0)
            {
                throw new ArgumentException("Rule: Low is negative");
            }
            if (high < 0)
            {
                throw new ArgumentException("Rule: High is negative");
            }
            if (low > high)
            {
                throw new ArgumentException("Rule: Low > High: " + low + " " + high);
            }
            Low = low;
            High = high;
            Entered = entered;
            Explicit = (low == high) && entered;
        }

        /// <summary>
        /// Set Low = High = Value.
        /// </summary>
        public int Value
        {
            set { Low = value; High = Low; }
        }

        public void Write(StreamWriter writer)
        {
            writer.WriteLine("Rule:");
            if (Low == High)
            {
                writer.WriteLine("Value: " + Low);
            }
            else
            {
                writer.WriteLine("Low: " + Low);
                writer.WriteLine("High: " + High);
            }
            writer.WriteLine("Entered: " + Entered);
            writer.WriteLine("Explicit: " + Explicit);
        }
    }

    /// <summary>
    /// An Attribute class. Attribute is a DEX variable, usually a member of a hierarchical structure.
    /// An Attribute consistes of:
    /// - string Name: atribute name;
    /// - string Description: a textual description;
    /// - Scale Dcale: list of values that can be assigned to the attribute;
    /// - List Rule function: utility function (a mapping from descendant attributes to this attribute);
    /// - List Attribute Attributes: array of this attribute's immediate descendants in attribute hierarchy;
    /// - Attribute Link: linked attribute (considered "logically" the same as this attribute);
    /// - int Level: depth level in the tree (root.level==0)
    /// - Distribution Values: current value distribution assigned to the attribute.
    /// </summary>
    public class Attribute
    {
        private List<Attribute> attributes = null;
        private List<Rule> function = null;
        private Scale scale = null;
        private Attribute link = null;

        /// <summary>
        /// Attribute Name
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Attribute Description
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Completeness
        /// </summary>
        public bool Completeness { get; set; }
        /// <summary>
        /// Explicitness
        /// </summary>
        public bool Explicitness { get; set; }
        /// <summary>
        /// Level
        /// </summary>
        public int Level { get; set; }
        /// <summary>
        /// List of attributes
        /// </summary>
        public List<Attribute> Attributes { get { return attributes; } }
        /// <summary>
        /// Lin Attribute
        /// </summary>
        public Attribute Link { get { return link; } }
        /// <summary>
        /// Scale
        /// </summary>
        public Scale Scale { get { return scale; } }
        /// <summary>
        /// Distribution
        /// </summary>
        public Distribution Values { get; set; }

        /// <summary>
        /// Scale Size
        /// </summary>
        public int ScaleSize
        {
            get { return (scale == null) ? 0 : scale.Count; }
        }

        /// <summary>
        /// Inputs
        /// </summary>
        public int Inputs { get { return attributes.Count; } }

        /// <summary>
        /// Attribute Count
        /// </summary>
        public int AttributeCount
        {
            get
            {
                if (attributes == null)
                {
                    return -1;
                }
                else
                {
                    return attributes.Count;
                }
            }
        }

        /// <summary>
        /// Rule Count
        /// </summary>
        public int RuleCount
        {
            get
            {
                if (function == null)
                {
                    return -1;
                }
                else
                {
                    return function.Count;
                }
            }
        }

        /// <summary>
        /// Attribute
        /// </summary>
        /// <param name="att">XmlNode</param>
        /// <param name="level">level</param>
        public Attribute(XmlNode att, int level)
        {
            attributes = new List<Attribute>();
            Level = level;
            foreach (XmlNode node in att.ChildNodes)
            {
                if (node.Name.Equals("NAME"))
                {
                    Name = node.FirstChild.Value;
                }
                else if (node.Name.Equals("DESCRIPTION"))
                {
                    Description = node.FirstChild.Value;
                }
                else if (node.Name.Equals("SCALE"))
                {
                    scale = new Scale(node);
                }
                else if (node.Name.Equals("FUNCTION"))
                {
                    ParseFunction(node);
                }
                else if (node.Name.Equals("ATTRIBUTE"))
                {
                    attributes.Add(new Attribute(node, level + 1));
                }
            }
            if (String.IsNullOrEmpty(Name))
            {
                throw new ArgumentException("Attribute: Name is undefined");
            }
            CheckExplicitness();
            CheckCompleteness();
        }

        private void ParseFunction(XmlNode fnc)
        {
            string low = null;
            string high = null;
            string entered = null;
            function = new List<Rule>();
            foreach (XmlNode node in fnc.ChildNodes)
            {
                if (node.Name.Equals("LOW"))
                {
                    low = node.FirstChild.Value;
                }
                else if (node.Name.Equals("HIGH"))
                {
                    high = node.FirstChild.Value;
                }
                else if (node.Name.Equals("ENTERED"))
                {
                    entered = node.FirstChild.Value;
                }
            }
            if (high == null)
            {
                high = low;
            }
            int[] iLow = ParseFunction(low);
            int[] iHigh = ParseFunction(high);
            bool[] bEntered = null;
            if (entered != null)
            {
                bEntered = ParseEntered(entered);
            }
            if ((iHigh.Length != iLow.Length) || ((bEntered != null) && (bEntered.Length != iLow.Length)))
            {
                throw new ArgumentException("Function: Erroroneus representation");
            }
            for (int l = 0; l < iLow.Length; l++)
            {
                Rule rule = new Rule(iLow[l], iHigh[l], (bEntered == null) || bEntered[l]);
                function.Add(rule);
            }
        }

        private static int[] ParseFunction(string value)
        {
            int[] ints = null;
            char[] chars = value.ToCharArray();
            ints = new int[chars.Length];
            for (int i = 0; i < chars.Length; i++)
            {
                ints[i] = int.Parse(chars[i].ToString());
            }
            return ints;
        }

        private static bool[] ParseEntered(string value)
        {
            bool[] entered = null;
            char[] chars = value.ToCharArray();
            entered = new bool[chars.Length];
            for (int i = 0; i < chars.Length; i++)
            {
                entered[i] = chars[i] == '+';
            }
            return entered;
        }

        /// <summary>
        /// Check completeness of subtree of attributes.
        /// </summary>
        protected void CheckCompleteness()
        {
            bool complete = true;
            int product = 1;
            foreach (Attribute att in attributes)
            {
                if (att.ScaleSize != 0)
                {
                    product = product * att.ScaleSize;
                }
                complete = complete && att.Completeness;
            }
            complete = complete && (this.ScaleSize > 0);
            complete = complete && ((function == null) || (product == function.Count));
            Completeness = complete;
        }

        /// <summary>
        /// Check explicitness of subtree of attributes.
        /// </summary>
        protected void CheckExplicitness()
        {
            bool explicite = true;
            foreach (Attribute att in attributes)
            {
                explicite = explicite && att.Explicitness;
            }
            if (function != null)
            {
                foreach (Rule rule in function)
                {
                    explicite = explicite && rule.Explicit;
                }
            }
            Explicitness = explicite;
        }

        /// <summary>
        /// Add attribute subtree to list.
        /// </summary>
        /// <param name="list">Current list of attributes, populated by this method</param>
        public void AddAttributes(List<Attribute> list)
        {
            list.Add(this);
            foreach (Attribute att in attributes)
            {
                att.AddAttributes(list);
            }
        }

        /// <summary>
        /// Checks whether or not this attribute affects other attributes.
        /// </summary>
        /// <param name="att">Attribute</param>
        /// <returns>True/False</returns>
        public bool Affects(Attribute att)
        {
            return att.Depends(this);
        }

        /// <summary>
        /// Checks whether or not this attribute depends on other attributes.
        /// </summary>
        /// <param name="att">Attribute</param>
        /// <returns>True/False</returns>
        public bool Depends(Attribute att)
        {
            foreach (Attribute a in attributes)
            {
                if (att == a)
                    return true;
                if (a.Depends(att))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Find an attribute from list that is a candidate for linking with attribute named name.
        /// </summary>
        /// <param name="name">Name of attribute to find</param>
        /// <param name="list">List of attributes</param>
        /// <returns>Attribute</returns>
        private Attribute FindLinkAttribute(string name, List<Attribute> list)
        {
            Attribute agg = null;
            Attribute bas = null;
            int aggcnt = 0;
            foreach (Attribute att in list)
            {
                if (att.Name.Equals(name))
                {
                    if (att.link != null)
                    {
                        // not a candidate any more
                    }
                    else if (att.attributes.Count == 0)
                    {
                        bas = att;
                    }
                    else
                    {
                        agg = att;
                        aggcnt++;
                    }
                }
            }
            if (agg != null && aggcnt == 1)
                return agg;
            else
                return bas;
        }

        /// <summary>
        /// Try to link this.attribute and all its descendants with candidate attributes in a list.
        /// </summary>
        /// <param name="list">List of attributes</param>
        public void LinkAttribute(List<Attribute> list)
        {
            link = null;
            if (attributes.Count == 0)
            {
                Attribute lnk = FindLinkAttribute(Name, list);
                if (lnk == this)
                {
                    lnk = null;
                }
                else if (lnk != null)
                {
                    if (Affects(lnk))
                    {
                        lnk = null;
                    }
                    else if (scale == null)
                    {
                        if (lnk.scale == null)
                        {
                            // ok
                        }
                        else
                            lnk = null;
                    }
                    else if (ScaleSize != lnk.ScaleSize)
                    {
                        lnk = null;
                    }
                }
                link = lnk;
            }
            foreach (Attribute att in attributes)
            {
                att.LinkAttribute(list);
            }
        }

        /// <summary>
        /// Converts an integer array of function argument values to rule index.
        /// </summary>
        /// <param name="args">List of args</param>
        /// <returns>index</returns>
        public int ArgsToIndex(int[] args)
        {
            int index = 0;
            int factor = 1;
            for (int i = args.Length - 1; i >= 0; i--)
            {
                int subord = args[i];
                int subsize = attributes[i].ScaleSize;
                index = index + (factor * subord);
                factor = factor * subsize;
            }
            return index;
        }

        /// <summary>
        /// Converts rule index to corresponding function argument values.
        /// </summary>
        /// <param name="index">Index</param>
        /// <returns>index array</returns>
        public int[] IndexToArgs(int index)
        {
            int[] result = new int[AttributeCount];

            for (int i = result.Length - 1; i >= 0; i--)
            {
                int subsize = attributes[i].ScaleSize;
                result[i] = index % subsize;
                index = index / subsize;
            }
            return result;
        }

        /// <summary>
        /// Calculate function value with respect to args[].
        /// </summary>
        /// <param name="args">List of args</param>
        /// <returns>Function Value</returns>
        public Rule FunctionValue(int[] args)
        {
            return function[ArgsToIndex(args)];
        }

        /// <summary>
        /// Evaluation of this.attribute and its descendants.
        /// </summary>
        /// <param name="evalType">Evaluation type.</param>
        /// <param name="normalize">If true, normalize results after evaluation.</param>
        public void Evaluate(Model.Evaluation evalType, bool normalize)
        {
            if (Values != null)
                return;
            if (attributes.Count > 0)
            {
                if (scale == null || function == null)
                {
                    return;
                }
                else
                {
                    foreach (Attribute att in attributes)
                    {
                        att.Evaluate(evalType, normalize);
                    }
                    CalculateDistr(evalType);
                }
            }
            else if (link != null)
            {
                link.Evaluate(evalType, normalize);
                Values = new Distribution(link.Values);
            }
        }

        /// <summary>
        /// Normalize value distribution, currently assigned to this.attribute.
        /// </summary>
        /// <param name="evalType"></param>
        private void NormalizeDistr(Model.Evaluation evalType)
        {
            switch (evalType)
            {
                case Model.Evaluation.SET:
                    Values.NormalizeSet();
                    break;

                case Model.Evaluation.PROB:
                    Values.NormalizeSum();
                    break;

                case Model.Evaluation.FUZZY:
                    Values.NormalizeMax();
                    break;
            }
        }

        /// <summary>
        /// Calculate distribution value that corresponds to distribution values
        /// assigned to this.attribute's descendants' distributions.
        /// </summary>
        /// <param name="args">Function arguments - ordinal numbers.</param>
        /// <param name="evalType">Evaluation type</param>
        /// <returns>Distribution value</returns>
        protected double DistributionValue(int[] args, Model.Evaluation evalType)
        {
            double value = 1.0;
            if (evalType != Model.Evaluation.SET)
            {
                for (int i = 0; i < args.Length; i++)
                {
                    Attribute att = attributes[i];
                    double val = att.Values[args[i]];
                    switch (evalType)
                    {
                        case Model.Evaluation.SET:
                            break;

                        case Model.Evaluation.PROB:
                            value *= val;
                            break;

                        case Model.Evaluation.FUZZY:
                            value = Math.Min(value, val);
                            break;
                    }
                }
            }
            return value;
        }

        /// <summary>
        /// Calculate this.attribute's value distribution with respect to value
        /// distributions of its descendants in the model. This is the main
        /// evaluation method for an individual attribute.
        /// </summary>
        /// <param name="evalType">Evaluation Type</param>
        protected void CalculateDistr(Model.Evaluation evalType)
        {
            foreach (Attribute att in attributes)
            {
                if (att.Values == null)
                    return;
                if (att.Values.Count == 0)
                    return;
            }

            int[][] pars = new int[attributes.Count][];
            for (int i = 0; i < attributes.Count; i++)
            {
                Attribute att = attributes[i];
                pars[i] = att.Values.Set;
            }

            int[] paridx = new int[attributes.Count];
            int[] args = new int[attributes.Count];
            Values = new Distribution(scale.Count);

            int carry = 0;
            while (carry == 0)
            {
                for (int i = 0; i < args.Length; i++)
                {
                    args[i] = pars[i][paridx[i]];
                }
                double factor = DistributionValue(args, evalType);
                Rule rule = FunctionValue(args);
                int low = rule.Low;
                int high = rule.High;
                if (evalType == Model.Evaluation.PROB && low < high)
                {
                    factor /= high - low + 1;
                }
                switch (evalType)
                {
                    case Model.Evaluation.SET:
                        for (int i = low; i <= high; i++)
                        {
                            Values[i] = 1.0;
                        }
                        break;

                    case Model.Evaluation.PROB:
                        for (int i = low; i <= high; i++)
                        {
                            Values[i] = Values[i] + factor;
                        }
                        break;

                    case Model.Evaluation.FUZZY:
                        for (int i = low; i <= high; i++)
                        {
                            Values[i] = Math.Max(Values[i], factor);
                        }
                        break;
                }
                carry = 1;
                for (int i = paridx.Length - 1; i >= 0; i--)
                {
                    paridx[i] += carry;
                    if (paridx[i] < pars[i].Length)
                    {
                        carry = 0;
                        break;
                    }
                    else
                    {
                        paridx[i] = 0;
                    }
                }
                Values.Cumulative = 1.0;
            }
        }

        /// <summary>
        /// Set attribute value distribution to a single integer value.
        /// </summary>
        /// <param name="value">Input value</param>
        public void SetValue(int value)
        {
            Values.Single = value;
        }

        /// <summary>
        /// Set Value[value.Ordinal] to mem.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="mem">Value element membership, typically in the interval [0.0,1.0].</param>
        /// <param name="clear">If true (default), clear Values before assignment.</param>
        public void SetValue(Value value, double mem = 1.0, bool clear = true)
        {
            SetValue(value.Ordinal, mem, clear);
        }

        /// <summary>
        /// Set Value by value name.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="mem">Value element membership, , typically in the interval [0.0,1.0].</param>
        /// <param name="clear">If true (default), clear Values before assignment.</param>
        public void SetValue(string value, double mem = 1.0, bool clear = true)
        {
            Value scalevalue = scale.FindValue(value);
            if (scalevalue == null)
            {
                throw new ArgumentException("Attribute: Unknown value name: " + value);
            }
            SetValue(scalevalue, mem, clear);
        }

        /// <summary>
        /// Set Values[value] to mem.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="mem">Value element membership, typically in the interval [0.0,1.0]. Default 1.0.</param>
        /// <param name="clear">If true (default), clear Values before assignment.</param>
        public void SetValue(int value, double mem = 1.0, bool clear = true)
        {
            if (Values == null)
            {
                Values = new Distribution(scale.Count);
                Values.Cumulative = 1.0;
            }
            else if (clear)
            {
                Values.Clear();
            }
            Values.Cumulative = Values.Cumulative - Values[value] + mem;
            Values[value] = mem;
        }

        /// <summary>
        /// Set the current value distribution to null.
        /// </summary>
        public void ClearValues()
        {
            this.Values = null;
        }

        /// <summary>
        /// Set Values to distr.
        /// </summary>
        /// <param name="distr">Distribution</param>
        public void SetDistr(Distribution distr)
        {
            if (scale == null)
            {
                throw new ArgumentException("Attribute.SetDistr: Scale is null");
            }
            if (distr.Count != scale.Count)
            {
                throw new ArgumentException("Attribute.SetDistr: Distribution size does not match scale size");
            }
            Values = new Distribution(distr);
        }

        /// <summary>
        /// Set Values to distr.
        /// </summary>
        /// <param name="distr">Distribution</param>
        public void SetDistr(double[] distr)
        {
            if (scale == null)
            {
                throw new ArgumentException("Attribute.SetDistr: Scale is null");
            }
            if (distr.Length != scale.Count)
            {
                throw new ArgumentException("Attribute.SetDistr: Distribution size does not match scale size");
            }
            Values = new Distribution(1.0, distr);
        }

        /// <summary>
        /// Get the name of index-th value of the scale attached to this attribute.
        /// </summary>
        /// <param name="index">Index</param>
        /// <returns>Scale as string</returns>
        public string ScaleValue(int index)
        {
            return scale.FindValue(index).Name;
        }

        /// <summary>
        /// Get index-th rule of the function attached to this attribute.
        /// </summary>
        /// <param name="index">Index</param>
        /// <returns>Rule</returns>
        public Rule Rule(int index)
        {
            return function[index];
        }

        /// <summary>
        /// Compare argument arrays.
        /// Limitation: assumes that all scales are increasing.
        /// </summary>
        /// <param name="args1"></param>
        /// <param name="args2"></param>
        /// <returns>null if args1 and args2 are incomparable,
        /// negative value if args1&lt;args2,
        /// zero if args1==args2,
        /// and positive value if args1>args2.</returns>
        public int? CompareArgs(int[] args1, int[] args2)
        {
            if (args1 == null || args2 == null || args1.Length != args2.Length)
            {
                return null;
            }
            int lts = 0;
            int gts = 0;
            for (int i = 0; i < args1.Length; i++)
            {
                if (args1[i] < args2[i])
                {
                    lts++;
                }
                else if (args1[i] > args2[i])
                {
                    gts++;
                }
            }
            if (lts == 0)
            {
                if (gts == 0)
                {
                    return 0;
                }
                else
                {
                    return gts;
                }
            }
            else
            {
                if (gts == 0)
                {
                    return -lts;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Compare arguments of rules indexed by r1 and r2.
        /// </summary>
        /// <param name="r1"></param>
        /// <param name="r2"></param>
        /// <returns>See CompareArgs.</returns>
        public int? CompareRules(int r1, int r2)
        {
            return CompareArgs(IndexToArgs(r1), IndexToArgs(r2));
        }

        /// <summary>
        /// Rule Bounds
        /// </summary>
        /// <param name="x">input </param>
        /// <returns>Rule applied</returns>
        public Rule RuleBounds(int x)
        {
            Rule result = new Rule(0, ScaleSize - 1, Rule(x).Entered);
            for (int i = 0; i < RuleCount; i++)
            {
                Rule r = Rule(i);
                if (i != x && r.Entered)
                {
                    int? c = CompareRules(i, x);
                    if (c.HasValue)
                    {
                        if (c.Value < 0)
                        {
                            if (r.Low > result.Low)
                            {
                                result.Low = r.Low;
                            }
                        }
                        else if (c.Value > 0)
                        {
                            if (r.High < result.High)
                            {
                                result.High = r.High;
                            }
                        }
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Tests whether rule x is consistent with respect to bounds.
        /// Also, all bounds are checked to lie within the scale range.
        /// </summary>
        /// <param name="x">Input value</param>
        /// <param name="bounds">Rule</param>
        /// <returns>True/False</returns>
        public bool RuleIsConsistent(int x, Rule bounds)
        {
            Rule r = Rule(x);
            int l = r.Low;
            int h = r.High;
            int bl = bounds.Low;
            int bh = bounds.High;
            int s = ScaleSize;
            return l <= h && l >= 0 && h >= 0 && l < s && h < s && bl <= bh
                    && bl >= 0 && bh >= 0 && bl < s && bh < s && l >= bl && l <= bh
                    && h >= bl && h <= bh;
        }

        /// <summary>
        /// Tests whether rule x is consistent according to the dominance principle.
        /// </summary>
        /// <param name="x">Input value</param>
        /// <returns>True/false</returns>
        public bool RuleIsConsistent(int x)
        {
            return RuleIsConsistent(x, RuleBounds(x));
        }

        /// <summary>
        /// Tests whether the entire function is consistent.
        /// </summary>
        /// <returns>True of false</returns>
        public bool FunctionIsConsistent()
        {
            for (int r = 0; r < RuleCount; r++)
            {
                if (!RuleIsConsistent(r))
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Represents function with a string. String contains function values in the
        /// standard order, separated by ";". Single values are represented as single
        /// numbers, and intervals are represented as "low:high". Non-entered
        /// function values are preceded with "-".
        /// </summary>
        public string FunctionString
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                for (int r = 0; r < RuleCount; r++)
                {
                    Rule rule = Rule(r);
                    if (r > 0)
                    {
                        sb.Append(";");
                    }
                    if (!rule.Entered)
                    {
                        sb.Append("-");
                    }
                    if (rule.Low == rule.High)
                    {
                        sb.Append(rule.Low);
                    }
                    else
                    {
                        sb.Append(rule.Low + ":" + rule.High);
                    }
                }
                return sb.ToString();
            }
            set
            {
                string[] vals = value.Split(';');
                for (int v = 0; v < vals.Length; v++)
                {
                    Rule rule = Rule(v);
                    string val = vals[v];
                    rule.Entered = !val.StartsWith("-");
                    if (!rule.Entered)
                    {
                        val = val.Substring(1);
                    }
                    if (val.Contains(":"))
                    {
                        string[] lh = val.Split(':');
                        rule.Low = Convert.ToInt32(lh[0]);
                        rule.High = Convert.ToInt32(lh[1]);
                    }
                    else
                    {
                        rule.Value = Convert.ToInt32(val);
                    }
                }
            }
        }

        /// <summary>
        /// Values as String
        /// </summary>
        public string ValuesString
        {
            get
            {

                if (Values == null)
                    return "??";
                StringBuilder sb = new StringBuilder();
                bool empty = true;
                if (Values.Cumulative != 1.0)
                    sb.Append("[" + Model.FormatDouble(Values.Cumulative) + "]");
                for (int i = 0; i < Values.Count; i++)
                {
                    double fact = Values[i];
                    if (fact != 0.0)
                    {
                        if (!empty)
                            sb.Append(",");
                        empty = false;
                        sb.Append(scale.FindValue(i).Name);
                        if (fact != 1.0)
                        {
                            sb.Append("/" + Model.FormatDouble(fact));
                        }
                    }
                }
                return sb.ToString();
            }
        }

        /// <summary>
        /// Write to stream
        /// </summary>
        /// <param name="writer">Input Stream</param>
        public void Write(StreamWriter writer)
        {
            writer.WriteLine("Attribute:");
            writer.WriteLine("Name: " + Name);
            writer.WriteLine("Description: " + Description);
            writer.WriteLine("Completeness: " + Completeness);
            writer.WriteLine("Explicitness: " + Explicitness);
            if (scale != null)
            {
                scale.Write(writer);
            }
            if (function != null)
            {
                for (int f = 0; f < function.Count; f++)
                {
                    writer.WriteLine("Rule[" + f + "] ");
                    Rule(f).Write(writer);
                }
            }
            for (int i = 0; i < attributes.Count; i++)
            {
                writer.WriteLine("SubAttribute[" + i + "]:");
                attributes[i].Write(writer);
            }
        }
    }

    /// <summary>
    /// Model class. Represents a single DEXi model.
    /// </summary>
    public class Model
    {
        /// <summary>
        /// Evaluation types, which specify hot to evaluate value distributions:
        /// - SET: set-based evaluation (this is standard and the only DEXi behavior).
        /// - PROB: probabilistic evaluation (extension to DEXi).
        /// - FUZZY: fuzzy evaluation (extension to DEXi).
        /// </summary>
        public enum Evaluation { SET, PROB, FUZZY };

        /// <summary>
        /// Format Double number
        /// </summary>
        /// <param name="dbl"></param>
        /// <returns></returns>
        public static string FormatDouble(double dbl)
        {
            return dbl.ToString("F2").Replace(',', '.').Trim('0').Trim('.');
        }

        /// <summary>
        /// List of all root attributes.
        /// </summary>
        private List<Attribute> attributes = null;

        private List<Attribute> basic = null;
        private List<Attribute> aggregate = null;
        private List<Attribute> linked = null;

        /// <summary>
        /// List of all basic attributes (model inputs).
        /// </summary>
        public List<Attribute> Basic { get { return basic; } }

        /// <summary>
        /// List of all aggregate attributes (model outputs).
        /// </summary>
        public List<Attribute> Aggregate { get { return aggregate; } }

        /// <summary>
        /// List of all linked attributes.
        /// </summary>
        public List<Attribute> Linked { get { return linked; } }

        /// <summary>
        /// DEXi advanced setting: Link equal attributes.
        /// </summary>
        public bool Linking { get; set; }

        private void ModelSetup()
        {
            List<Attribute> all = AllAttributes;
            if (Linking)
            {
                LinkAttributes(all);
            }
            basic = new List<Attribute>();
            aggregate = new List<Attribute>();
            linked = new List<Attribute>();
            foreach (Attribute att in all)
            {
                if (att.Link != null)
                    linked.Add(att);
                else if (att.Inputs == 0)
                    basic.Add(att);
                else
                    aggregate.Add(att);
            }
        }

        /// <summary>
        /// Model constructor.
        /// </summary>
        /// <param name="xml">string xml: represents file name or XML string</param>
        /// <param name="fromFile">if true, XML is read from file 'xml', othereise from string 'xml'</param>
        public Model(string xml, bool fromFile)
        {
            if (fromFile)
                ParseXmlFile(xml);
            else
                ParseXmlString(xml);
            ModelSetup();
        }

        /// <summary>
        /// Model
        /// </summary>
        /// <param name="xml">Xml input</param>
        public Model(string xml) : this(xml, true)
        {
        }

        /// <summary>
        /// Return all attributes as string
        /// </summary>
        /// <param name="list">Input attribute list</param>
        /// <returns>List of attributes</returns>
        public string AttributesToString(List<Attribute> list)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < list.Count; i++)
            {
                if (i > 0)
                    sb.Append(";");
                sb.Append(list[i].Name);
            }
            return sb.ToString();
        }

        /// <summary>
        /// Links attributes after loading the model.
        /// </summary>
        /// <param name="list">Input attribute list</param>
        private void LinkAttributes(List<Attribute> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                list[i].LinkAttribute(list);
            }
        }

        /// <summary>
        /// Get all attributes contained in the model.
        /// </summary>
        public List<Attribute> AllAttributes
        {
            get
            {
                List<Attribute> all = new List<Attribute>();
                for (int i = 0; i < attributes.Count; i++)
                {
                    attributes[i].AddAttributes(all);
                }
                return all;
            }
        }

        /// <summary>
        /// Sets all attributes's Values in the list to null.
        /// </summary>
        /// <param name="list">Input attribute list</param>
        protected void ClearAttributeValues(List<Attribute> list)
        {
            foreach (Attribute att in list)
            {
                att.ClearValues();
            }
        }

        /// <summary>
        /// Sets all input attribute Values to full distributions.
        /// </summary>
        public void ClearInputValues()
        {
            foreach (Attribute att in basic)
            {
                if (att.Values == null)
                {
                    att.Values = new Distribution(att.ScaleSize);
                }
                att.Values.Full();
            }
        }

        /// <summary>
        /// Sets all aggregate and linked attribute Values to null.
        /// </summary>
        public void ClearOutputValues()
        {
            ClearAttributeValues(aggregate);
            ClearAttributeValues(linked);
        }

        /// <summary>
        /// Set the values of input attributes.
        /// </summary>
        /// <param name="variables">Variables as string</param>
        public void SetInputValuesByNames(string variables)
        {
            VariableList variableList = new VariableList(variables);
            for (int i = 0; i < variableList.Count; i++)
            {
                Variable var = variableList[i];
                Attribute att = FindAttribute(var.Name, basic);
                if (att == null)
                {
                    throw new ArgumentException("Model.SetInputValuesByNames: Unknown attribute name: " + var.Name);
                }
                string val = var.Value;
                att.SetValue(val);
            }
        }

        /// <summary>
        /// Set the values of input attributes.
        /// </summary>
        /// <param name="values">Array of values</param>
        public void SetInputValues(string[] values)
        {
            if (values.Length != basic.Count)
            {
                throw new ArgumentException(String.Format(
                        "Model.SetInputValues: Argument count is {0}, should be {1}", values.Length, basic.Count));
            }
            for (int i = 0; i < values.Length; i++)
            {
                Attribute att = basic[i];
                att.SetValue(values[i]);
            }
        }

        /// <summary>
        /// Set the values of input attributes.
        /// </summary>
        /// <param name="values">Array of values</param>
        public void SetInputValues(int[] values)
        {
            if (values.Length != basic.Count)
            {
                throw new ArgumentException(String.Format(
                        "Model.SetInputValues: Argument count is {0}, should be {1}", values.Length, basic.Count));
            }
            for (int i = 0; i < values.Length; i++)
            {
                Attribute att = basic[i];
                att.SetValue(values[i]);
            }
        }

        /// <summary>
        /// Set input value of attribute given by name.
        /// </summary>
        /// <param name="name">Name</param>
        /// <param name="value">Value</param>
        /// <param name="mem">Memory</param>
        /// <param name="clear">Clear</param>
        public void SetInputValue(string name, int value, double mem = 1.0, bool clear = true)
        {
            Attribute att = FindAttribute(name, basic);
            if (att == null)
            {
                throw new ArgumentException("Model.SetInputValue: Unknown attribute name: " + name);
            }
            att.SetValue(value, mem, clear);
        }

        /// <summary>
        /// Set input value of attribute given by name.
        /// </summary>
        /// <param name="name">Name</param>
        /// <param name="value">Value</param>
        /// <param name="mem">Memory</param>
        /// <param name="clear">Clear</param>
        public void SetInputValue(string name, string value, double mem = 1.0, bool clear = true)
        {
            Attribute att = FindAttribute(name, basic);
            if (att == null)
            {
                throw new ArgumentException("Model.SetInputValue: Unknown attribute name: " + name);
            }
            att.SetValue(value, mem, clear);
        }

        /// <summary>
        /// Set input value of index-th attribute.
        /// </summary>
        /// <param name="index">Index</param>
        /// <param name="value">Value</param>
        /// <param name="mem">Memory</param>
        /// <param name="clear">Clear</param>
        public void SetInputValue(int index, int value, double mem = 1.0, bool clear = true)
        {
            Attribute att = basic[index];
            att.SetValue(value, mem, clear);
        }

        /// <summary>
        /// Set input value of index-th attribute.
        /// </summary>
        /// <param name="index">Index</param>
        /// <param name="value">Value</param>
        /// <param name="mem">Memory</param>
        /// <param name="clear">Clear</param>
        public void SetInputValue(int index, string value, double mem = 1.0, bool clear = true)
        {
            Attribute att = basic[index];
            att.SetValue(value, mem, clear);
        }

        /// <summary>
        /// Set input value of attribute name to distribution.
        /// </summary>
        /// <param name="name">Name</param>
        /// <param name="distr">Distribution Object</param>
        public void SetInputValue(string name, Distribution distr)
        {
            Attribute att = FindAttribute(name, basic);
            if (att == null)
            {
                throw new ArgumentException("Model.SetInputValue: Unknown attribute name: " + name);
            }
            att.SetDistr(distr);
        }

        /// <summary>
        /// Set input value of attribute name to distribution.
        /// </summary>
        /// <param name="name">Name</param>
        /// <param name="distr">Distribution Object</param>
        public void SetInputValue(string name, double[] distr)
        {
            Attribute att = FindAttribute(name, basic);
            if (att == null)
            {
                throw new ArgumentException("Model.SetInputValue: Unknown attribute name: " + name);
            }
            att.SetDistr(distr);
        }

        /// <summary>
        /// Set input value of index-th attribute name to distribution.
        /// </summary>
        /// <param name="index">Index</param>
        /// <param name="distr">Distribution</param>
        public void SetInputValue(int index, Distribution distr)
        {
            Attribute att = basic[index];
            att.SetDistr(distr);
        }

        /// <summary>
        /// Set input value of index-th attribute name to distribution.
        /// </summary>
        /// <param name="index">Index</param>
        /// <param name="distr">Distribution</param>
        public void SetInputValue(int index, double[] distr)
        {
            Attribute att = basic[index];
            att.SetDistr(distr);
        }

        public string AttributeValues(List<Attribute> list)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < list.Count; i++)
            {
                if (i > 0)
                {
                    sb.Append(";");
                }
                Attribute att = list[i];
                Distribution distr = att.Values;
                sb.Append(att.Name + "=");
                if (distr != null)
                {
                    sb.Append(att.ValuesString);
                }
                else
                {
                    sb.Append("<null>");
                }
            }
            return sb.ToString();
        }

        /// <summary>
        /// Get a name=value[;] string of basic attribute values.
        /// </summary>
        public string InputValues
        {
            get { return AttributeValues(basic); }
        }

        /// <summary>
        /// Get a name=value[;] string of aggregate attribute values.
        /// </summary>
        public string OutputValues
        {
            get { return AttributeValues(aggregate); }
        }

        
        /// <summary>
        /// Get the value distribution of the index-th aggregate (output) attribute.
        /// </summary>
        /// <param name="index">Index</param>
        /// <returns>Distribution</returns>
        public Distribution OutputDistr(int index)
        {
            return aggregate[index].Values;
        }

        /// <summary>
        /// Get the value distribution an aggregate (output) attribute.
        /// </summary>
        /// <param name="name">Attribute name.</param>
        /// <returns>Distribution</returns>
        public Distribution OutputDistr(string name)
        {
            Attribute att = FindAttribute(name, aggregate);
            if (att == null)
            {
                throw new ArgumentException("Model.OutputDistr: Unknown attribute name: " + name);
            }
            return att.Values;
        }

        /// <summary>
        /// Get value distribution of index-th output attribute.
        /// </summary>
        /// <param name="index">Index</param>
        /// <returns>Distribution</returns>
        public string OutputValuesString(int index)
        {
            return aggregate[index].ValuesString;
        }

        /// <summary>
        /// Get value distribution of some output attribute.
        /// </summary>
        /// <param name="name">Attribute name.</param>
        /// <returns>Output as string</returns>
        public string OutputValuesString(string name)
        {
            Attribute att = FindAttribute(name, aggregate);
            if (att == null)
            {
                throw new ArgumentException("Model.OutputValuesString: Unknown attribute name: " + name);
            }
            return att.ValuesString;
        }

        /// <summary>
        /// Evaluate current altenative, represented by basic attributes' Values.
        /// </summary>
        /// <param name="evalType">Type of evaluation.</param>
        /// <param name="normalize">Indicates whether or not distributions are
        /// normalized before (for input attributes) and after (output attributes) evaluation.</param>
        public void Evaluate(Evaluation evalType, bool normalize)
        {
            ClearOutputValues();
            foreach (Attribute att in attributes)
            {
                att.Evaluate(evalType, normalize);
            }
        }

        /// <summary>
        /// Find attribute in the model by name.
        /// </summary>
        /// <param name="name">Attribute name.</param>
        /// <returns>Attribute</returns>
        public Attribute FindAttribute(string name)
        {
            return FindAttribute(name, attributes);
        }

        /// <summary>
        /// Find attribute by name in list.
        /// </summary>
        /// <param name="name">Name of attribute to find</param>
        /// <param name="list">List of all attributes</param>
        /// <returns>Found attribute, otherwise null</returns>
        public Attribute FindAttribute(string name, List<Attribute> list)
        {
            foreach (Attribute att in list)
            {
                if (att.Name.Equals(name))
                {
                    return att;
                }
            }
            return null;
        }

        /// <summary>
        /// Get model explicitness, that is, whether or not all functions are 100% defined.
        /// </summary>
        public bool Explicitness
        {
            get
            {
                bool explicite = true;

                foreach (Attribute att in attributes)
                {
                    explicite = explicite && att.Explicitness;
                }
                return explicite;
            }
        }

        /// <summary>
        /// Get model completeness, that is, whether or not all functions have a required number of rules.
        /// </summary>
        public bool Completeness
        {
            get
            {
                bool complete = true;

                foreach (Attribute att in attributes)
                {
                    complete = complete && att.Completeness;
                }

                return complete;
            }
        }

        /// <summary>
        /// Get a string array of names of attributes in attlist.
        /// </summary>
        /// <param name="list">List of attributes</param>
        /// <returns>List of attribute names</returns>
        public string[] ListAttributes(List<Attribute> list)
        {
            string[] result = new string[list.Count];
            for (int i = 0; i < result.Length; i++)
            {
                result[i] = list[i].Name;
            }
            return result;
        }

        /// <summary>
        /// A string array of input attribute names.
        /// </summary>
        public string[] Inputs
        {
            get { return ListAttributes(basic); }
        }

        /// <summary>
        /// A string array of output attribute names.
        /// </summary>
        public string[] Outputs
        {
            get { return ListAttributes(aggregate); }
        }

        /// <summary>
        /// Get a tab-delimited string of attribute names.
        /// </summary>
        /// <param name="list">List of attributes</param>
        /// <returns>Tab delimited attribute names</returns>
        public string TabbedAttributes(List<Attribute> list)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < list.Count; i++)
            {
                if (i > 0)
                    sb.Append("\t");
                sb.Append(list[i].Name);
            }
            return sb.ToString();
        }

        /// <summary>
        /// Tab-delimited list of all input attribute names.
        /// </summary>
        public string TabbedInputs
        {
            get { return TabbedAttributes(basic); }
        }

        /// <summary>
        /// Tab-delimited list of all input attribute names.
        /// </summary>
        public string TabbedOutputs
        {
            get { return TabbedAttributes(aggregate); }
        }

        protected void ParseDEXi(XmlNode dexi)
        {
            if (!dexi.Name.Equals("DEXi"))
            {
                throw new ArgumentException("Model: Xml root must be 'DEXi'");
            }
            attributes = new List<Attribute>();
            Linking = false;
            foreach (XmlNode node in dexi.ChildNodes)
            {
                if (node.Name.Equals("ATTRIBUTE"))
                {
                    Attribute att = new Attribute(node, 0);
                    attributes.Add(att);
                }
                else if (node.Name.Equals("SETTINGS") && node.HasChildNodes)
                {
                    XmlNodeList settings = node.ChildNodes;
                    foreach (XmlNode setting in settings)
                    {
                        if (setting.Name.Equals("LINKING"))
                        {
                            Linking = setting.FirstChild.Value.Equals("True");
                        }
                    }
                }
            }
        }

        protected void ParseXmlString(string xmlString)
        {
            try
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(xmlString);
                XmlNode node = xmlDoc.DocumentElement.SelectSingleNode("DEXi");
                ParseDEXi(node);
            }
            catch (Exception e)
            {
                throw new ArgumentException("Model.ParseXmlString: Error parsing xml string: " + e.Message);
            }
        }

        protected void ParseXmlFile(string xmlFile)
        {
            try
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(xmlFile);
                XmlNode node = xmlDoc.DocumentElement;
                ParseDEXi(node);
            }
            catch (Exception e)
            {
                throw new ArgumentException("Model.ParseXmlFile: Error parsing XML file " + xmlFile + ": " + e.Message);
            }
        }

        /// <summary>
        /// Write to file
        /// </summary>
        /// <param name="writer">Stream Writer</param>
        public void Write(StreamWriter writer)
        {
            writer.WriteLine("Model:");
            for (int i = 0; i < attributes.Count; i++)
            {
                writer.WriteLine("Attribute[" + i + "]:");
                attributes[i].Write(writer);
            }
        }
    }
}
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member