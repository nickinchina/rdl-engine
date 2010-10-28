using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Rdl.Engine
{
    class QueryParameter : ReportElement
    {
        private string _name = "";
        private Expression _value = null;

        public QueryParameter(XmlNode node, ReportElement parent)
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
                    _value = new Expression(attr, this, false);
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
