using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Rdl.Engine
{
    public class Expression : ReportElement
    {
        private string _expression = string.Empty;
        private Int32 _key = -1;

        public Expression(XmlNode node, ReportElement parent) : base(node, parent)
        {
        }

        public Expression(string expression, ReportElement parent)
            : base(null, parent)
        {
            _expression = expression;
        }

        protected override void ParseAttribute(XmlNode attr)
        {
            _expression = attr.InnerText;
            if (_expression.Length > 0)
                if (_expression[0] == '=')
                    _key = Report.AddFunction(_expression.Substring(1).Replace("\n", string.Empty).Replace("\r", string.Empty));
        }

        public bool Empty
        {
            get { return _expression == string.Empty; }
        }

        //public object Exec()
        //{
        //    if (_key >= 0)
        //        return Report.Rtb.Exec(_key, Context);
        //    else
        //        return _expression;
        //}
        public object Exec(Rdl.Runtime.Context context)
        {
            if (_key >= 0)
                return Report.Rtb.Exec(_key, context);
            else
                return _expression;
        }

        public string ExecAsString(Rdl.Runtime.Context context)
        {
            if (_key >= 0)
                return Report.Rtb.ExecAsString(_key, context);
            else
                return _expression;
        }

        public string ExecAsString(Rdl.Runtime.Context context, string format)
        {
            if (_key >= 0)
                try
                {
                    return Report.Rtb.ExecAsString(_key, context, format);
                }
                catch (Exception err)
                {
                    throw new Exception("Error evaluating expression " + _expression, err);
                }
            else
                return _expression;
        }

        public bool ExecAsBoolean(Rdl.Runtime.Context context)
        {
            if (_key >= 0)
                return Report.Rtb.ExecAsBoolean(_key, context);
            else
                return bool.Parse(_expression);
        }

        public Int32 ExecAsInt(Rdl.Runtime.Context context)
        {
            if (_key >= 0)
                return Report.Rtb.ExecAsInt(_key, context);
            else
                return Int32.Parse(_expression);
        }

        public decimal ExecAsDecimal(Rdl.Runtime.Context context)
        {
            if (_key >= 0)
                return Report.Rtb.ExecAsDecimal(_key, context);
            else
                return decimal.Parse(_expression);
        }
    }
}
