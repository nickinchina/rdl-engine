using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace RdlEngine
{
    class SortBy : ReportElement
    {
        public enum SortDirection
        {
            Ascending,
            Descending
        };

        private Expression _sortExpression = null;
        private SortDirection _direction = SortDirection.Ascending;

        public SortBy(XmlNode node, ReportElement parent)
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
                case "direction":
                    _direction = (SortDirection)Enum.Parse(typeof(SortDirection), attr.InnerText,true);
                    break;
                default:
                    break;
            }
        }

        public Expression Expression
        {
            get { return _sortExpression; }
        }

        public SortDirection Direction
        {
            get { return _direction; }
        }
    }
}
