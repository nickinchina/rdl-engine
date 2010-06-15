using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Printing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Rdl.Render;
using System.IO;

namespace RdlViewer
{
    /// <summary>
    /// A Windows.Forms RDL report viewer control.
    /// </summary>
    public partial class ReportViewer : UserControl
    {
        struct ToggleElementStruct
        {
            public TextElement elmt;
            public Rectangle rect;
        }

        struct Line
        {
            public Point P1, P2;
            public int Width;

            public Line(decimal x1, decimal y1, decimal x2, decimal y2, decimal width)
            {
                P1 = new Point((int)x1, (int)y1);
                P2 = new Point((int)x2, (int)y2);
                Width = (int)width;
            }
        }

        public class ReportViewEventArgs : EventArgs
        {
            public ReportViewEventArgs(Rdl.Engine.Report report)
            {
                Report = report;
            }

            public Rdl.Engine.Report Report { get; set; }
        }

        private PageRender _pageRender = null;
        private Rdl.Engine.Report _report = null;
        private Page _currentPage = null;
        private int _currentPageNum = 0;
        private Font[] _fonts;
        private int _printingPage = 0;
        private PrintDocument _pd;
        private int _pageOffset = 0;
        private List<ToggleElementStruct> _displayedToggleList = null;
        private List<Line> _lineList = new List<Line>();
        private Bitmap _pageBitmap = null;
        public event EventHandler<ReportViewEventArgs> BeginPrint;
        public event EventHandler<ReportViewEventArgs> EndPrint;

        /// <summary>
        /// Default constructor
        /// </summary>
        public ReportViewer()
        {
            InitializeComponent();

            _pd = new PrintDocument();
            _pd.PrintPage += new PrintPageEventHandler(pd_PrintPage);
            _pd.BeginPrint += new PrintEventHandler(pd_BeginPrint);
            _pd.EndPrint += new PrintEventHandler(pd_EndPrint);
        }

        void pd_EndPrint(object sender, PrintEventArgs e)
        {
            if (EndPrint != null)
                EndPrint(this, new ReportViewEventArgs(_report));
        }

        /// <summary>
        /// Sets the report to view in the viewer control
        /// </summary>
        /// <param name="report"></param>
        public void SetReport( Rdl.Engine.Report report )
        {
            _report = report;

            _fonts = new Font[_report.StyleList.Count];

            // Build a list of fonts corresponding to the styles in the report
            for (int i = 0; i < _report.StyleList.Count; i++)
            {
                if (_report.StyleList[i] is Rdl.Render.TextStyle)
                {
                    TextStyle ts = _report.StyleList[i] as TextStyle;
                    _fonts[i] = ts.GetWindowsFont();
                }
            }

            // Render the report to a list of pages.
            _pageRender = new PageRender();
            _pageRender.Render(report);


            StreamWriter sw;
            if (_pageRender.Pages.Count > 0)
            {
                sw = new StreamWriter(@"pages.txt", false);

                for (int i = 0; i < _pageRender.Pages.Count; i++)
                {
                    sw.WriteLine("Page " + i.ToString());
                    sw.Write(_pageRender.Pages[i].ToString(0));
                    sw.WriteLine("\n");
                }
                sw.Close();
            }

            textPages.Text = _pageRender.Pages.Count.ToString();
            _currentPageNum = 0;
            _currentPage = _pageRender.Pages[0];

            BuildToggleList(_currentPage);

            // Set the scrollable position of the panel.
            panelReport.Invalidate();
        }

        //private void layoutPage()
        //{
        //    panelReport.Controls.Clear();
        //    _lineList.Clear();
        //    foreach (Rdl.Render.Element elmt in _currentPage.Box.Children)
        //        recurseLayoutPage(elmt, 0, 0);
        //}

        //private void recurseLayoutPage(Rdl.Render.Element elmt, decimal top, decimal left)
        //{
        //    decimal l = left;
        //    decimal width = elmt.Width;
        //    Graphics g = CreateGraphics();

        //    decimal mult = (decimal)g.DpiX / 72m;
        //    g.Dispose();

