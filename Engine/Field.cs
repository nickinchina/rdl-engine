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
            return !context.DataSet.GetTable(context).Columns.Contains(_dataField);
        }
    }
}
