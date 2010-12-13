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
    public class Field : ReportElement
    {
        private string _name;
        private string _dataField = null;
        private Expression _value = null;

        public Field(XmlNode node, ReportElement parent)
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
                case "datafield":
                    _dataField = attr.InnerText;
                    break;
                case "value":
                    _value = new Expression(attr, this);
                    break;
                default:
                    break;
            }
        }

        public string Name
        {
            get { return _name; }
        }

        static System.Diagnostics.Stopwatch _stopwatch1 = new System.Diagnostics.Stopwatch();
        internal object GetValue(Rdl.Runtime.Context context)
        {
            if (_dataField != null)
            {
                if (context.CurrentRow == null)
                    return null;
                object val = context.CurrentRow[_dataField];
                return val;
            }
            else if (_value != null)
                return _value.Exec(context);
            return null;
        }

        internal bool IsMissing(Rdl.Runtime.Context context)
        {
            return !context.DataSet.Table.Columns.Contains(_dataField);
        }
    }
}
