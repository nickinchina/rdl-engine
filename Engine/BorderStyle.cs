using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Rdl.Engine
{
    public class BorderStyle : ReportElement
    {
        public enum BorderStyleEnum
        {
            None,
            Dotted,
            Dashed,
            Solid,
            Double,
            Groove,
            Ridge,
            Inset,
            WindowInset,
            Outset
        };

        private Expression _default = null;
        private Expression _left = null;
        private Expression _right = null;
        private Expression _top = null;
        private Expression _bottom = null;

        public BorderStyle(XmlNode node, ReportElement parent)
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

        public BorderStyleEnum Default(Rdl.Runtime.Context context)
        {
            return (_default == null) ? BorderStyleEnum.None : (BorderStyleEnum)Enum.Parse(typeof(BorderStyleEnum), _default.ExecAsString(context),true);
        }

        public BorderStyleEnum Left(Rdl.Runtime.Context context)
        {
            return (_left == null) ? Default(context) : (BorderStyleEnum)Enum.Parse(typeof(BorderStyleEnum), _left.ExecAsString(context), true);
        }

        public BorderStyleEnum Right(Rdl.Runtime.Context context)
        {
            return (_right == null) ? Default(context) : (BorderStyleEnum)Enum.Parse(typeof(BorderStyleEnum), _right.ExecAsString(context), true);
        }

        public BorderStyleEnum Top(Rdl.Runtime.Context context)
        {
            return (_top == null) ? Default(context) : (BorderStyleEnum)Enum.Parse(typeof(BorderStyleEnum), _top.ExecAsString(context), true);
        }

        public BorderStyleEnum Bottom(Rdl.Runtime.Context context)
        {
            return (_bottom == null) ? Default(context) : (BorderStyleEnum)Enum.Parse(typeof(BorderStyleEnum), _bottom.ExecAsString(context), true);
        }
    }
}
