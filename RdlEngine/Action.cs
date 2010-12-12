using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace RdlEngine
{
    class Action : ReportElement
    {
        private Expression _hyperlink = null;
        private Drillthrough _drillthrough = null;
        private Expression _bookmarkLink = null;

        public Action(XmlNode node, ReportElement parent) : base( node, parent )
        {
        }

        protected override void ParseAttribute(XmlNode attr)
        {
            switch (attr.Name.ToLower())
            {
                case "hyperlink":
                    _hyperlink = new Expression(attr, this);
                    break;
                case "drillghrough":
                    _drillthrough = new Drillthrough(attr, this);
                    break;
                case "bookmarklink":
                    _bookmarkLink = new Expression(attr, this);
                    break;
                default:
                    break;
            }
        }
    }
}
