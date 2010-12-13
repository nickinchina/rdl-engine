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
    public class BorderColor : ReportElement
    {
        private Expression _default = null;
        private Expression _left = null;
        private Expression _right = null;
        private Expression _top = null;
        private Expression _bottom = null;

        public BorderColor(XmlNode node, ReportElement parent)
            : base(node, parent)
        {
        }

        protected override void ParseAttribute(XmlNode attr)
        {
            switch (attr.Name.ToLower())
            {
                case "default":
                    _default = new Expression(attr, this);
                    break;
                case "left":
                    _left = new Expression(attr, this);
                    break;
                case "right":
                    _right = new Expression(attr, this);
                    break;
                case "top":
                    _top = new Expression(attr, this);
                    break;
                case "bottom":
                    _bottom = new Expression(attr, this);
                    break;
                default:
                    break;
            }
        }

        public string Default(Rdl.Runtime.Context context)
        {
            return (_default == null) ? "black" : _default.ExecAsString(context);
        }

        public string Left(Rdl.Runtime.Context context)
        {
            return (_left == null) ? Default(context) : _left.ExecAsString(context);
        }

        public string Right(Rdl.Runtime.Context context)
        {
            return (_right == null) ? Default(context) : _right.ExecAsString(context);
        }

        public string Top(Rdl.Runtime.Context context)
        {
            return (_top == null) ? Default(context) : _top.ExecAsString(context);
        }

        public string Bottom(Rdl.Runtime.Context context)
        {
            return (_bottom == null) ? Default(context) : _bottom.ExecAsString(context);
        }
    }
}
