using System;
using System.Collections.Generic;
using System.Text;

namespace Rdl.Runtime
{
    public partial class RuntimeBase
    {
        public object Sum(AggrFn fn)
        {
            return Sum(fn, null);
        }

        public object Sum(AggrFn fn, string scope)
        {
            return Sum(fn, scope, false);
        }

        public object Sum(AggrFn fn, string scope, bool runningValue)
        {
            Context ctxt = _currentContext;
            if (scope != null && scope != string.Empty)
                ctxt = ctxt.FindContextByGroupName(scope);
            int rowIndex = ctxt.RowIndex;
            bool dec = false;
            decimal decValue = 0;
            double doubValue = 0;

            int top = (runningValue) ? rowIndex : ctxt.Rows.Count - 1;
            for (int i = 0; i <= top; i++)
            {
                ctxt.RowIndex = i;

                object value = fn();
                if (value != null && !Convert.IsDBNull(value))
                {
                    if (value is decimal && !dec)
                    {
                        dec = true;
                        decValue = Convert.ToDecimal(doubValue);
                    }
                    if (dec)
                        decValue += Convert.ToDecimal(value);
                    else
                        doubValue += Convert.ToDouble(value);
                }
            }

            ctxt.RowIndex = rowIndex;
            return (dec) ? (object)decValue : (object)doubValue;
        }

        public object Avg(AggrFn fn)
        {
            return Avg(fn, null);
        }

        public object Avg(AggrFn fn, string scope)
        {
            return Avg(fn, scope, false);
        }

        public object Avg(AggrFn fn, string scope, bool runningValue)
        {
            Context ctxt = _currentContext;
            if (scope != null && scope != string.Empty)
                ctxt = ctxt.FindContextByGroupName(scope);
            int rowIndex = ctxt.RowIndex;
            int ct = 0;
            bool dec = false;
            decimal decValue = 0;
            double doubValue = 0;

            int top = (runningValue) ? rowIndex : ctxt.Rows.Count - 1;
            for (int i = 0; i <= top; i++)
            {
                ctxt.RowIndex = i;

                object value = fn();
                if (value != null && !Convert.IsDBNull(value))
                {
                    if (value is decimal && !dec)
                    {
                        dec = true;
                        decValue = Convert.ToDecimal(doubValue);
                    }
                    if (dec)
                        decValue += Convert.ToDecimal(value);
                    else
                        doubValue += Convert.ToDouble(value);
                    ct++;
                }
            }

            ctxt.RowIndex = rowIndex;
            return (dec) ? (object)(decValue / ct) : (object)(doubValue / ct);
        }

        public object Max(AggrFn fn)
        {
            return Max(fn, null);
        }

        public object Max(AggrFn fn, string scope)
        {
            return Max(fn, scope, false);
        }

        public object Max(AggrFn fn, string scope, bool runningValue)
        {
            Context ctxt = _currentContext;
            if (scope != null && scope != string.Empty)
                ctxt = ctxt.FindContextByGroupName(scope);
            int rowIndex = ctxt.RowIndex;
            object maxVal = null;

            int top = (runningValue) ? rowIndex : ctxt.Rows.Count - 1;
            for (int i = 0; i <= top; i++)
            {
                ctxt.RowIndex = i;

                object value = fn();
                if (value != null && !Convert.IsDBNull(value))
                {
                    if (maxVal == null)
                        maxVal = value;
                    else if (Compare.CompareTo(value, maxVal) > 0)
                        maxVal = value;
                }
            }

            ctxt.RowIndex = rowIndex;
            return maxVal;
        }

        public object Min(AggrFn fn)
        {
            return Min(fn, null);
        }

        public object Min(AggrFn fn, string scope)
        {
            return Min(fn, scope, false);
        }

        public object Min(AggrFn fn, string scope, bool runningValue)
        {
            Context ctxt = _currentContext;
            if (scope != null && scope != string.Empty)
                ctxt = ctxt.FindContextByGroupName(scope);
            int rowIndex = ctxt.RowIndex;
            object maxVal = null;

