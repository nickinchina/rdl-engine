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
using System.Globalization;
using System.Drawing;

namespace Rdl.Pdf
{
    public class ContentStream : PdfObject
    {
        public enum TextDecorationEnum
        {
            None,
            Underline,
            Overline,
            LineThrough
        };

        public enum WritingModeEnum
        {
            lr_tb,
            tb_rl
        };

        public enum LineStyleEnum
        {
            Solid,
            Dashed,
            Dotted
        };

        private StringBuilder _contents = new StringBuilder();
        private Dictionary<string, Font> _fontsUsed = new Dictionary<string, Font>();
        private Page _page;

        public ContentStream(Document doc, Page page)
            : base(doc)
        {
            _page = page;
        }

        public override string ToString()
        {
            string s = _contents.ToString();
            return "<</Length " + s.Length.ToString() + ">>\nstream\n" + _contents.ToString() + "endstream\n";
        }

        public Dictionary<string, Font> FontsUsed
        {
            get { return _fontsUsed; }
        }
        
        internal void AddLine(decimal x, decimal y, decimal x2, decimal y2, decimal width,
            System.Drawing.Color c,
            LineStyleEnum ls)
        {
            // Get the line color
            double red = c.R;
            double green = c.G;
            double blue = c.B;
            red = Math.Round((red / 255), 3);
            green = Math.Round((green / 255), 3);
            blue = Math.Round((blue / 255), 3);
            // Get the line style Dotted - Dashed - Solid
            // TBD - Other line styles are possible through tiling patters.  They just
            // need to be defined.
            string linestyle;
            switch (ls)
            {
                case LineStyleEnum.Dashed:
                    linestyle = "[3 2] 0 d";
                    break;
                case LineStyleEnum.Dotted:
                    linestyle = "[2] 0 d";
                    break;
                case LineStyleEnum.Solid:
                default:
                    linestyle = "[] 0 d";
                    break;
            }

            _contents.AppendFormat(NumberFormatInfo.InvariantInfo,
                "q {0} w {1} {2} {3} RG {4} {5} {6} m {7} {8} l S Q\n",
                width,			// line width
                red, green, blue,		// line color
                linestyle,				// line style
                x, y, x2, y2);	// positioning
        }

        /// <summary>
        /// Add a filled rectangle
        /// </summary>
        /// <returns></returns>
        internal void AddFillRect(decimal x, decimal y, decimal x2, decimal y2, Color c)
        {
            // Get the fill color
            double red = c.R;
            double green = c.G;
            double blue = c.B;
            red = Math.Round((red / 255), 3);
            green = Math.Round((green / 255), 3);
            blue = Math.Round((blue / 255), 3);

            _contents.AppendFormat(NumberFormatInfo.InvariantInfo,
                "q {0} {1} {2} RG {0} {1} {2} rg {3} {4} {5} {6} re f Q\n",
                red, green, blue,		// color
                x, y, x2, y2);	// positioning
        }

        /// <summary>
        /// Add image to the page.  Adds a new XObject Image and a reference to it.
        /// </summary>
        /// <returns>string Image name</returns>
        //internal string AddImage(PdfImages images, string name, int contentRef, StyleInfo si,
        //    ImageFormat imf, float x, float y, float width, float height,
        //    byte[] im, int samplesW, int samplesH, string url)
        //{
        //    string imgname = images.GetPdfImage(this.p, name, contentRef, imf, im, samplesW, samplesH);

        //    elements.AppendFormat(NumberFormatInfo.InvariantInfo,
        //        "\r\nq\t{2} 0 0 {3} {0} {1} cm\t",
        //        x, pSize.yHeight - y - height, width, height);	// push graphics state, positioning

        //    elements.AppendFormat(NumberFormatInfo.InvariantInfo, "\t/{0} Do\tQ\t", imgname);	// do the image then pop graphics state

        //    if (url != null)
        //        p.AddHyperlink(x, pSize.yHeight - y, height, width, url);

        //    // Border goes around the image padding
        //    AddBorder(si, x - si.PaddingLeft, y - si.PaddingTop,
        //        height + si.PaddingTop + si.PaddingBottom,
        //        width + si.PaddingLeft + si.PaddingRight);			// add any required border

        //    return imgname;
        //}

        /// <summary>
        /// Page Text element at the X Y position; multiple lines handled
        /// </summary>
        /// <returns></returns>
        internal void AddText(decimal x1, decimal y1, decimal x2, decimal y2, string text,
            Color color, Rdl.Pdf.Font font, int fontSize, int lineHeight, string url,
            WritingModeEnum writingMode,
            TextDecorationEnum textDecoration)
        {
            // Calculate the RGB colors e.g. RGB(255, 0, 0) = red = 1 0 0 rg
            double r = color.R;
            double g = color.G;
            double b = color.B;
            r = Math.Round((r / 255), 3);
            g = Math.Round((g / 255), 3);
            b = Math.Round((b / 255), 3);

            // Make sure the font used is included in the fonts for this page.
            if (!_fontsUsed.ContainsKey(font.Name))
                _fontsUsed.Add(font.Name, font);

            // Set the clipping path
            _contents.AppendFormat(NumberFormatInfo.InvariantInfo,
                "q {0} {1} {2} {3} re W n",
                x1, y1, x2, y2);

            if (url != null)
                AddHyperlink(x1, y2, x2, y2, url);

            // Escape the text
            text = text.Replace("\\", "\\\\").Replace("(", "\\(").Replace(")", "\\)");
            if (writingMode == WritingModeEnum.lr_tb)
            {
                _contents.AppendFormat(NumberFormatInfo.InvariantInfo,
                    " BT /{0} {1} Tf {3} {4} {5} rg {6} {7} Td ({2}) Tj ET Q\n",
                    font.Name, fontSize, text, r, g, b, x1, y1 + font.Descent);
            }
            //else
            //{	// Rotate text -90 degrees=-.5 radians (this works for english don't know about true tb-rl language)
            //    //   had to play with reader to find best approximation for this rotation; didn't do what I expected
            //    //    see pdf spec section 4.2.2 pg 141  "Common Transformations"

            //    elements.AppendFormat(NumberFormatInfo.InvariantInfo,
            //        "\r\nBT/{0} 0 R {1} Tf\t{5} {6} {7} rg\t{8} {9} {10} {11} {2} {3} Tm \t({4}) Tj\tET\tQ\t",
            //        font.Id, fontSize, x1, (pSize.yHeight - startY), text, r, g, b,
            //        radsCos, radsSin, -radsSin, radsCos);
            //}

            // Handle underlining etc.
            switch (textDecoration)
            {
                case TextDecorationEnum.Underline:
                    AddLine(x1, y1, x2, y1, 1, color, LineStyleEnum.Solid);
                    break;
                case TextDecorationEnum.LineThrough:
                    AddLine(x1, (y1 + y2) / 2, x2, (y1 + y2) / 2, 1, color, LineStyleEnum.Solid);
                    break;
                case TextDecorationEnum.Overline:
                    AddLine(x1, y2, x2, y2, 1, color, LineStyleEnum.Solid);
                    break;
                case TextDecorationEnum.None:
                default:
                    break;
            }
        }

        internal void AddHyperlink(decimal x1, decimal y1, decimal x2, decimal y2, string url)
        {
            _page.Annotations.AppendLine(string.Format(
                @"<</Type /Annot /Subtype /Link /Rect [{0} {1} {2} {3}] /Border [0 0 0] /A <</S /URI /URI ({4})>>>>",
                x1, y1, x2, y2, url));
        }
    }
}
