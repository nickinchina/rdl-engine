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
    public class RenderToCsv
    {
        class TextPoint
        {
            public string text;
            public decimal pos;
        }
        class TextLine
        {
            public List<TextPoint> text;
            public decimal pos;
        }

        private List<TextLine> _rows;

        public string Render(Rdl.Render.GenericRender report)
        {
            report.SetSizes(true);

            _rows = new List<TextLine>();
            RecurseBuildRows(report.BodyContainer, 0, 0);
            _rows.Sort(delegate(TextLine d1, TextLine d2) { return decimal.Compare(d1.pos, d2.pos); });

            StringBuilder sb = new StringBuilder();
            foreach (TextLine t in _rows)
            {
                t.text.Sort(delegate(TextPoint d1, TextPoint d2) { return decimal.Compare(d1.pos, d2.pos); });

                bool first = true;
                foreach( TextPoint p in t.text)
                {
                    if (!first)
                        sb.Append(",");
                    first = false;

                    sb.Append("\"" + p.text.Replace("\"", "\"\"").TrimEnd() + "\"");
                }
                sb.AppendLine();
            }

            return sb.ToString();
        }

        private void RecurseBuildRows(Element elmt, decimal top, decimal left)
        {
            top += elmt.Top;
            left += elmt.Left;

            if (elmt is TextElement) 
            {
                TextLine t = _rows.Find(delegate(TextLine d) { return d.pos == top; });
                if ( t == null)
                {
                    t = new TextLine();
                    t.pos = top;
                    t.text = new List<TextPoint>();
                    _rows.Add(t);
                }

                TextPoint p = new TextPoint();
                p.pos = left;
                p.text = ((TextElement)elmt).Text;
                t.text.Add(p);
            }

            if (elmt is Container)
                foreach (Element child in ((Container)elmt).Children)
                    RecurseBuildRows(child, top, left);
        }
    }
}
