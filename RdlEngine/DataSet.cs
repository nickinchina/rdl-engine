using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace RdlEngine
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
            RdlRuntime.InitializeDataSetEventArgs args = new RdlRuntime.InitializeDataSetEventArgs();
            args.DataSetName = _name;

            Report.OnInitializeDataSet(args);
            if (args.dt != null)
                _table = args.dt;
            else
            {
                _query.Exec(ds, _name);
                _table = ds.Tables[_name];
            }
        }

        public string Name
        {
            get { return _name; }
        }

        public Dictionary<string, Field> Fields
        {
            get { return _fields; }
        }

        public System.Data.DataTable Table
        {
            get { return _table; }
        }

        public Int32 Rows
        {
            get { return _table.Rows.Count; }
        }
    }
}
