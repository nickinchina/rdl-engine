using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Rdl.Engine
{
    public class Action : ReportElement
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
                case "drillthrough":
                    _drillthrough = new Drillthrough(attr, this);
                    break;
                case "bookmarklink":
                    _bookmarkLink = new Expression(attr, this);
                    break;
                default:
                    break;
            }
        }

        internal override void Render(Rdl.Render.Container box, Rdl.Runtime.Context context)
        {
            // Find the ActionElement
            Rdl.Render.ActionElement ae = null;

            foreach(Rdl.Render.Element e in box.Children)
                if (e is Rdl.Render.ActionElement)
                    ae = (Rdl.Render.ActionElement)e;

            if (ae == null)
                throw new Exception("An action is defined where no textbox or image is contained");

            if (_hyperlink != null)
                ae.Hyperlink = _hyperlink.ExecAsString(context);
            if (_bookmarkLink != null)
                ae.BookmarkLink = _bookmarkLink.ExecAsString(context);
            if (_drillthrough != null)
            {
                ae.DrillThroughReportName = _drillthrough.ReportName;
                foreach (Parameter parm in _drillthrough.Parameters)
                    ae.DrillThroughParameterList.Add(
                        new Rdl.Render.ActionElement.ActionParameter(parm.Name,
                            parm.Value(context)));
            }
        }
    }
}