        //    if (elmt is Rdl.Render.TextElement || elmt is Rdl.Render.Container)
        //    {
        //        if (elmt.Style != null && elmt.Style.BorderWidth != null)
        //        {
        //            // TBD, we need to define background images to correspond with 
        //            // the different border styles.
        //            if (elmt.Style.BorderWidth.Left.points > 0 && elmt.Style.BorderStyle.Left != Rdl.Engine.BorderStyle.BorderStyleEnum.None)
        //                _lineList.Add(new Line(left, top, left, top + elmt.Width, elmt.Style.BorderWidth.Right.points));
        //            if (elmt.Style.BorderWidth.Top.points > 0 && elmt.Style.BorderStyle.Top != Rdl.Engine.BorderStyle.BorderStyleEnum.None)
        //                _lineList.Add(new Line(left, top, left + elmt.Width, top, elmt.Style.BorderWidth.Top.points));
        //            if (elmt.Style.BorderWidth.Right.points > 0 && elmt.Style.BorderStyle.Right != Rdl.Engine.BorderStyle.BorderStyleEnum.None)
        //                _lineList.Add(new Line(left+elmt.Width, top, left + elmt.Width, top+elmt.Height, elmt.Style.BorderWidth.Right.points));
        //            if (elmt.Style.BorderWidth.Bottom.points > 0 && elmt.Style.BorderStyle.Bottom != Rdl.Engine.BorderStyle.BorderStyleEnum.None)
        //                _lineList.Add(new Line(left, top+elmt.Height, left + elmt.Width, top+elmt.Height, elmt.Style.BorderWidth.Bottom.points));
        //        }
        //    }

        //    if (elmt is TextElement)
        //    {
        //        TextElement te = elmt as TextElement;
        //        TextStyle ts = te.Style as TextStyle;

        //        if (te.ToggleList != null && te.ToggleList.Count > 0)
        //        {
        //            Button btn = new Button();
        //            if (te.ToggleState == TextElement.ToggleStateEnum.open)
        //                btn.Image = Properties.Resources.minus;
        //            else
        //                btn.Image =  Properties.Resources.plus;
        //            btn.Top = (int)(top * mult); 
        //            btn.Left = (int)(left * mult);
        //            btn.Width = Properties.Resources.minus.Width;
        //            btn.Text = string.Empty;
        //            btn.Click += new EventHandler(btn_Click);
        //            btn.FlatStyle = FlatStyle.Flat;
        //            btn.FlatAppearance.BorderSize = 0;

        //            panelReport.Controls.Add(btn);

        //            l += Properties.Resources.minus.Width;
        //            width -= Properties.Resources.minus.Width;
        //        }

        //        ContentAlignment ca = ContentAlignment.TopLeft;
        //        switch( ts.TextAlign)
        //        {
        //            case Rdl.Engine.Style.TextAlignEnum.General:
        //            case Rdl.Engine.Style.TextAlignEnum.Left:
        //                switch (ts.VerticalAlign)
        //                {
        //                    case Rdl.Engine.Style.VerticalAlignEnum.Top:
        //                        ca = ContentAlignment.TopLeft;
        //                        break;
        //                    case Rdl.Engine.Style.VerticalAlignEnum.Bottom:
        //                        ca = ContentAlignment.BottomLeft;
        //                        break;
        //                    case Rdl.Engine.Style.VerticalAlignEnum.Middle:
        //                        ca = ContentAlignment.MiddleLeft;
        //                        break;
        //                }
        //                break;
        //            case Rdl.Engine.Style.TextAlignEnum.Center:
        //                switch (ts.VerticalAlign)
        //                {
        //                    case Rdl.Engine.Style.VerticalAlignEnum.Top:
        //                        ca = ContentAlignment.TopCenter;
        //                        break;
        //                    case Rdl.Engine.Style.VerticalAlignEnum.Bottom:
        //                        ca = ContentAlignment.BottomCenter;
        //                        break;
        //                    case Rdl.Engine.Style.VerticalAlignEnum.Middle:
        //                        ca = ContentAlignment.TopCenter;
        //                        break;
        //                }
        //                break;
        //            case Rdl.Engine.Style.TextAlignEnum.Right:
        //                switch (ts.VerticalAlign)
        //                {
        //                    case Rdl.Engine.Style.VerticalAlignEnum.Top:
        //                        ca = ContentAlignment.TopRight;
        //                        break;
        //                    case Rdl.Engine.Style.VerticalAlignEnum.Bottom:
        //                        ca = ContentAlignment.BottomRight;
        //                        break;
        //                    case Rdl.Engine.Style.VerticalAlignEnum.Middle:
        //                        ca = ContentAlignment.MiddleRight;
        //                        break;
        //                }
        //                break;
        //        }

