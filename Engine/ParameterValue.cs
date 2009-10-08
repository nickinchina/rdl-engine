using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Rdl.Engine
{
    public class ParameterValue : ReportElement
    {
        private Expression _value = null;
        private Expression _label = null;

        public ParameterValue(XmlNode node, ReportElement parent)
            : base(node, parent)
        {
        }

        public ParameterValue(string value, string label) : base(null, null)
        {
            _value = new Expression(value, this);
            _label = new Expression(label, this);
        }

        protected override void ParseAttribute(XmlNode attr)
        {
            switch (attr.Name.ToLower())
            {
                case "value":
                    _value = new Expression(attr, this);
                    break;
                case "label":
                    _label = new Expression(attr, this);
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Gets the value for this default value.
        /// </summary>
        public string Value
        {
            get
            {
                if (_value == null)
                    return null;
                else
                    return _value.ExecAsString(new Rdl.Runtime.Context(null, null, null, null, null));
            }
        }

        /// <summary>
        /// Gets the label for this default value.
        /// </summary>
        public string Label
        {
            get
            {
                if (_label == null)
                    return Value;
                else
                    return _label.ExecAsString(new Rdl.Runtime.Context(null, null, null, null, null));
            }
        }
    }
}
