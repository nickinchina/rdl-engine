using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Rdl.Engine
{
    public class BorderColor : ReportElement
    {
        private Expression _default = null;
        private Expression _left = null;
        private Expression _right = null;
        private Expression _top = null;
        private Expression _bottom = null;

        public BorderColor(XmlNode node, ReportElement parent)
            : base(node, parent)
        {
        }

        protected override void ParseAttribute(XmlNode attr)
        {
            switch (attr.Name.ToLower())
            {
                case "default":
                    _default = new Expression(attr, this);
                    break;
                case "left":
                    _left = new Expression(attr, this);
                    break;
                case "right":
                    _right = new Expression(attr, this);
                    break;
                case "top":
                    _top = new Expression(attr, this);
                    break;
                case "bottom":
                    _bottom = new Expression(attr, this);
                    break;
                default:
                    break;
            }
        }

        public string Default(Rdl.Runtime.Context context)
        {
            return (_default == null) ? "black" : _default.ExecAsString(context);
        }

        public string Left(Rdl.Runtime.Context context)
        {
            return (_left == null) ? Default(context) : _left.ExecAsString(context);
        }

        public string Right(Rdl.Runtime.Context context)
        {
            return (_right == null) ? Default(context) : _right.ExecAsString(context);
        }

        public string Top(Rdl.Runtime.Context context)
        {
            return (_top == null) ? Default(context) : _top.ExecAsString(context);
        }

        public string Bottom(Rdl.Runtime.Context context)
        {
            return (_bottom == null) ? Default(context) : _bottom.ExecAsString(context);
        }
    }
}