        //        Label lbl = new Label();
        //        lbl.Text = te.Text;
        //        lbl.TextAlign = ca;
        //        lbl.Cursor = Cursors.IBeam;
        //        lbl.BorderStyle = System.Windows.Forms.BorderStyle.None;
        //        lbl.Top = (int)(top * mult);
        //        lbl.Left = (int)(l * mult);
        //        lbl.Width = (int)(width * mult);
        //        lbl.Height = (int)(elmt.Height * mult);
        //        lbl.Font = _fonts[te.StyleIndex];
        //        lbl.ForeColor = Rdl.Engine.Style.W32Color(ts.Color);
        //        panelReport.Controls.Add(lbl);
        //    }

        //    if (elmt is Rdl.Render.Container)
        //        foreach (Element child in ((Rdl.Render.Container)elmt).Children)
        //            recurseLayoutPage(child, top + elmt.Top, left + elmt.Left);
        //}

        void btn_Click(object sender, EventArgs e)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        private void panelReport_Paint(object sender, PaintEventArgs e)
        {
            textCurrentPage.Text = (_currentPageNum + 1).ToString();

            _pageOffset = 0;
            if (_currentPage == null)
                e.Graphics.Clear(Color.White);
            else
            {
                if (_currentPage != null && _currentPage.Style != null && _currentPage.Style.BackgroundColor != null)
                    e.Graphics.Clear(Rdl.Engine.Style.W32Color(_currentPage.Style.BackgroundColor));
                else
                    e.Graphics.Clear(Color.White);

                // Adjust the scroll position and clip rectangle to point coordinates.
                Point scrollPos = panelReport.AutoScrollPosition;
                Rectangle clipRect = e.ClipRectangle;

                if (_pageBitmap == null)
                    _pageBitmap = PaintPage(e.Graphics, _currentPage, false, (decimal)e.Graphics.DpiX / 72, (decimal)e.Graphics.DpiY / 72);
                panelReport.AutoScrollMinSize = new Size(_pageBitmap.Width, _pageBitmap.Height);

                // Intersect the clip rectangle with the bitmap.
                clipRect.Width = Math.Min(clipRect.Width, _pageBitmap.Width + scrollPos.X - clipRect.X);
                clipRect.Height = Math.Min(clipRect.Height, _pageBitmap.Height + scrollPos.Y - clipRect.Y);

                if (clipRect.Width > 0 && clipRect.Height > 0)
                    e.Graphics.DrawImage(_pageBitmap, clipRect,
                        new Rectangle(clipRect.X - scrollPos.X, clipRect.Y - scrollPos.Y, clipRect.Width, clipRect.Height),
                        e.Graphics.PageUnit);
            }
        }

        private Bitmap PaintPage(Graphics g, Rdl.Render.Page rdlPage, bool print, decimal xMult, decimal yMult)
        {
            Bitmap bp = new Bitmap((int)((float)rdlPage.Width * g.DpiX / 72), (int)((float)rdlPage.Height * g.DpiY / 72));

            foreach (Rdl.Render.Element elmt in rdlPage.Children)
            {
                RecursePaintPanel(Graphics.FromImage(bp),
                    xMult,
                    yMult,
                    print,
                    elmt,
                    new Point(0,0),
                    0m, 0m);
            }

            return bp;
        }

