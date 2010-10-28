using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Text.RegularExpressions;
using System.Globalization;

namespace RdlEngine
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

        internal bool Evaluate()
        {
            object value1 = _filterExpression.Exec();
            List<object> values2 = new List<object>();
            foreach (Expression exp in _filterValues)
                values2.Add(exp.Exec());
            switch (_operator)
            {
                case FilterOperator.Equal:
                    return Utility.ApplyCompare(value1, values2[0]) == 0;
                case FilterOperator.Like:
                    return IsSqlLikeMatch((string)value1, (string)values2[0]);
                case FilterOperator.NotEqual:
                    return Utility.ApplyCompare(value1, values2[0]) != 0;
                case FilterOperator.GreaterThan:
                    return Utility.ApplyCompare(value1, values2[0]) > 0;
                case FilterOperator.GreaterThanOrEqual:
                    return Utility.ApplyCompare(value1, values2[0]) >= 0;
                case FilterOperator.LessThan:
                    return Utility.ApplyCompare(value1, values2[0]) < 0;
                case FilterOperator.LessThanOrEqual:
                    return Utility.ApplyCompare(value1, values2[0]) <= 0;
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
                        if (Utility.ApplyCompare(value1, o) == 0)
                            return true;
                    return false;
                case FilterOperator.Between:
                    return Utility.ApplyCompare(value1, values2[0]) >= 0 &&
                        Utility.ApplyCompare(value1, values2[1]) <= 0;
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
