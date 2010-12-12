using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace RdlEngine
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
                    _value = new Expression(attr, this);
                    break;
                default:
                    break;
            }
        }

        public string Name
        {
            get { return _name; }
        }

        public object value
        {
            get { return _value.Exec(); }
        }
    }
}
