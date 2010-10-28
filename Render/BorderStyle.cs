using System;
using System.Collections.Generic;
using System.Text;

namespace Rdl.Render
{
    public class BorderStyle
    {
        public Rdl.Engine.BorderStyle.BorderStyleEnum Left;
        public Rdl.Engine.BorderStyle.BorderStyleEnum Right;
        public Rdl.Engine.BorderStyle.BorderStyleEnum Top;
        public Rdl.Engine.BorderStyle.BorderStyleEnum Bottom;

        public BorderStyle(Rdl.Engine.BorderStyle b, Rdl.Runtime.Context context)
        {
            Left = b.Left(context);
            Right = b.Right(context);
            Top = b.Top(context);
            Bottom = b.Bottom(context);
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
