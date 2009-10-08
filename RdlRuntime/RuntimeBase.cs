using System;
using System.Collections.Generic;
using System.Text;
using RdlEngine;

public delegate object RdlExpressionDeletage();

namespace RdlRuntime
{
    public class RuntimeBase
    {
        // 
        protected RdlEngine.Report _rpt;
        // The list of Field elements.  This is set in Report.Render
        protected Fields Fields;
        // The list of ReportParameters elements.  This is set in Report.Render
        protected Parameters Parameters;
        // The list of named expressions.  This is built in Report.Render
        private List<RdlExpressionDeletage> _expressionList = new List<RdlExpressionDeletage>();
        // At every expression the current context is set to the 
        // current DataSet Filters so the Fields can be
        // resolved appropriately.
        private Context _currentContext;

        public delegate object AggrFn();

        public RuntimeBase(RdlEngine.Report rpt)
        {
            Fields = new Fields(this);
            Parameters = new Parameters(_rpt);
        }

        public void AddFunction(RdlExpressionDeletage expression)
        {
            _expressionList.Add(expression);
        }

        internal Context CurrentContext
        {
            get { return _currentContext; }
        }

        internal object Exec(Int32 key, Context ctxt)
        {
            _currentContext = ctxt;
            return _expressionList[key]();
        }

        internal string ExecAsString(Int32 key, Context ctxt)
        {
            _currentContext = ctxt;
            object val = _expressionList[key]();
            if (Convert.IsDBNull(val))
                return null;
            else
                return val.ToString();
        }

        internal string ExecAsString(Int32 key, Context ctxt, string format)
        {
            _currentContext = ctxt;
            object val = _expressionList[key]();
            if (Convert.IsDBNull(val))
                return null;
            else if (format == null || format == string.Empty)
                return val.ToString();
            else
                return String.Format("{0:" + format + "}", val);
        }

        internal bool ExecAsBoolean(Int32 key, Context ctxt)
        {
            _currentContext = ctxt;
            return (bool)_expressionList[key]();
        }

        internal Int32 ExecAsInt(Int32 key, Context ctxt)
        {
            _currentContext = ctxt;
            return (Int32)_expressionList[key]();
        }

        public object sum(AggrFn fn)
        {
            int rowIndex = _currentContext.RowIndex;
            bool doub = false;
            decimal decValue = 0;
            double doubValue = 0;

            for (int i = 0; i < _currentContext.Rows.Count; i++ )
            {
                _currentContext.RowIndex = i;

                object value = fn();
                if (value is System.Single || value is System.Double && !doub)
                {
                    doub = true;
                    doubValue = (double)decValue;
                }
                if (doub)
                    doubValue += (double)value;
                else
                    decValue += Convert.ToDecimal(value);
            }

            _currentContext.RowIndex = rowIndex;
            return (doub) ? (object)doubValue : (object)decValue;
        }

        public object Count(AggrFn fn)
        {
            return _currentContext.Rows.Count;
        }

        public object CountDistinct(AggrFn fn)
        {
            List<object> valueList = new List<object>();
            int rowIndex = _currentContext.RowIndex;

            for (int i = 0; i < _currentContext.Rows.Count; i++)
            {
                _currentContext.RowIndex = i;
                valueList.Add(fn());
            }
            valueList.Sort(RdlEngine.Utility.ApplyCompare);

            int ct = (valueList.Count == 0)?0:1;
            for (int i = 0; i < valueList.Count - 1; i++)
                if (RdlEngine.Utility.ApplyCompare(valueList[i], valueList[i + 1]) != 0)
                    ct++;

            return ct;
        }
    }
}
