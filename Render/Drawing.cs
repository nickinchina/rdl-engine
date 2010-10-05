using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace Rdl.Render
{
    public class Drawing
    {
        public static void DrawBorder(System.Drawing.Graphics g, System.Drawing.Rectangle rect,
            Rdl.Render.BorderWidth bw, Rdl.Render.BorderStyle bs, Rdl.Render.BorderColor bc)
        {
            decimal xMult = (decimal)g.DpiX / 72;
            decimal yMult = (decimal)g.DpiY / 72;

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
