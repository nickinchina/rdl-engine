using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Rdl.Engine
{
    class Parameter : ReportElement
    {
        private string _name = string.Empty;
        private Expression _value = null;
        private Expression _omit = null;

        public Parameter(XmlNode node, ReportElement parent)
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
                case "value":
                    _value = new Expression(attr, this);
                    break;
                case "omit":
                    _omit = new Expression(attr, this);
                    break;
                default:
                    break;
            }
        }

        public string Name
        {
            get { return _name; }
        }

        public object Value(Rdl.Runtime.Context context)
        {
            return _value.Exec(context);
        }
    }
}
