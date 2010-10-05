using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace RdlEngine
{
    public class ReportParameter : ReportElement
    {
        private string _name;
        private Type _dataType;
        private bool _nullable = true;
        private DefaultValue _defaultValue = null;
        private bool _allowBlank = true;
        private string _prompt = "";
        private bool _hidden = false;
        private List<ParameterValue> _validValues = new List<ParameterValue>();
        private DataSetReference _validValuesDS = null;
        private bool _multiValue = false;
        private Enums.Auto _usedInQuery = Enums.Auto.False;
        private object _value;

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
                    _dataType = Type.GetType(attr.InnerText);
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
                    if (child.Name.ToLower() == "DataSetReference")
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

        public string Name
        {
            get { return _name; }
        }

        public object Value
        {
            get { return _value; }
            set { _value = value; }
        }
    }
}