            int top = (runningValue) ? rowIndex : ctxt.Rows.Count - 1;
            for (int i = 0; i <= top; i++)
            {
                ctxt.RowIndex = i;

                object value = fn();
                if (value != null && !Convert.IsDBNull(value))
                {
                    if (maxVal == null)
                        maxVal = value;
                    else if (Compare.CompareTo(value, maxVal) < 0)
                        maxVal = value;
                }
            }

            ctxt.RowIndex = rowIndex;
            return maxVal;
        }

        public int Count(AggrFn fn)
        {
            return Count(fn, null);
        }

        public int Count(AggrFn fn, string scope)
        {
            return Count(fn, scope, false);
        }

        public int Count(AggrFn fn, string scope, bool runningValue)
        {
            Context ctxt = _currentContext;
            if (scope != null && scope != string.Empty)
                ctxt = ctxt.FindContextByGroupName(scope);
            int rowIndex = ctxt.RowIndex;
            int ct = 0;

            int top = (runningValue) ? rowIndex : ctxt.Rows.Count - 1;
            for (int i = 0; i <= top; i++)
            {
                ctxt.RowIndex = i;

                object value = fn();
                if (value != null && !Convert.IsDBNull(value))
                    ct++;
            }

            ctxt.RowIndex = rowIndex;
            return ct;
        }

        public int CountDistinct(AggrFn fn)
        {
            return CountDistinct(fn, null);
        }

        public int CountDistinct(AggrFn fn, string scope)
        {
            return CountDistinct(fn, scope, false);
        }

        public int CountDistinct(AggrFn fn, string scope, bool runningValue)
        {
            Context ctxt = _currentContext;
            if (scope != null && scope != string.Empty)
                ctxt = ctxt.FindContextByGroupName(scope);
            List<object> valueList = new List<object>();
            int rowIndex = ctxt.RowIndex;

            int top = (runningValue) ? rowIndex : ctxt.Rows.Count - 1;
            for (int i = 0; i <= top; i++)
            {
                ctxt.RowIndex = i;
                object value = fn();
                if (value != null && !Convert.IsDBNull(value))
                    valueList.Add(value);
            }
            valueList.Sort(delegate(object v1, object v2) {return (Compare.CompareTo(v1, v2));});

            int ct = (valueList.Count == 0) ? 0 : 1;
            for (int i = 0; i < valueList.Count - 1; i++)
                if (Compare.CompareTo(valueList[i], valueList[i+1]) != 0)
                    ct++;

            ctxt.RowIndex = rowIndex;

            return ct;
        }

        public int CountRows()
        {
            return CountRows(null);
        }

        public int CountRows(string scope)
        {
            Context ctxt = _currentContext;
            if (scope != null && scope != string.Empty)
                ctxt = ctxt.FindContextByGroupName(scope);
            return ctxt.Rows.Count;
        }

        public Int32 RowNumber()
        {
            return RowNumber(null);
        }

        public Int32 RowNumber(string scope)
        {
            Context ctxt = _currentContext;
            if (scope != null && scope != string.Empty)
                ctxt = ctxt.FindContextByGroupName(scope);

            if (ctxt == null)
                throw new Exception("Unable to find context " + scope + " in RowNumber");

            return ctxt.RowIndex;
        }

        public float StDev(AggrFn fn)
        {
            return StDev(fn, null);
        }

        public float StDev(AggrFn fn, string scope)
        {
            return StDev(fn, scope, false);
        }

        public float StDev(AggrFn fn, string scope, bool runningValue)
        {
            Context ctxt = _currentContext;
            if (scope != null && scope != string.Empty)
                ctxt = ctxt.FindContextByGroupName(scope);
            int rowIndex = ctxt.RowIndex;
            int ct = 0;
            double sum1 = 0;
            double sum2 = 0;

            int top = (runningValue) ? rowIndex : ctxt.Rows.Count - 1;
            for (int i = 0; i <= top; i++)
            {
                ctxt.RowIndex = i;

                object value = fn();
                if (value != null && !Convert.IsDBNull(value))
                {
                    sum1 += Convert.ToDouble(value);
                    sum2 += (Convert.ToDouble(value) * Convert.ToDouble(value));
                    ct++;
                }
            }

            ctxt.RowIndex = rowIndex;
            if (ct > 0)
                return (float)(Math.Sqrt((ct * sum2) - (sum1 * sum1)) / ct);
            else
                return 0;
        }

