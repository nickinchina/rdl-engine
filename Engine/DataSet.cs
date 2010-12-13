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
    class DataSet : ReportElement
    {
        private string _name;
        private Dictionary<string, Field> _fields = new Dictionary<string, Field>();
        private Query _query = null;
        private Enums.Auto _caseSensitivity = Enums.Auto.False;
        private string _colation = null;
        private Enums.Auto _accentSensitivity = Enums.Auto.False;
        private Enums.Auto _kanatypeSensitivity = Enums.Auto.False;
        private Enums.Auto _widthSensitivity = Enums.Auto.False;
        private Filters _filters = null;
        private System.Data.DataSet _ds = null;

        private System.Data.DataTable _table = null;

        public DataSet(XmlNode node, ReportElement parent)
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
                case "fields":
                    foreach (XmlNode child in attr.ChildNodes)
                    {
                        Field fld = new Field(child, this);
                        _fields.Add(fld.Name, fld);
                    }
                    break;
                case "query":
                    _query = new Query(attr, this);
                    break;
                case "casesensitivity":
                    _caseSensitivity = (Enums.Auto)Enum.Parse(typeof(Enums.Auto), attr.InnerText,true);
                    break;
                case "colation":
                    _colation = attr.InnerText;
                    break;
                case "accentsensitivity":
                    _accentSensitivity = (Enums.Auto)Enum.Parse(typeof(Enums.Auto), attr.InnerText, true);
                    break;
                case "kanatypesensitivity":
                    _kanatypeSensitivity = (Enums.Auto)Enum.Parse(typeof(Enums.Auto), attr.InnerText, true);
                    break;
                case "widthsensitivity":
                    _widthSensitivity = (Enums.Auto)Enum.Parse(typeof(Enums.Auto), attr.InnerText, true);
                    break;
                case "filters":
                    _filters = new Filters(attr, this);
                    break;
                default:
                    break;
            }
        }

        public void Initialize(System.Data.DataSet ds)
        {
            _ds = ds;
        }

        private void Load(Rdl.Runtime.Context context)
        {
            Rdl.Runtime.InitializeDataSetEventArgs args = new Rdl.Runtime.InitializeDataSetEventArgs();
            args.DataSetName = _name;
            Report.OnInitializeDataSet(args);
            if (args.dt != null)
                _table = args.dt;
            else
            {
                // Clear the table prior to populating it.
                if (_ds.Tables.Contains(_name))
                    _ds.Tables[_name].Clear();
                _query.Exec(_ds, _name, context);
                _table = _ds.Tables[_name];
            }
        }

        public void Reset()
        {
            _table = null;
        }

        public string Name
        {
            get { return _name; }
        }

        public Dictionary<string, Field> Fields
        {
            get { return _fields; }
        }

        public System.Data.DataTable GetTable(Rdl.Runtime.Context context)
        {
            if (_table == null)
                Load(context);
            return _table;
        }

        public Int32 Rows
        {
            get { return _table.Rows.Count; }
        }
    }
}
