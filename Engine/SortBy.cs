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
