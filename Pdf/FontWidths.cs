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

namespace Rdl.Pdf
{
    class FontWidths : PdfObject
    {
        private uint[] _widths;

        public FontWidths(Document doc, IntPtr hdc, WinGdi.OUTLINETEXTMETRIC otm)
            : base(doc)
        {
            _widths = new uint[otm.TextMetrics.LastChar - otm.TextMetrics.FirstChar + 1];

            WinGdi.ABC[] abcWidths = new WinGdi.ABC[otm.TextMetrics.LastChar - otm.TextMetrics.FirstChar + 1];
            bool b = WinGdi.GetCharABCWidths(hdc, otm.TextMetrics.FirstChar, otm.TextMetrics.LastChar, ref abcWidths[0]);

            for (int i = 0; i < abcWidths.Length; i++)
                _widths[i] = (uint)(abcWidths[i].abcA + (int)abcWidths[i].abcB + abcWidths[i].abcC);
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("[");
            int j = 0;
            for (int i = 0; i < _widths.Length; i++)
            {
                if (j++ > 20)
                {
                    sb.AppendLine(_widths[i].ToString() + " ");
                    j = 0;
                }
                else
                    sb.Append(_widths[i].ToString() + " ");
            }
            sb.AppendLine("]");

            return sb.ToString();
        }
    }
}
