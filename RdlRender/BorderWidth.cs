using System;
using System.Collections.Generic;
using System.Text;

namespace RdlRender
{
    public class BorderWidth
    {
        public RdlEngine.Size Left;
        public RdlEngine.Size Right;
        public RdlEngine.Size Top;
        public RdlEngine.Size Bottom;

        public BorderWidth(RdlEngine.BorderWidth b)
        {
            Left = b.Left;
            Right = b.Right;
            Top = b.Top;
            Bottom = b.Bottom;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is BorderWidth))
                return false;
            BorderWidth b = obj as BorderWidth;
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