        private void RecursePaintPanel(Graphics g, 
            decimal xMult,
            decimal yMult,
            bool print, 
            Rdl.Render.Element elmt, 
            Point destOffset,
            decimal top, decimal left)
        {
            decimal mult = 1;

            int t = (int)((top + elmt.Top) * yMult) + destOffset.Y;
            int l = (int)((left + elmt.Left) * xMult) + destOffset.X;
            int w;
            int h;
            if (elmt is Rdl.Render.ImageElement && _report.ImageList[elmt._imageIndex].Sizing == Rdl.Engine.Image.ImageSizingEnum.Autosize)
            {
                w = (int)elmt.Width;
                h = (int)elmt.Height;
            }
            else
            {
                w = (int)(elmt.Width * xMult);
                h = (int)(elmt.Height * yMult);
            }

            Rectangle clipRect = new Rectangle(l, t, w, h);

            if (elmt is Rdl.Render.TextElement 
                || elmt is Rdl.Render.ImageElement 
                || elmt is Rdl.Render.Container
                || elmt is Rdl.Render.ChartElement)
            {
                if (elmt.Style != null && elmt.Style.BackgroundColor != null)
                    g.FillRectangle(new SolidBrush(Rdl.Engine.Style.W32Color(elmt.Style.BackgroundColor)),
                        clipRect);

                if (elmt.Style != null && elmt.Style.BorderWidth != null)
                    Rdl.Render.Drawing.DrawBorder(g, new System.Drawing.Rectangle(l, t, w, h),
                        elmt.Style.BorderWidth, elmt.Style.BorderStyle, elmt.Style.BorderColor);
            }

            if (elmt.Style != null)
            {
                l += (int)(elmt.Style.PaddingLeft.points * xMult);
                t += (int)(elmt.Style.PaddingTop.points * yMult);
                w -= (int)((elmt.Style.PaddingLeft.points + elmt.Style.PaddingRight.points) * xMult);
                h -= (int)((elmt.Style.PaddingTop.points + elmt.Style.PaddingBottom.points) * yMult);
            }

            if (elmt is TextElement)
            {
                TextElement te = elmt as TextElement;
                TextStyle ts = te.Style as TextStyle;
                int width = (int)(elmt.Width * mult);

                if (te.IsToggle && !print)
                {
                    if (te.ToggleState == TextElement.ToggleStateEnum.open)
                        g.DrawImage(Properties.Resources.minus, new Point(l, t));
                    else
                        g.DrawImage(Properties.Resources.plus, new Point(l, t));

                    l += Properties.Resources.minus.Width;
                    width -= Properties.Resources.minus.Width;
                }

                StringFormat sf = new StringFormat();
                Rdl.Engine.Style.TextAlignEnum align = ts.TextAlign;
                if (align == Rdl.Engine.Style.TextAlignEnum.General)
                {
                    decimal d;
                    if (decimal.TryParse(te.Text, out d))
                        align = Rdl.Engine.Style.TextAlignEnum.Right;
                    else
                        align = Rdl.Engine.Style.TextAlignEnum.Left;
                }
                switch(align)
                {
                    case Rdl.Engine.Style.TextAlignEnum.General:
                    case Rdl.Engine.Style.TextAlignEnum.Left:
                        sf.Alignment = StringAlignment.Near;
                        break;
                    case Rdl.Engine.Style.TextAlignEnum.Center:
                        sf.Alignment = StringAlignment.Center;
                        break;
                    case Rdl.Engine.Style.TextAlignEnum.Right:
                        sf.Alignment = StringAlignment.Far;
                        break;
                }

                g.DrawString(te.Text,
                    _fonts[te.StyleIndex],
                    new SolidBrush(Rdl.Engine.Style.W32Color(te.Style.Color)),
                    new Rectangle(l, t, w, h),
                    sf
                    );
            }

            if (elmt._imageIndex >= 0)
            {
                g.DrawImage(_report.ImageList[elmt._imageIndex].GetSizedImage(w, h),
                    new Rectangle(l, t, w, h));
            }

            if (elmt is ChartElement)
            {
                ChartElement cc = elmt as ChartElement;
                g.DrawImage(cc.RenderChart(w, h), new Rectangle(l, t, w, h));
            }

            if (elmt is Rdl.Render.Container)
                foreach (Element child in ((Rdl.Render.Container)elmt).Children)
                    RecursePaintPanel(g, xMult, yMult, print, child, destOffset, top + elmt.Top, left + elmt.Left);
        }

