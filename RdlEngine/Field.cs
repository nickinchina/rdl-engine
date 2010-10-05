using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace RdlEngine
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

        internal object GetValue(RdlRuntime.Context context)
        {
            if (_dataField != null)
            {
                return context.CurrentRow[_dataField];
            }
            else if (_value != null)
                return _value.Exec(context);
            return null;
        }

        internal bool IsMissing(RdlRuntime.Context context)
        {
            return !context.DataSet.Table.Columns.Contains(_dataField);
        }
    }
}
