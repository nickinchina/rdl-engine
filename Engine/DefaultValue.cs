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
                ds.Initialize(dsTemp, new Rdl.Runtime.Context(null, null, null, null, null));
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
