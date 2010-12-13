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
using System.Globalization;
using System.Text;
using System.Drawing;
using System.Runtime.InteropServices;

namespace Rdl.Pdf
{
    public class Font : PdfObject
    {
        string _fontName;
        string _faceName;
        //WinGdi.TEXTMETRICS _tm;
        WinGdi.OUTLINETEXTMETRIC _otm; 
        FontWidths _fw;
        FontDescriptor _fd;

        public Font(Document doc, string fontName, System.Drawing.Font winFont)
            : base(doc)
        {
            _fontName = fontName;

            //TBD - I have no idea why 720 seems to work.  I don't understand the relationship
            // between windows font coordinates and PDF glyph coordinates.  It seems like this
            // should be 1000, but that doesn't work.
            System.Drawing.Font f = new System.Drawing.Font(winFont.FontFamily, 720, winFont.Style, GraphicsUnit.Point);

            IntPtr hDC = WinGdi.GetDC(IntPtr.Zero); //Screen DC 
            IntPtr hObjOld = WinGdi.SelectObject(hDC, f.ToHfont());
            uint cbSize = WinGdi.GetOutlineTextMetrics(hDC, 0, IntPtr.Zero);
            if (cbSize == 0) throw new Exception();

            StringBuilder sb = new StringBuilder(50);
            WinGdi.GetTextFace(hDC, 50, sb);

            IntPtr buffer = Marshal.AllocHGlobal((int)cbSize);
            try
            {
                if (WinGdi.GetOutlineTextMetrics(hDC, cbSize, buffer) != 0)
                {
                    _otm = (WinGdi.OUTLINETEXTMETRIC)Marshal.PtrToStructure(buffer, typeof(WinGdi.OUTLINETEXTMETRIC));
                    _faceName = Marshal.PtrToStringAnsi((IntPtr)((long)buffer + _otm.pFaceName));
                    _faceName = _faceName.Replace(" ", "-");
                    _otm.EMSquare = (uint)(_otm.EMSquare / winFont.SizeInPoints);
                }
            }
            finally
            {
                Marshal.FreeHGlobal(buffer);
            }
 
            _fw = new FontWidths(doc, hDC, _otm);
            _fd = new FontDescriptor(doc, _faceName, _otm);

            WinGdi.SelectObject(hDC, hObjOld);
            WinGdi.ReleaseDC(IntPtr.Zero, hDC);
        }

        public override string ToString()
        {
            return string.Format("<</Type /Font /Name /{0}" +
                " /BaseFont /{1}" +
                " /Subtype /TrueType" +
                " /Encoding /WinAnsiEncoding" +
                " /FirstChar {2}" +
                " /LastChar {3}" +
                " /Widths {4} 0 R" +
                " /FontDescriptor {5} 0 R" +
                ">>",
                _fontName, _faceName, _otm.TextMetrics.FirstChar, _otm.TextMetrics.LastChar,
                _fw.Id, _fd.Id);
        }

        public string Name
        {
            get { return _fontName; }
        }

        public int Descent
        {
            get { return _otm.TextMetrics.Descent / 100; }
        }
    }
}
