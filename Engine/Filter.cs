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
using System.Text.RegularExpressions;
using System.Globalization;

namespace Rdl.Engine
{
    class Filter : ReportElement
    {
        public enum FilterOperator
        {
            Equal,
            Like,
            NotEqual,
            GreaterThan,
            GreaterThanOrEqual,
            LessThan,
            LessThanOrEqual,
            TopN,
            BottomN,
            TopPercent,
            BottomPercent,
            In,
            Between
        }

        private Expression _filterExpression = null;
        private FilterOperator _operator = FilterOperator.Equal;
        private List<Expression> _filterValues = new List<Expression>();
        private object _groupValue = null;

        public Filter(XmlNode node, ReportElement parent)
            : base(node, parent)
        {
        }

        protected override void ParseAttribute(XmlNode attr)
        {
            switch (attr.Name.ToLower())
            {
                case "filterexpression":
                    _filterExpression = new Expression(attr, this);
                    break;
                case "operator":
                    _operator = (FilterOperator)Enum.Parse(typeof(FilterOperator), attr.InnerText, true);
                    break;
                case "filtervalues":
                    foreach (XmlNode child in attr.ChildNodes)
                        _filterValues.Add(new Expression(child, this));
                    break;
                default:
                    break;
            }
        }

        public Expression FilterExpression
        {
            get { return _filterExpression; }
        }

        public FilterOperator Operator
        {
            get { return _operator; }
        }

        public object GroupValue
        {
            get { return _groupValue; }
        }

        internal bool Evaluate(Rdl.Runtime.Context context)
        {
            object value1 = _filterExpression.Exec(context);
            List<object> values2 = new List<object>();
            foreach (Expression exp in _filterValues)
                values2.Add(exp.Exec(context));
            switch (_operator)
            {
                case FilterOperator.Equal:
                    return Rdl.Runtime.Compare.CompareTo(value1, values2[0]) == 0;
                case FilterOperator.Like:
                    return IsSqlLikeMatch((string)value1, (string)values2[0]);
                case FilterOperator.NotEqual:
                    return Rdl.Runtime.Compare.CompareTo(value1, values2[0]) != 0;
                case FilterOperator.GreaterThan:
                    return Rdl.Runtime.Compare.CompareTo(value1, values2[0]) > 0;
                case FilterOperator.GreaterThanOrEqual:
                    return Rdl.Runtime.Compare.CompareTo(value1, values2[0]) >= 0;
                case FilterOperator.LessThan:
                    return Rdl.Runtime.Compare.CompareTo(value1, values2[0]) < 0;
                case FilterOperator.LessThanOrEqual:
                    return Rdl.Runtime.Compare.CompareTo(value1, values2[0]) <= 0;
                case FilterOperator.TopN:
                    _groupValue = values2[0];
                    break;
                case FilterOperator.BottomN:
                    _groupValue = values2[0];
                    break;
                case FilterOperator.TopPercent:
                    _groupValue = values2[0];
                    break;
                case FilterOperator.BottomPercent:
                    _groupValue = values2[0];
                    break;
                case FilterOperator.In:
                    foreach (object o in values2)
                        if (Rdl.Runtime.Compare.CompareTo(value1, o) == 0)
                            return true;
                    return false;
                case FilterOperator.Between:
                    return Rdl.Runtime.Compare.CompareTo(value1, values2[0]) >= 0 &&
                        Rdl.Runtime.Compare.CompareTo(value1, values2[0]) <= 0;
            }
            return false;
        }

        private static bool IsSqlLikeMatch(string input, string pattern)
        {
            /* Turn "off" all regular expression related syntax in
            * the pattern string. */
            pattern = Regex.Escape(pattern);

            /* Replace the SQL LIKE wildcard metacharacters with the
            * equivalent regular expression metacharacters. */
            pattern = pattern.Replace("%", ".*?").Replace("_", ".");

            /* The previous call to Regex.Escape actually turned off
            * too many metacharacters, i.e. those which are recognized by
            * both the regular expression engine and the SQL LIKE
            * statement ([...] and [^...]). Those metacharacters have
            * to be manually unescaped here. */
            pattern = pattern.Replace(@"\[", "[").Replace(@"\]","]").Replace(@"\^", "^");

            return Regex.IsMatch(input, pattern);
        }
    }
}
