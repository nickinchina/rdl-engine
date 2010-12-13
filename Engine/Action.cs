/*-----------------------------------------------------------------------------------
This file is part of the SawikiSoft RDL Engine.
The SawikiSoft RDL Engine is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

The SawikiSoft RDL Engine is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with Foobar.  If not, see <http://www.gnu.org/licenses/>.
-----------------------------------------------------------------------------------*/
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
