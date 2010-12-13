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

namespace Rdl.Pdf
{
    class FontDescriptor : PdfObject
    {
        private string _fontName;
        private WinGdi.OUTLINETEXTMETRIC _otm;

        public FontDescriptor(Document doc, string fontName, WinGdi.OUTLINETEXTMETRIC otm)
            : base(doc)
        {
            _fontName = fontName;
            _otm = otm;
        }

        private int DescriptorFlags()
        {
            int ret = 0;
            if ((_otm.TextMetrics.PitchAndFamily & WinGdi.TMPF_FIXED_PITCH) == 0)
                ret |= 1; // Fixed pitch
            // TBD I'm not sure how to determine AllCap, SmallCap or ForceBold
            //if (_otm.TextMetrics.CharSet == 0 || _otm.TextMetrics.CharSet == 1)
            //    ret |= 16; // Symbolic
            //else
                ret |= 4; // Not symbolic
            if (_otm.TextMetrics.Italic != 0)
                ret |= 32; // Italic
            if (_otm.PanoseNumber.bSerifStyle != 0)
                ret |= 2; // Serif
            if ((_otm.PanoseNumber.bFamilyType & WinGdi.PAN_FAMILY_SCRIPT) != 0)
                ret |= 8; // Script

            return ret;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("<</Type /FontDescriptor");
            sb.AppendLine("  /FontName /" + _fontName);
            sb.AppendLine("  /Flags " + DescriptorFlags().ToString());
            sb.AppendFormat(NumberFormatInfo.InvariantInfo,
                "  /FontBBox [{0} {1} {2} {3}]\n", -1 * _otm.TextMetrics.Overhang, -1 * _otm.TextMetrics.Descent, _otm.TextMetrics.MaxCharWidth, _otm.TextMetrics.Ascent);
            // TBD Find the actual italic angle
            sb.AppendLine("  /ItalicAngle -10");
            sb.AppendLine("  /Ascent " + _otm.TextMetrics.Ascent.ToString());
            sb.AppendLine("  /Descent " + (-1 * _otm.TextMetrics.Descent).ToString());
            sb.AppendLine(">>");

            return sb.ToString();
        }
    }
}
