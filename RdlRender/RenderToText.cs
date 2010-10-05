using System;
using System.Collections.Generic;
using System.Text;

namespace RdlRender
{
    public class RenderToText
    {
        // Assumes 12 point lines
        private string[] _lines = new string[1];
        private Int32 _numLines;

        public string Render(Element report)
        {
            RecurseRender(report, 0, 0);

            StringBuilder sb = new StringBuilder();
            for( Int32 i=0; i < _numLines; i++)
                sb.Append(_lines[i] + "\n");

            return sb.ToString();
        }

        private void RecurseRender(Element elmt, decimal top, decimal left)
        {
            top += elmt.Top;
            left += elmt.Left;

            if (elmt is TextElement)
            {
                Int32 lineNo = (Int32)Math.Floor(top / 12m);
                if (lineNo >= _lines.Length)
                {
                   Array.Resize(ref _lines, Math.Max(_lines.Length * 2, lineNo+1));
                }
                _numLines = Math.Max(_numLines, lineNo+1);

                Int32 charPos = (Int32)Math.Floor(left/7.65m);
                Int32 charCount = (Int32)Math.Floor(elmt.Width / 7.65m);
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