        public float StDevP(AggrFn fn)
        {
            return StDevP(fn, null);
        }

        public float StDevP(AggrFn fn, string scope)
        {
            return StDevP(fn, scope, false);
        }

        public float StDevP(AggrFn fn, string scope, bool runningValue)
        {
            Context ctxt = _currentContext;
            if (scope != null && scope != string.Empty)
                ctxt = ctxt.FindContextByGroupName(scope);
            int rowIndex = ctxt.RowIndex;
            int ct = 0;
            double sum1 = 0;
            double sum2 = 0;

            int top = (runningValue) ? rowIndex : ctxt.Rows.Count - 1;
            for (int i = 0; i <= top; i++)
            {
                ctxt.RowIndex = i;

                object value = fn();
                if (value != null && !Convert.IsDBNull(value))
                {
                    sum1 += Convert.ToDouble(value);
                    sum2 += (Convert.ToDouble(value) * Convert.ToDouble(value));
                    ct++;
                }
            }

            ctxt.RowIndex = rowIndex;
            if (ct > 1)
                return (float)(Math.Sqrt((ct * sum2) - (sum1 * sum1)) / (ct - 1));
            else
                return 0;
        }

        public object First(AggrFn fn)
        {
            return First(fn, null);
        }

        public object First(AggrFn fn, string scope)
        {
            Context ctxt = _currentContext;
            if (scope != null && scope != string.Empty)
                ctxt = ctxt.FindContextByGroupName(scope);
            int rowIndex = ctxt.RowIndex;

            if (ctxt.Rows.Count > 0)
            {
                ctxt.RowIndex = 0;
                object value = fn();
                ctxt.RowIndex = rowIndex;

                return value;
            }
            else
                return null;
        }

        public object Last(AggrFn fn)
        {
            return Last(fn, null);
        }

        public object Last(AggrFn fn, string scope)
        {
            Context ctxt = _currentContext;
            if (scope != null && scope != string.Empty)
                ctxt = ctxt.FindContextByGroupName(scope);
            int rowIndex = ctxt.RowIndex;

            if (ctxt.Rows.Count > 0)
            {
                ctxt.RowIndex = ctxt.Rows.Count-1;
                object value = fn();
                ctxt.RowIndex = rowIndex;

                return value;
            }
            else
                return null;
        }

        public object Previous(AggrFn fn)
        {
            return Previous(fn, null);
        }

        public object Previous(AggrFn fn, string scope)
        {
            Context ctxt = _currentContext;
            if (scope != null && scope != string.Empty)
                ctxt = ctxt.FindContextByGroupName(scope);
            int rowIndex = ctxt.RowIndex;

            if (ctxt.RowIndex > 0)
            {
                ctxt.RowIndex--; ;
                object value = fn();
                ctxt.RowIndex = rowIndex;

                return value;
            }
            else
                return null;
        }

        public object RunningValue(AggrFn fn, string function)
        {
            return RunningValue(fn, null);
        }

        public object RunningValue(AggrFn fn, string function, string scope)
        {
            switch (function.ToLower())
            {
                case "sum":
                    return Sum(fn, scope, true);
                case "avg":
                    return Avg(fn, scope, true);
                case "max":
                    return Max(fn, scope, true);
                case "min":
                    return Min(fn, scope, true);
                case "count":
                    return Count(fn, scope, true);
                case "countdistinct":
                    return CountDistinct(fn, scope, true);
                case "stdev":
                    return StDev(fn, scope, true);
                case "stdevp":
                    return StDevP(fn, scope, true);
                default:
                    throw new Exception("Unknown function " + function + " in RunningValue");
            }
        }

    }
}
