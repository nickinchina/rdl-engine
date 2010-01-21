using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Drawing;
using Rdl.Pdf;

namespace Rdl.Render
{
    public class RenderPagesToPdf
    {
        private Pdf.Font[] _pdfFontList;
        private System.Drawing.Font[] _winFontList;
        private Document _doc;
        private Rdl.Render.PageRender _pageRender;
        private Bitmap _bm;
        private Graphics _g;

        public string Render(Rdl.Engine.Report rpt, PageRender pageRender)
        {
            _doc = new Document();
            _pageRender = pageRender;

            _bm = new Bitmap(1000, 1000);
            _g = Graphics.FromImage(_bm);
            _g.PageUnit = GraphicsUnit.Point;

            // Create the fonts used in the report.
            int ct = rpt.StyleList.Count;
            _pdfFontList = new Pdf.Font[ct];
            _winFontList = new System.Drawing.Font[ct];
            for (int i = 0; i < ct; i++)
                if (rpt.StyleList[i] is TextStyle)
                {
                    TextStyle ts = rpt.StyleList[i] as TextStyle;

                    _winFontList[i] = ts.GetWindowsFont();

                    _pdfFontList[i] = new Pdf.Font(_doc, "F" + i.ToString(), _winFontList[i]);
                }

            // Loop through the pages in the document rendering the pages to PDF.
            for (int pageNum = 0; pageNum < pageRender.Pages.Count; pageNum++)
            {
                Pdf.Page pdfPage = _doc.Pages.AddPage(_doc,
                    new Rectangle(0, 0, (int)_pageRender.PageWidth, (int)_pageRender.PageHeight));

                Render.Page renderedPage = pageRender.Pages[pageNum];

                Pdf.ContentStream cs = pdfPage.AddContents(_doc);

                foreach (Element elmt in pageRender.Pages[pageNum].Children)
                    RecurseRender(elmt, cs, pageRender.TopMargin, pageRender.LeftMargin);
            }

            return _doc.ToString();
        }

        protected void RecurseRender(Element elmt, Pdf.ContentStream contents, decimal top, decimal left)
        {
            top += elmt.Top;
            left += elmt.Left;

            if (elmt is Container || elmt is TextElement)
            {
                // If the background is not transparent then fill it in.
                if (elmt.Style != null && elmt.Style.BackgroundColor != null && elmt.Style.BackgroundColor != "transparent")
                    contents.AddFillRect(left, _pageRender.PageHeight - (top + elmt.Height), left + elmt.Width, _pageRender.PageHeight - top,
                        Rdl.Engine.Style.W32Color(elmt.Style.BackgroundColor));

                // If there is a border then build the border.
                if (elmt.Style != null && elmt.Style.BorderWidth.Left.points > 0 && elmt.Style.BorderStyle.Left != Rdl.Engine.BorderStyle.BorderStyleEnum.None)
                    contents.AddLine(left, _pageRender.PageHeight - (top + elmt.Height), left, _pageRender.PageHeight - top,
                        elmt.Style.BorderWidth.Left.points,
                        Rdl.Engine.Style.W32Color(elmt.Style.BorderColor.Left),
                        PdfLineStyle(elmt.Style.BorderStyle.Left));
                if (elmt.Style != null && elmt.Style.BorderWidth.Right.points > 0 && elmt.Style.BorderStyle.Right != Rdl.Engine.BorderStyle.BorderStyleEnum.None)
                    contents.AddLine(left + elmt.Width, _pageRender.PageHeight - (top + elmt.Height), left + elmt.Width, _pageRender.PageHeight - top,
                        elmt.Style.BorderWidth.Right.points,
                        Rdl.Engine.Style.W32Color(elmt.Style.BorderColor.Right),
                        PdfLineStyle(elmt.Style.BorderStyle.Right));
                if (elmt.Style != null && elmt.Style.BorderWidth.Top.points > 0 && elmt.Style.BorderStyle.Top != Rdl.Engine.BorderStyle.BorderStyleEnum.None)
                    contents.AddLine(left, _pageRender.PageHeight - top, left + elmt.Width, _pageRender.PageHeight - top,
                        elmt.Style.BorderWidth.Top.points,
                        Rdl.Engine.Style.W32Color(elmt.Style.BorderColor.Top),
                        PdfLineStyle(elmt.Style.BorderStyle.Top));
                if (elmt.Style != null && elmt.Style.BorderWidth.Bottom.points > 0 && elmt.Style.BorderStyle.Bottom != Rdl.Engine.BorderStyle.BorderStyleEnum.None)
                    contents.AddLine(left, _pageRender.PageHeight - (top + elmt.Height), left + elmt.Width, _pageRender.PageHeight - (top + elmt.Height),
                        elmt.Style.BorderWidth.Bottom.points,
                        Rdl.Engine.Style.W32Color(elmt.Style.BorderColor.Bottom),
                        PdfLineStyle(elmt.Style.BorderStyle.Bottom));
            }

            if (elmt is TextElement)
            {
                TextElement te = elmt as TextElement;
                TextStyle ts = te.Style as TextStyle;

                if (te.Text.Length > 0)
                {
                    decimal[] widths;
                    CharacterRange[] charRanges = BuildCharacterRanges(
                        te.Text, 
                        _winFontList[elmt.StyleIndex], 
                        new Rectangle(0,0, (int)te.Width, (int)te.Height),
                        out widths);

                    for (int i = 0; i < charRanges.Length; i++)
                    {
                        decimal l = 0, r = 0;
                        switch (ts.TextAlign)
                        {
                            case Rdl.Engine.Style.TextAlignEnum.Left:
                            case Rdl.Engine.Style.TextAlignEnum.General:
                                l = left;
                                r = l + widths[i];
                                break;
                            case Rdl.Engine.Style.TextAlignEnum.Center:
                                l = left + ((elmt.Width - widths[i]) / 2);
                                r = l + widths[i];
                                break;
                            case Rdl.Engine.Style.TextAlignEnum.Right:
                                r = left + elmt.Width;
                                l = r - widths[i];
                                break;
                        }

                        contents.AddText(l, 
                            _pageRender.PageHeight - top - (ts.LineHeight.points * (i+1)), 
                            r,
                            _pageRender.PageHeight - top - (ts.LineHeight.points * i),
                            te.Text.Substring(charRanges[i].First, charRanges[i].Length),
                            Rdl.Engine.Style.W32Color(te.Style.Color), _pdfFontList[elmt.StyleIndex],
                            (int)ts.FontSize.points, (int)ts.LineHeight.points,
                            null,
                            Rdl.Pdf.ContentStream.WritingModeEnum.lr_tb,
                            PdfFontDecofation(ts.TextDecoration));
                    }
                }
            }

            if (elmt is Container)
                foreach (Element child in ((Container)elmt).Children)
                    RecurseRender(child, contents, top, left);
        }

