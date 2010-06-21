using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Rdl.Engine
{
    public class ReportParameter : ReportElement
    {
        private string _name;
        private Type _dataType;
        private bool _nullable = false;
        private DefaultValue _defaultValue = null;
        private bool _allowBlank = true;
        private string _prompt = null;
        private bool _hidden = false;
        private List<ParameterValue> _validValues = new List<ParameterValue>();
        private DataSetReference _validValuesDS = null;
        private bool _multiValue = false;
        private Enums.Auto _usedInQuery = Enums.Auto.False;
        private string[] _value;

        public ReportParameter(XmlNode node, ReportElement parent)
            : base(node, parent)
        {
        }

        protected override void ParseAttribute(XmlNode attr)
        {
            switch (attr.Name.ToLower())
            {
                case "name":
                    _name = attr.InnerText;
                    break;
                case "datatype":
                    switch (attr.InnerText)
                    {
                        case "Boolean" :
                            _dataType = Type.GetType("System.Boolean", true, true);
                            break;
                        case "DateTime" :
                            _dataType = Type.GetType("System.DateTime", true, true);
                            break;
                        case "Integer" :
                            _dataType = Type.GetType("System.Int32", true, true);
                            break;
                        case "Float" :
                            _dataType = Type.GetType("System.Single", true, true);
                            break;
                        case "String" :
                            _dataType = Type.GetType("System.String", true, true);
                            break;
                        default:
                            throw new Exception("Unknown type " + attr.InnerText + " in parameter definition");
                    }
                    break;
                case "nullable":
                    _nullable = bool.Parse(attr.InnerText);
                    break;
                case "defaultvalue":
                    _defaultValue = new DefaultValue(attr, this);
                    break;
                case "allowblank":
                    _allowBlank = bool.Parse(attr.InnerText);
                    break;
                case "prompt":
                    _prompt = attr.InnerText;
                    break;
                case "hidden":
                    _hidden = bool.Parse(attr.InnerText);
                    break;
                case "validvalues":
                    XmlNode child = attr.ChildNodes[0];
                    if (child.Name.ToLower() == "parametervalues")
                    {
                        foreach (XmlNode parm in child.ChildNodes)
                        {
                            _validValues.Add(new ParameterValue(parm, this));
                        }
                    }
                    if (child.Name.ToLower() == "datasetreference")
                    {
                        _validValuesDS = new DataSetReference(child, this);
                    }
                    break;
                case "multivalue":
                    _multiValue = bool.Parse(attr.InnerText);
                    break;
                case "usedinquery":
                    _usedInQuery = (Enums.Auto)Enum.Parse(typeof(Enums.Auto), attr.InnerText, true);
                    break;
                default:
                    break;
            }
        }

        public void LoadDefaultValues(Report rpt)
        {
            if (_defaultValue != null)
            {
                _defaultValue.LoadValues(rpt);
                _value = _defaultValue.GetValues(new Rdl.Runtime.Context(null, null, null, null, null));
            }
        }

        public void LoadValidValues(Report rpt)
        {
            if (_validValuesDS != null)
            {
                DataSet ds = rpt.DataSets[_validValuesDS.DataSetName];
                if (ds == null)
                    throw new Exception("Invalid data set " + _validValuesDS.DataSetName + " in ReportParameter");

                if (ds.Fields[_validValuesDS.ValueField] == null)
                    throw new Exception("Invalid value field " + _validValuesDS.ValueField + " in ReportParameter");
                if (_validValuesDS.LableField != null && ds.Fields[_validValuesDS.LableField] == null)
                    throw new Exception("Invalid label field " + _validValuesDS.LableField + " in ReportParameter");

                System.Data.DataSet dsTemp = new System.Data.DataSet();
                ds.Initialize(dsTemp, new Rdl.Runtime.Context(null, null, null, null, null));
                Rdl.Runtime.Context context = new Rdl.Runtime.Context(null, ds, null, null, null);
                while (context.CurrentRow != null)
                {
                    string value = context.CurrentRow[_validValuesDS.ValueField].ToString();
                    string label = string.Empty;

                    if (_validValuesDS.LableField != null)
                        label = context.CurrentRow[_validValuesDS.LableField].ToString();
                    ParameterValue pv = new ParameterValue(value, label);
                    if (!_validValues.Contains(pv))
                        _validValues.Add(pv);

                    context.MoveNext();
                }
            }
        }

        /// <summary>
        /// Gets the Name of the parameter
        /// </summary>
        public string Name
        {
            get { return _name; }
        }

        /// <summary>
        /// Gets or Sets the current value of the parameter.
        /// </summary>
        public object Value
        {
            get
            {
                object ret = null;
                if (_multiValue)
                    ret = (object)_value;
                else if (_value == null)
                    ret = null;
                else
                {
                    try
                    {
                        ret = (object)Convert.ChangeType(_value[0], _dataType);
                    }
                    catch (Exception)
                    {
                        ret = null;
                    }
                }
                return ret;
            }

            set 
            {
                if (value is string[])
                    _value = (string[])value;
                else if (value is Array)
                {
                    Array v = value as Array;
                    _value = new string[v.Length];
                    for (int i = 0; i < v.Length; i++)
                        _value[i] = v.GetValue(i).ToString();
                }
                else if (value == null)
                    _value = null;
                else
                    _value = new string[] { value.ToString() };
            }
        }

        /// <summary>
        /// If valid values are defined for this report, then this gets the labels associated
        /// with the current values in the valid values list.  Otherwise this returns
        /// the current values
        /// </summary>
        public object Label
        {
            get
            {
                if (_validValues == null)
                    return Value;

                if (_multiValue)
                {
                    List<string> values = new List<string>();
                    foreach (string s in _value)
                        foreach (ParameterValue p in _validValues)
                            if (p.Value == s)
                                values.Add(p.Label);
                    return values.ToArray();
                }
                else
                {
                    foreach (ParameterValue p in _validValues)
                        if (p.Value == _value[0])
                            return p.Label;
                    return Value;
                }
            }
        }

        /// <summary>
        /// If a parameter is multi-value then this returns the count of values in the
        /// parameter.  Otherwise it returns 1.
        /// </summary>
        public int Count
        {
            get
            {
                if (_multiValue)
                    return ((Array)_value).Length;
                return 1;
            }
        }

        /// <summary>
        /// Returns true if the report parameter is set to multi-value
        /// </summary>
        public bool MultiValue
        {
            get { return _multiValue; }
        }

        /// <summary>
        /// Returns the parameter prompt
        /// </summary>
        public string Prompt
        {
            get
            {
                if (_prompt != null)
                    return _prompt;
                else
                    return _name;
            }
        }

        /// <summary>
        /// Returns the parameter data type
        /// </summary>
        public Type DataType
        {
            get { return _dataType; }
        }

        /// <summary>
        /// Returns a list of default values. If the parameter is not multi-value then
        /// this will return a single value array.  If there is no default value defined
        /// in the report for this parameter then this returns null.
        /// </summary>
        public String[] DefaultValue
        {
            get 
            {
                if (_defaultValue == null)
                    return null;
                else
                    return _defaultValue.GetValues(new Rdl.Runtime.Context(null, null, null, null, null));
            }
        }

        /// <summary>
        /// Returns a list of <see cref="ParameterValue"/>.
        /// </summary>
        public List<ParameterValue> ValidValues
        {
            get { return _validValues; }
        }

        public bool Hidden
        {
            get { return _hidden; }
            set { _hidden = value; }
        }

        public bool Nullable
        {
            get { return _nullable; }
        }

        public bool AllowBlank
        {
            get { return _allowBlank; }
        }
    }
}
