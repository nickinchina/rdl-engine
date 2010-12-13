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
            PushContextState();
            try
            {
                if (scope != null && scope != string.Empty)
                    ctxt = ctxt.FindContextByGroupName(scope, _rpt);
                bool dec = false;
                decimal decValue = 0;
                double doubValue = 0;

                int top = (runningValue) ? ctxt.RowIndex : ctxt.Rows.Count - 1;
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

                return (dec) ? (object)decValue : (object)doubValue;
            }
            finally
            {
                PopContextState();
            }
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
            PushContextState();
            try
            {
                if (scope != null && scope != string.Empty)
                    ctxt = ctxt.FindContextByGroupName(scope, _rpt);
                int ct = 0;
                bool dec = false;
                decimal decValue = 0;
                double doubValue = 0;

                int top = (runningValue) ? ctxt.RowIndex : ctxt.Rows.Count - 1;
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

                return (dec) ? (object)(decValue / ct) : (object)(doubValue / ct);
            }
            finally
            {
                PopContextState();
            }
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
            PushContextState();
            try
            {
                if (scope != null && scope != string.Empty)
                    ctxt = ctxt.FindContextByGroupName(scope, _rpt);
                object maxVal = null;

                int top = (runningValue) ? ctxt.RowIndex : ctxt.Rows.Count - 1;
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

                return maxVal;
            }
            finally
            {
                PopContextState();
            }
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
            PushContextState();
            Context ctxt = _currentContext;
            try
            {
                if (scope != null && scope != string.Empty)
                    ctxt = ctxt.FindContextByGroupName(scope, _rpt);
                object maxVal = null;

                int top = (runningValue) ? ctxt.RowIndex : ctxt.Rows.Count - 1;
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

                return maxVal;
            }
            finally
            {
                PopContextState();
            }
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
            PushContextState();
            try
            {
                if (scope != null && scope != string.Empty)
                    ctxt = ctxt.FindContextByGroupName(scope, _rpt);
                int ct = 0;

                int top = (runningValue) ? ctxt.RowIndex : ctxt.Rows.Count - 1;
                for (int i = 0; i <= top; i++)
                {
                    ctxt.RowIndex = i;

                    object value = fn();
                    if (value != null && !Convert.IsDBNull(value))
                        ct++;
                }

                return ct;
            }
            finally
            {
                PopContextState();
            }
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
            PushContextState();
            try
            {
                if (scope != null && scope != string.Empty)
                    ctxt = ctxt.FindContextByGroupName(scope, _rpt);
                List<object> valueList = new List<object>();

                int top = (runningValue) ? ctxt.RowIndex : ctxt.Rows.Count - 1;
                for (int i = 0; i <= top; i++)
                {
                    ctxt.RowIndex = i;
                    object value = fn();
                    if (value != null && !Convert.IsDBNull(value))
                        valueList.Add(value);
                }
                valueList.Sort(delegate(object v1, object v2) { return (Compare.CompareTo(v1, v2)); });

                int ct = (valueList.Count == 0) ? 0 : 1;
                for (int i = 0; i < valueList.Count - 1; i++)
                    if (Compare.CompareTo(valueList[i], valueList[i + 1]) != 0)
                        ct++;

                return ct;
            }
            finally
            {
                PopContextState();
            }
        }

        public int CountRows()
        {
            return CountRows(null);
        }

        public int CountRows(string scope)
        {
            Context ctxt = _currentContext;
            if (scope != null && scope != string.Empty)
                ctxt = ctxt.FindContextByGroupName(scope, _rpt);
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
                ctxt = ctxt.FindContextByGroupName(scope, _rpt);

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
            PushContextState();
            try
            {
                if (scope != null && scope != string.Empty)
                    ctxt = ctxt.FindContextByGroupName(scope, _rpt);
                int ct = 0;
                double sum1 = 0;
                double sum2 = 0;

                int top = (runningValue) ? ctxt.RowIndex : ctxt.Rows.Count - 1;
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

                if (ct > 0)
                    return (float)(Math.Sqrt((ct * sum2) - (sum1 * sum1)) / ct);
                else
                    return 0;
            }
            finally
            {
                PopContextState();
            }
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
            PushContextState();
            try
            {
                if (scope != null && scope != string.Empty)
                    ctxt = ctxt.FindContextByGroupName(scope, _rpt);
                int ct = 0;
                double sum1 = 0;
                double sum2 = 0;

                int top = (runningValue) ? ctxt.RowIndex : ctxt.Rows.Count - 1;
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

                if (ct > 1)
                    return (float)(Math.Sqrt((ct * sum2) - (sum1 * sum1)) / (ct - 1));
                else
                    return 0;
            }
            finally
            {
                PopContextState();
            }
        }

        public object First(AggrFn fn)
        {
            return First(fn, null);
        }

        public object First(AggrFn fn, string scope)
        {
            try
            {
                PushContextState();
                if (scope != null && scope != string.Empty)
                    _currentContext = _currentContext.FindContextByGroupName(scope, _rpt);

                if (_currentContext.Rows.Count > 0)
                {
                    _currentContext.RowIndex = 0;
                    object value = fn();

                    return value;
                }
                else
                    return null;
            }
            finally
            {
                PopContextState();
            }
        }

        public object Last(AggrFn fn)
        {
            return Last(fn, null);
        }

        public object Last(AggrFn fn, string scope)
        {
            Context ctxt = _currentContext;
            PushContextState();
            try
            {
                if (scope != null && scope != string.Empty)
                    ctxt = ctxt.FindContextByGroupName(scope, _rpt);

                if (ctxt.Rows.Count > 0)
                {
                    ctxt.RowIndex = ctxt.Rows.Count - 1;
                    object value = fn();

                    return value;
                }
                else
                    return null;
            }
            finally
            {
                PopContextState();
            }
        }

        public object Previous(AggrFn fn)
        {
            return Previous(fn, null);
        }

        public object Previous(AggrFn fn, string scope)
        {
            Context ctxt = _currentContext;
            PushContextState();
            try
            {
                if (scope != null && scope != string.Empty)
                    ctxt = ctxt.FindContextByGroupName(scope, _rpt);

                if (ctxt.RowIndex > 0)
                {
                    ctxt.RowIndex--; ;
                    object value = fn();

                    return value;
                }
                else
                    return null;
            }
            finally
            {
                PopContextState();
            }
        }

        public enum RunningValueFunction
        {
            Sum = 1, Avg, Max, Min, Count, CountDistinct, StDev, StDevP
        };

        public object RunningValue(AggrFn fn, RunningValueFunction function)
        {
            return RunningValue(fn, function);
        }

        public object RunningValue(AggrFn fn, RunningValueFunction function, string scope)
        {
            switch (function)
            {
                case RunningValueFunction.Sum:
                    return Sum(fn, scope, true);
                case RunningValueFunction.Avg:
                    return Avg(fn, scope, true);
                case RunningValueFunction.Max:
                    return Max(fn, scope, true);
                case RunningValueFunction.Min:
                    return Min(fn, scope, true);
                case RunningValueFunction.Count:
                    return Count(fn, scope, true);
                case RunningValueFunction.CountDistinct:
                    return CountDistinct(fn, scope, true);
                case RunningValueFunction.StDev:
                    return StDev(fn, scope, true);
                case RunningValueFunction.StDevP:
                    return StDevP(fn, scope, true);
                default:
                    throw new Exception("Unknown function " + function + " in RunningValue");
            }
        }

    }
}
