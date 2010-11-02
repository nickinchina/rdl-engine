using System;
using System.Collections.Generic;
using System.Text;

namespace RdlRender
{
    public class BorderStyle
    {
        public RdlEngine.BorderStyle.BorderStyleEnum Left;
        public RdlEngine.BorderStyle.BorderStyleEnum Right;
        public RdlEngine.BorderStyle.BorderStyleEnum Top;
        public RdlEngine.BorderStyle.BorderStyleEnum Bottom;

        public BorderStyle(RdlEngine.BorderStyle b)
        {
            Left = b.Left;
            Right = b.Right;
            Top = b.Top;
            Bottom = b.Bottom;
        }
        
        public override bool Equals(object obj)
        {
            if (!(obj is BorderStyle))
                return false;
            BorderStyle b = obj as BorderStyle;
            return (Left == b.Left &&
                Right == b.Right &&
                Top == b.Top &&
                Bottom == b.Bottom);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