        private void BuildToggleList(Rdl.Render.Page page)
        {
            _displayedToggleList = new List<ToggleElementStruct>();
            foreach (Element child in page.Children)
                RecurseBuildToggleList(child, 0, 0);
        }

        private void RecurseBuildToggleList(Rdl.Render.Element elmt, decimal top, decimal left)
        {
            if (elmt is TextElement)
            {
                TextElement te = elmt as TextElement;
                TextStyle ts = te.Style as TextStyle;
                int width = (int)elmt.Width;

                if (te.IsToggle)
                {
                    ToggleElementStruct tes = new ToggleElementStruct();
                    tes.elmt = te;
                    tes.rect = new Rectangle((int)left, (int)top, Properties.Resources.minus.Width, Properties.Resources.minus.Height);
                    _displayedToggleList.Add(tes);
                }
            }

            if (elmt is Rdl.Render.Container)
                foreach (Element child in ((Rdl.Render.Container)elmt).Children)
                    RecurseBuildToggleList(child, top + elmt.Top, left + elmt.Left);
        }

        private void ReportViewer_Resize(object sender, EventArgs e)
        {
            panelReport.Height = Height - 31;
        }

        private void buttonPageDown_Click(object sender, EventArgs e)
        {
            if (_currentPageNum+1 < _pageRender.Pages.Count)
            {
                _currentPageNum++;
                _currentPage = _pageRender.Pages[_currentPageNum];
                BuildToggleList(_currentPage);
                _pageBitmap = null;
                panelReport.Refresh();
            }
        }

        private void buttonPageUp_Click(object sender, EventArgs e)
        {
            if (_currentPageNum > 0)
            {
                _currentPageNum--;
                _currentPage = _pageRender.Pages[_currentPageNum];
                BuildToggleList(_currentPage);
                _pageBitmap = null;
                panelReport.Refresh();
            }
        }

        private void textCurrentPage_Validating(object sender, CancelEventArgs e)
        {
            int p;
            if (int.TryParse(textCurrentPage.Text, out p))
            {
                if (p > 0 && p <= _pageRender.Pages.Count)
                {
                    _currentPageNum = p-1;
                    _currentPage = _pageRender.Pages[_currentPageNum];
                    panelReport.Refresh();
                }
                else
                    e.Cancel = true;
            }
            else
                e.Cancel = true;
        }

        /// <summary>
        /// Print the report viewing in the control
        /// </summary>
        /// <param name="showPrintDialog"></param>
        public void Print(bool showPrintDialog)
        {
            bool print = true;
            if (showPrintDialog)
            {
                printDialog1.Document = _pd;
                printDialog1.AllowSomePages = true;
                print = (printDialog1.ShowDialog() == DialogResult.OK);
            }
            if (print)
                _pd.Print();
        }

        private void buttonPrint_Click(object sender, EventArgs e)
        {
            Print(true);
        }

        private void buttonPrintPreview_Click(object sender, EventArgs e)
        {
            printPreviewDialog1.Document = _pd;
            printPreviewDialog1.ShowDialog();
        }

        private void buttonPageSetup_Click(object sender, EventArgs e)
        {
            pageSetupDialog1.Document = _pd;
            pageSetupDialog1.ShowDialog();
        }

        void pd_BeginPrint(object sender, PrintEventArgs e)
        {
            if (BeginPrint != null)
                BeginPrint(this, new ReportViewEventArgs(_report));
            _printingPage = 0;
            _pageOffset = 0;

            int pageWidth, pageHeight;
            if (_pageRender.PageHeight > _pageRender.PageWidth)
            {
                pageWidth = (int)(_pageRender.PageWidth * 100) / 72;
                pageHeight = (int)(_pageRender.PageHeight * 100) / 72;
                _pd.DefaultPageSettings.Landscape = false;
            }
            else
            {
                pageWidth = (int)(_pageRender.PageHeight * 100) / 72;
                pageHeight = (int)(_pageRender.PageWidth * 100) / 72;
                _pd.DefaultPageSettings.Landscape = true;
            }
            foreach (PaperSize ps in _pd.PrinterSettings.PaperSizes)
                if (ps.Width == pageWidth && ps.Height == pageHeight)
                    _pd.PrinterSettings.DefaultPageSettings.PaperSize = ps;
            _pd.DefaultPageSettings.Margins = new Margins((int)_pageRender.LeftMargin,
                0,
                (int)_pageRender.TopMargin,
                0);
        }

