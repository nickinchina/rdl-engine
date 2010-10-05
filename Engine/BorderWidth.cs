using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Rdl.Engine
{
    public class BorderWidth : ReportElement
    {
        private Expression _default = null;
        private Expression _left = null;
        private Expression _right = null;
        private Expression _top = null;
        private Expression _bottom = null;

        public BorderWidth(XmlNode node, ReportElement parent)
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

        public Size Default(Rdl.Runtime.Context context)
        {
            return (_default == null) ? Size.OnePt : new Size(_default.ExecAsString(context));
        }

        public Size Left(Rdl.Runtime.Context context)
        {
            return (_left == null) ? Default(context) : new Size(_left.ExecAsString(context));
        }

        public Size Right(Rdl.Runtime.Context context)
        {
            return (_right == null) ? Default(context) : new Size(_right.ExecAsString(context));
        }

        public Size Top(Rdl.Runtime.Context context)
        {
            return (_top == null) ? Default(context) : new Size(_top.ExecAsString(context));
        }

        public Size Bottom(Rdl.Runtime.Context context)
        {
            return (_bottom == null) ? Default(context) : new Size(_bottom.ExecAsString(context));
        }
    }
}
