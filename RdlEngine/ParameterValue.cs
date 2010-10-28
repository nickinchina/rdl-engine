using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace RdlEngine
{
    class ParameterValue : ReportElement
    {
        private Expression _value = null;
        private Expression _label = null;

        public ParameterValue(XmlNode node, ReportElement parent)
            : base(node, parent)
        {
        }

        protected override void ParseAttribute(XmlNode attr)
        {
            switch (attr.Name.ToLower())
            {
                case "Value":
                    _value = new Expression(attr, this);
                    break;
                case "Label":
                    _label = new Expression(attr, this);
                    break;
                default:
                    break;
            }
        }

    }
}
