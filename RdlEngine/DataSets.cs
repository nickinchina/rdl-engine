using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace RdlEngine
{
    class DataSets : ReportElement
    {
        private Dictionary<string, DataSet> _dataSets = new Dictionary<string, DataSet>();
        private DataSet _first = null;
        private System.Data.DataSet _ds = new System.Data.DataSet();

        public DataSets(XmlNode attr, ReportElement parent)
            : base(attr, parent)
        {
        }

        protected override void ParseAttribute(XmlNode attr)
        {
            if (attr.Name.ToLower() == "dataset")
            {
                DataSet ds = new DataSet(attr, this);
                _dataSets.Add(ds.Name, ds);
                if (_first == null)
                    _first = ds;
            }
        }

        public void Initialize()
        {
            foreach (DataSet ds in _dataSets.Values)
                ds.Initialize(_ds);

            //// Find parent/child relationships
            //foreach (DataSet ds1 in _dataSets.Values)
            //{
            //    foreach (DataSet ds2 in _dataSets.Values)
            //    {
            //        System.Data.DataTable dt1 = ds1.Table;
            //        System.Data.DataTable dt2 = ds2.Table;

            //        if (dt1 != dt2)
            //        {
            //            System.Data.DataColumn[] dcChild = new System.Data.DataColumn[dt1.PrimaryKey.Length];
            //            System.Data.DataColumn[] dcParent = new System.Data.DataColumn[dt1.PrimaryKey.Length];
            //            bool foundAll = true;
            //            int i=0;
            //            foreach (System.Data.DataColumn dc in dt1.PrimaryKey)
            //            {
            //                if (dt2.Columns[dc.ColumnName] != null)
            //                {
            //                    dcChild[i] = dt2.Columns[dc.ColumnName];
            //                    dcParent[i] = dc;
            //                    i++;
            //                }
            //                else
            //                {
            //                    foundAll = false;
            //                    break;
            //                }
            //            }
            //            if (foundAll && i > 0)
            //            {
            //                try
            //                {
            //                    _ds.Relations.Add(new System.Data.DataRelation(
            //                        ds2.Name + "to" + ds1.Name,
            //                        dcParent, dcChild, true));
            //                }
            //                catch (Exception)
            //                {
            //                }
            //            }
            //        }
            //    }
            //}
        }

        public DataSet this[string key]
        {
            get { return _dataSets[key]; }
        }

        public DataSet FirstDataSet
        {
            get { return _first; }
        }

        public IEnumerator GetEnumerator()
        {
            return _dataSets.Values.GetEnumerator();
        }

        public System.Data.DataSet systemDs
        {
            get { return _ds; }
        }
    }
}
