using System;
using System.Collections.Generic;
using System.Text;

namespace RdlRender
{
    public class BorderColor
    {
        public string Left;
        public string Right;
        public string Top;
        public string Bottom;

        public BorderColor(RdlEngine.BorderColor b)
        {
            Left = b.Left;
            Right = b.Right;
            Top = b.Top;
            Bottom = b.Bottom;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is BorderColor))
                return false;
            BorderColor b = obj as BorderColor;
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