        void pd_QueryPageSettings(object sender, QueryPageSettingsEventArgs e)
        {
        }

        void pd_PrintPage(object sender, PrintPageEventArgs e)
        {
            int printWidth = (int)(_pageRender.PageWidth - _pageRender.LeftMargin - _pageRender.RightMargin);
            int printHeight = (int)(_pageRender.PageHeight - _pageRender.TopMargin - _pageRender.BottomMargin);

            e.Graphics.PageUnit = GraphicsUnit.Point;
            e.Graphics.IntersectClip(
                new Rectangle((int)_pageRender.LeftMargin, (int)_pageRender.TopMargin, printWidth, printHeight));
            foreach (Rdl.Render.Element elmt in _pageRender.Pages[_printingPage].Children)
            {
                RecursePaintPanel(e.Graphics,
                    1,1,true,
                    elmt,
                    new Point((int)_pageRender.LeftMargin - _pageOffset, (int)_pageRender.TopMargin),
                    0m, 0m);
            }

            if ((int)_pageRender.Pages[_printingPage].Width - _pageOffset > printWidth)
            {
                _pageOffset += printWidth;
                e.HasMorePages = true;
            }
            else
            {
                _printingPage++;
                _pageOffset = 0;
                e.HasMorePages = (_printingPage < _pageRender.Pages.Count);
            }
        }

        private void panelReport_MouseClick(object sender, MouseEventArgs e)
        {
            Point p = e.Location;
            p.Y -= panelReport.AutoScrollPosition.Y;
            p.X -= panelReport.AutoScrollPosition.X;
            Graphics g = this.CreateGraphics();
            p.X = (int)((float)p.X * 72f / g.DpiX);
            p.Y = (int)((float)p.Y * 72f / g.DpiY);
            if (_displayedToggleList != null)
                foreach( ToggleElementStruct tes in _displayedToggleList)
                    if (tes.rect.Contains(p))
                    {
                        if (tes.elmt.ToggleState == TextElement.ToggleStateEnum.open)
                            tes.elmt.ToggleState = TextElement.ToggleStateEnum.closed;
                        else
                            tes.elmt.ToggleState = TextElement.ToggleStateEnum.open;

                        // Render the report to a list of pages.
                        _pageRender.Render(_report);

                        textPages.Text = _pageRender.Pages.Count.ToString();
                        if (_currentPageNum > _pageRender.Pages.Count - 1)
                            _currentPageNum = _pageRender.Pages.Count - 1;
                        _currentPage = _pageRender.Pages[_currentPageNum];

                        BuildToggleList(_currentPage);

                        if (_pageRender.Pages.Count > 1)
                        {
                            StreamWriter sw = new StreamWriter(@"pages.txt", false);
                            sw.Write(_pageRender.Pages[1].ToString());
                            sw.Close();
                        }

                        // Set the scrollable position of the panel.
                        _pageBitmap = null;
                        panelReport.Refresh();
                    }
            g.Dispose();
        }

        private void comboExport_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboExport.SelectedIndex > 0)
            {
                SaveFileDialog fd = new SaveFileDialog();
                fd.DefaultExt = comboExport.Text;
                if (fd.ShowDialog() == DialogResult.OK)
                {
                    StreamWriter sw = new StreamWriter(fd.OpenFile());
                    switch(comboExport.Text)
                    {
                        case "Pdf":
                            Rdl.Render.RenderPagesToPdf pdf = new RenderPagesToPdf();
                            sw.Write(pdf.Render(_report, _pageRender));
                            break;
                        case "Txt":
                            Rdl.Render.RenderToText txt = new RenderToText();
                            sw.Write(txt.Render(_report));
                            break;
                        case "Xls":
                            Rdl.Render.RenderToXls xls = new RenderToXls();
                            sw.Write(xls.Render(_report));
                            break;
                        default:
                            break;
                    }
                    sw.Close();
                }
            }

            comboExport.SelectedIndex = -1;
            comboExport.Text = string.Empty;
        }

    }
}