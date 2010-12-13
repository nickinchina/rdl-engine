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
    class Grouping : ReportElement
    {
        private string _name = string.Empty;
        private Expression _label = null;
        private List<Expression> _groupExpressions = new List<Expression>();
        private bool _pageBreakAtStart = false;
        private bool _pageBreakAtEnd = false;
        private Filters _filters = null;
        private Expression _parentGroup = null;
        private string _dataElementName = string.Empty;
        private string _dataCollectionName = string.Empty;
        private Enums.DataElementOutputEnum _dataElementOutput = Enums.DataElementOutputEnum.Output;

        public Grouping(XmlNode node, ReportElement parent)
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
                case "label":
                    _label = new Expression(attr, this);
                    break;
                case "groupexpressions":
                    foreach (XmlNode child in attr.ChildNodes)
                        _groupExpressions.Add(new Expression(child, this));
                    break;
                case "pagebreakatstart":
                    _pageBreakAtStart = bool.Parse(attr.InnerText);
                    break;
                case "pagebreakatend":
                    _pageBreakAtEnd = bool.Parse(attr.InnerText);
                    break;
                case "filters":
                    _filters = new Filters(attr, this);
                    break;
                case "parent":
                    _parentGroup = new Expression(attr, this); ;
                    break;
                case "dataelementname":
                    _dataElementName = attr.InnerText;
                    break;
                case "datacollectionname":
                    _dataCollectionName = attr.InnerText;
                    break;
                case "dataelementoutput":
                    _dataElementOutput = (Enums.DataElementOutputEnum)Enum.Parse(typeof(Enums.DataElementOutputEnum), attr.InnerText,true);
                    break;
                default:
                    break;
            }
        }

        internal string Name
        {
            get { return _name; }
        }

        internal string Label(Rdl.Runtime.Context context)
        {
            return _label.ExecAsString(context);
        }

        internal List<Expression> GroupExpressions
        {
            get { return _groupExpressions; }
        }

        internal bool PageBreakAtStart
        {
            get { return _pageBreakAtStart; }
        }

        internal bool PageBreakAtEnd
        {
            get { return _pageBreakAtEnd; }
        }

        internal Filters Filters
        {
            get { return _filters; }
        }
    }
}
