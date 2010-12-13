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
    public class Visibility : ReportElement
    {
        private Expression _hidden = null;
        private string _toggleItem = null;
        public static Visibility Visible = new Visibility("false");

        public Visibility(XmlNode node, ReportElement parent)
            : base(node, parent)
        {
        }

        private Visibility( string hidden ) 
            : base( null, null)
        {
            _hidden = new Expression(hidden, null);
        }

        protected override void ParseAttribute(XmlNode attr)
        {
            switch (attr.Name.ToLower())
            {
                case "hidden":
                    _hidden = new Expression(attr, this);
                    break;
                case "toggleitem":
                    _toggleItem = attr.InnerText;
                    break;
                default:
                    break;
            }
        }

        public bool IsHidden(Rdl.Runtime.Context context)
        {
            return _hidden.ExecAsBoolean(context);
        }

        public string ToggleItem
        {
            get { return _toggleItem; }
        }
    }
}
