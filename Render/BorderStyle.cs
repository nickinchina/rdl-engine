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
