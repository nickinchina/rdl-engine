using System;
using System.Collections.Generic;
using System.Text;

namespace Rdl.Render
{
    public class BorderWidth
    {
        public Rdl.Engine.Size Left;
        public Rdl.Engine.Size Right;
        public Rdl.Engine.Size Top;
        public Rdl.Engine.Size Bottom;

        public BorderWidth(Rdl.Engine.BorderWidth b, Rdl.Runtime.Context context)
        {
            Left = b.Left(context);
            Right = b.Right(context);
            Top = b.Top(context);
            Bottom = b.Bottom(context);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is BorderWidth))
                return false;
            BorderWidth b = obj as BorderWidth;
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
