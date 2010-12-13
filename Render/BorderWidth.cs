/*-----------------------------------------------------------------------------------
This file is part of the SawikiSoft RDL Engine.
The SawikiSoft RDL Engine is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

The SawikiSoft RDL Engine is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with Foobar.  If not, see <http://www.gnu.org/licenses/>.
-----------------------------------------------------------------------------------*/
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
