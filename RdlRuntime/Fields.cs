using System;
using System.Collections.Generic;
using System.Text;

namespace RdlRuntime
{
    public class Fields
    {
        private RuntimeBase _rtb;

        public Fields(RuntimeBase rtb)
        {
            _rtb = rtb;
        }

        public object this[string key]
        {
            get 
            {
                return new Field(_rtb, key);
            }
        }
    }

    public class Field
    {
        private string _key;
        private RuntimeBase _rtb;

        public Field(RuntimeBase rtb, string key)
        {
            _key = key;
            _rtb = rtb;
        }

        public object Value
        {
            get { return _rtb.CurrentContext.DataSet.Fields[_key].GetValue(_rtb.CurrentContext); }
        }

        public bool IsMissing
        {
            get { return _rtb.CurrentContext.DataSet.Fields[_key].IsMissing(_rtb.CurrentContext); }
        }
    }
}
