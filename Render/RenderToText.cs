using System;
using System.Collections.Generic;
using System.Text;

namespace Rdl.Render
{
    public class RenderToText
    {
        // _pointsPerChar = 72pt/i * 8.5i/pg / 80c/pg = 7.65pt/c
        private const decimal _pointsPerChar = 7.65m;
        private List<decimal> _rows = new List<decimal>();
        private int[] _rowsToLines;
        private string[] _lines = new string[1];

        public string Render(Rdl.Engine.Report report)
        {
            report.SetSizes(true);

            RecurseBuildRows(report.BodyContainer, 0);
            _rows.Add(0);
            _rows.Sort(delegate(decimal d1, decimal d2) { return decimal.Compare(d1, d2); });

            // Convert the list of text positions into text lines assuming 12 points per line.
            _rowsToLines = new int[_rows.Count];
            int line = 0;
            decimal lastPos = 0;
            for (int i = 0; i < _rows.Count; i++)
            {
                int lineAdd = (int)((_rows[i] - lastPos)/24);
                line += lineAdd;
                _rowsToLines[i] = line;
                lastPos = _rows[i];

                line++;
            }
            _lines = new string[line];

            // Render the report onto _lines.
            RecurseRender(report.BodyContainer, 0, 0);

            StringBuilder sb = new StringBuilder();
            for (Int32 i = 0; i < line; i++)
                if (_lines[i] == null)
                    sb.AppendLine();
                else
                    sb.AppendLine(_lines[i]);

            return sb.ToString();
        }

        private void RecurseBuildRows(Element elmt, decimal top)
        {
            top += elmt.Top;

            if (elmt is TextElement && top != 0 && _rows.Find(delegate(decimal d) { return d == top; }) == decimal.Zero)
                _rows.Add(top);

            if (elmt is Container)
                foreach (Element child in ((Container)elmt).Children)
                    RecurseBuildRows(child, top);
        }


        private void RecurseRender(Element elmt, decimal top, decimal left)
        {
            top += elmt.Top;
            left += elmt.Left;

            if (elmt is TextElement)
            {
                Int32 row = _rows.FindIndex(delegate(decimal d) { return d == top; });
                Int32 lineNo = _rowsToLines[row];

                Int32 charPos = (Int32)Math.Floor(left / _pointsPerChar);
                Int32 charCount = (Int32)Math.Floor(elmt.Width / _pointsPerChar);
                string text = ((TextElement)elmt).Text;
                if (text != null)
                {
                    text = text.Substring(0, Math.Min(text.Length, charCount));
                    if (_lines[lineNo] == null)
                    {
                        _lines[lineNo] = (new String(' ', charPos)) + text;
                    }
                    else
                    {
                        string str = _lines[lineNo];

                        _lines[lineNo] = str.Substring(0, Math.Min(charPos, str.Length)).PadRight(charPos)
                            + text;
                        if (str.Length > _lines[lineNo].Length)
                            _lines[lineNo] += str.Substring(charPos + text.Length);
                    }
                }
            }

            if (elmt is Container)
                foreach (Element child in ((Container)elmt).Children)
                    RecurseRender(child, top, left);
        }
    }
}
