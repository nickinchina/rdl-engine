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
using System.Drawing;

namespace Rdl.Render
{
    public class Drawing
    {
        public static void DrawBorder(System.Drawing.Graphics g, System.Drawing.Rectangle rect,
            Rdl.Render.BorderWidth bw, Rdl.Render.BorderStyle bs, Rdl.Render.BorderColor bc,
            decimal xMult, decimal yMult)
        {
            // TBD, we need to define background images to correspond with 
            // the different border styles.
            if (bw.Left.points > 0 && bs.Left != Rdl.Engine.BorderStyle.BorderStyleEnum.None)
                g.DrawLine(new System.Drawing.Pen(new SolidBrush(Rdl.Engine.Style.W32Color(bc.Left)), (float)(bw.Left.points * xMult)),
                    new Point(rect.Left, rect.Top), new Point(rect.Left, rect.Top + rect.Height));
            if (bw.Top.points > 0 && bs.Top != Rdl.Engine.BorderStyle.BorderStyleEnum.None)
                g.DrawLine(new System.Drawing.Pen(new SolidBrush(Rdl.Engine.Style.W32Color(bc.Top)), (float)(bw.Top.points * yMult)),
                    new Point(rect.Left, rect.Top), new Point(rect.Left + rect.Width, rect.Top));
            if (bw.Right.points > 0 && bs.Right != Rdl.Engine.BorderStyle.BorderStyleEnum.None)
                g.DrawLine(new System.Drawing.Pen(new SolidBrush(Rdl.Engine.Style.W32Color(bc.Right)), (float)(bw.Right.points * xMult)),
                    new Point(rect.Left + rect.Width, rect.Top), new Point(rect.Left + rect.Width, rect.Top + rect.Height));
            if (bw.Bottom.points > 0 && bs.Bottom != Rdl.Engine.BorderStyle.BorderStyleEnum.None)
                g.DrawLine(new System.Drawing.Pen(new SolidBrush(Rdl.Engine.Style.W32Color(bc.Bottom)), (float)(bw.Bottom.points * yMult)),
                    new Point(rect.Left, rect.Top + rect.Height), new Point(rect.Left + rect.Width, rect.Top + rect.Height));
        }
    }
}
