using System;
using System.Collections.Generic;
using System.Text;

namespace Rdl.Render
{
    public class BorderColor
    {
        public string Left;
        public string Right;
        public string Top;
        public string Bottom;

        public BorderColor(Rdl.Engine.BorderColor b, Rdl.Runtime.Context context)
        {
            Left = b.Left(context);
            Right = b.Right(context);
            Top = b.Top(context);
            Bottom = b.Bottom(context);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is BorderColor))
                return false;
            BorderColor b = obj as BorderColor;
            return (Left.Equals(b.Left) &&
                Right.Equals(b.Right) &&
                Top.Equals(b.Top) &&
                Bottom.Equals(b.Bottom));
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
