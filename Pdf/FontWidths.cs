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
