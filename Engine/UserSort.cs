using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Rdl.Engine
{
    class UserSort : ReportElement
    {
        private Expression _sortExpression = null;
        private string _sortExpressionScope = null;
        private string _sortTarget = null;

        public UserSort(XmlNode node, ReportElement parent)
            : base(node, parent)
        {
        }

        protected override void ParseAttribute(XmlNode attr)
        {
            switch (attr.Name.ToLower())
            {
                case "sortexpression":
                    _sortExpression = new Expression(attr, this);
                    break;
                case "sortexpressionscope":
                    _sortExpressionScope = attr.InnerText;
                    break;
                case "sorttarget":
                    _sortTarget = attr.InnerText;
                    break;
                default:
                    break;
            }
        }
    }
}