        // As far as I can determine, there is no windows fn to show how a string is going
        // to be broken onto multiple lines, So we have to have a function to infer this infomation.
        private CharacterRange[] BuildCharacterRanges(
            string text, 
            System.Drawing.Font font, 
            Rectangle bounds,
            out decimal[] widths)
        {
            CharacterRange[] ranges = new CharacterRange[1];
            ranges[0] = new CharacterRange(0, text.Length);
            int currentRange = 0;
            while (true)
            {
                StringFormat sf = new StringFormat();
                sf.SetMeasurableCharacterRanges(ranges);
                Region[] regions = _g.MeasureCharacterRanges(
                    text, 
                    font,
                    bounds, 
                    sf
                    );

                if (regions[currentRange].GetBounds(_g).Height <= font.Height)
                    if (currentRange == ranges.Length - 1)
                    {
                        widths = new decimal[currentRange + 1];
                        for (int i = 0; i <= currentRange; i++)
                            widths[i] = (decimal)regions[i].GetBounds(_g).Width;
                        return ranges;
                    }
                    else
                        currentRange++;
                else
                {
                    if (ranges[currentRange].Length + ranges[currentRange].First == text.Length)
                    {
                        Array.Resize(ref ranges, currentRange + 2);
                        ranges[currentRange + 1].First = ranges[currentRange].First + ranges[currentRange].Length - 1;
                        ranges[currentRange + 1].Length = 1;
                    }
                    else
                    {
                        ranges[currentRange + 1].Length++;
                        ranges[currentRange + 1].First--;
                    }
                    ranges[currentRange].Length--;
                }
            }
        }

        Pdf.ContentStream.LineStyleEnum PdfLineStyle(Rdl.Engine.BorderStyle.BorderStyleEnum style)
        {
            switch (style)
            {
                case Rdl.Engine.BorderStyle.BorderStyleEnum.Dashed:
                    return Rdl.Pdf.ContentStream.LineStyleEnum.Dashed;
                case Rdl.Engine.BorderStyle.BorderStyleEnum.Dotted:
                    return Rdl.Pdf.ContentStream.LineStyleEnum.Dotted;
                default:
                    return Rdl.Pdf.ContentStream.LineStyleEnum.Solid;
            }
        }

        Pdf.ContentStream.TextDecorationEnum PdfFontDecofation(Rdl.Engine.Style.TextDecorationEnum td)
        {
            switch (td)
            {
                case Rdl.Engine.Style.TextDecorationEnum.LineThrough:
                    return Rdl.Pdf.ContentStream.TextDecorationEnum.LineThrough;
                case Rdl.Engine.Style.TextDecorationEnum.Overline:
                    return Rdl.Pdf.ContentStream.TextDecorationEnum.Overline;
                case Rdl.Engine.Style.TextDecorationEnum.Underline:
                    return Rdl.Pdf.ContentStream.TextDecorationEnum.Underline;
                default:
                    return Rdl.Pdf.ContentStream.TextDecorationEnum.None;
            }
        }
    }
}
