using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Rdl.Engine
{
    class DefaultValue : ReportElement
    {
        private DataSetReference _dataSetReference = null;
        private List<Expression> _values = null;

        public DefaultValue(XmlNode node, ReportElement parent)
            : base(node, parent)
        {
        }

        protected override void ParseAttribute(XmlNode attr)
        {
            switch (attr.Name.ToLower())
            {
                case "datasetreference":
                    _dataSetReference = new DataSetReference(attr, this);
                    break;
                case "values":
                    _values = new List<Expression>();
                    foreach (XmlNode child in attr.ChildNodes)
                        _values.Add(new Expression(child, this, false));
                    break;
                default:
                    break;
            }
        }

        internal void LoadValues(Report rpt)
        {
            if (_dataSetReference != null)
            {
                DataSet ds = rpt.DataSets[_dataSetReference.DataSetName];
                if (ds == null)
                    throw new Exception("Invalid data set " + _dataSetReference.DataSetName + " in DefaultValue");

                if (ds.Fields[_dataSetReference.ValueField] == null)
                    throw new Exception("Invalid value field " + _dataSetReference.ValueField + " in DefaultValue");
                if (ds.Fields[_dataSetReference.LableField] == null)
                    throw new Exception("Invalid label field " + _dataSetReference.LableField + " in DefaultValue");

                System.Data.DataSet dsTemp = new System.Data.DataSet();
                ds.Initialize(dsTemp);
                Rdl.Runtime.Context context = new Rdl.Runtime.Context(null, ds, null, null, null);
                while (context.CurrentRow != null)
                {
                    _values.Add(new Expression(
                        context.CurrentRow[_dataSetReference.ValueField].ToString(), this, false));

                    context.MoveNext();
                }
            }
        }

        public string[] GetValues(Rdl.Runtime.Context context)
        {
            string[] ret = new string[_values.Count];

            for(int i=0; i < _values.Count; i++)
                ret[i] = _values[i].ExecAsString(context);

            return ret;
        }
    }
}
