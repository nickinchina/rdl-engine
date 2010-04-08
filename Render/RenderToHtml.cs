using System;
using System.Collections.Generic;
using System.Text;

namespace Rdl.Render
{
    public class RenderToHtml
    {
        protected StringBuilder _body = new StringBuilder();
        protected StringBuilder _styles = new StringBuilder();
        private int _styleBase = 0;
        protected StringBuilder _script = new StringBuilder();
        private string _plusGif = "plus.gif";
        private string _minusGif = "minus.gif";
        protected Rdl.Engine.Report _sourceReport = null;
        private Dictionary<string, Rdl.Render.ChartElement> _charts = new Dictionary<string, ChartElement>();

        public class ImageUrlArgs : EventArgs
        {
            public string ElementName;
            public string Source;
            public string ImageName;
            public string Url;
        }
        public delegate void ImageUrlEventHandler(object sender, ImageUrlArgs args);
        public event ImageUrlEventHandler ImageUrl;

        public void Render(Rdl.Engine.Report rpt)
        {
            Render(rpt, false);
        }

        public void Render(Rdl.Engine.Report report, bool forPrint)
        {
            //_body.AppendLine("<div style=\"height:" + report.Width + "pt; width:" + report.Width + "pt;\">");
            _styles.Length = 0;
            _script.Length = 0;

            _plusGif = GetImageUrl("", "resource", "plus.gif");
            _minusGif = GetImageUrl("", "resource", "minus.gif");

            _sourceReport = report;

            int styleTop = AddStyles(report, 0);
            RecurseAddStyles(report.PageHeaderContainer, 0, ref styleTop);
            RecurseAddStyles(report.PageFooterContainer, 0, ref styleTop);
            RecurseAddStyles(report.BodyContainer, 0, ref styleTop);
            //int styleBase = AddStyles(report);

            RenderBody(forPrint);

            //_body.AppendLine("</div>");
        }

        public void RenderBody(bool forPrint)
        {
            _body.Length = 0;

            _sourceReport.SetSizes(false);

            RecurseRender(_sourceReport, _body, _sourceReport.PageHeaderContainer, 1, forPrint);
            RecurseRender(_sourceReport, _body, _sourceReport.BodyContainer, 1, forPrint);
            RecurseRender(_sourceReport, _body, _sourceReport.PageFooterContainer, 1, forPrint);
        }

        public string Body
        {
            get
            {
                return _body.ToString();
            }
        }

        public string Style
        {
            get
            {
                _styles.AppendLine("div {");
                _styles.AppendLine(" position: relative;");
                //_styles.AppendLine(" overflow: hidden;");
                _styles.AppendLine(" margin: 0;");
                _styles.AppendLine(" padding: 0;");
                _styles.AppendLine(" word-break: break-all;");
                _styles.AppendLine("}");
                _styles.AppendLine("table {");
                _styles.AppendLine(" position: relative;");
                //_styles.AppendLine(" overflow: hidden;");
                _styles.AppendLine(" margin: 0;");
                _styles.AppendLine(" padding: 0;");
                _styles.AppendLine("}");
                _styles.AppendLine("tr {");
                _styles.AppendLine(" margin: 0;");
                _styles.AppendLine(" padding: 0;");
                _styles.AppendLine("}");
                _styles.AppendLine("td {");
                _styles.AppendLine(" margin: 0;");
                _styles.AppendLine(" padding: 0;");
                _styles.AppendLine(" vertical-align: top;");
                _styles.AppendLine("}");
                return _styles.ToString();
            }
        }

        public string Script
        {
            get
            {
                return _script.ToString();
            }
        }

        public Rdl.Engine.Report SourceReport
        {
            get { return _sourceReport; }
        }

        protected int RecurseRender(Rdl.Engine.Report rpt, StringBuilder body, Element elmt, int level, bool forPrint)
        {
            StringBuilder bodyPart = new StringBuilder();
            bool hasAction = false;

            string style = string.Empty;
            //BoxStyle bs = rpt.StyleList[elmt.StyleIndex];

            if (elmt.ReportElement is Rdl.Engine.Report)
            {
                rpt = (Rdl.Engine.Report)elmt.ReportElement;
            }
            if (elmt is Container)
            {
                Container c = elmt as Container;
                if (!c.IsVisible)
                    return 0;
            }
            if ((elmt is TextElement || elmt is FixedContainer || elmt is ImageElement || elmt is ChartElement))
            {
                style += "width: " + ElementWidth(rpt, elmt) + "pt;";
            }
            if ((elmt is TextElement || elmt is FixedContainer || elmt is ImageElement || elmt is ChartElement))
            {
                style += "height: " + ElementHeight(rpt, elmt) + "pt;";
            }
            if (elmt._imageIndex >= 0 && !(elmt is ImageElement))
            {
                style += "background-image: url(" + GetImageUrl(elmt.Name, "StaticImage", elmt._imageIndex.ToString()) + ");";
                style += "background-repeat: " + rpt.ImageList[elmt._imageIndex].ImageRepeat.ToString() + ";";
            }
            if (elmt.Parent != null && elmt.Parent is Rdl.Render.FixedContainer)
            {
                decimal top = elmt.Top;
                decimal left = elmt.Left;
                style +=
                    "top: " + top + "pt;" +
                    "left: " + left + "pt;";
                // 12/26/2007 This line caused problems with tables.
                //if (!elmt.MatchParentHeight) 
                    style += "position: absolute;";
            }
            if (elmt is TextElement && ((TextStyle)elmt.Style).TextAlign == Rdl.Engine.Style.TextAlignEnum.General)
            {
                TextElement te = elmt as TextElement;
                decimal val;

                if (te.Text.Length > 0 && 
                        decimal.TryParse(te.Text, 
                            System.Globalization.NumberStyles.AllowCurrencySymbol |
                                System.Globalization.NumberStyles.Number, 
                            System.Globalization.CultureInfo.CurrentCulture, 
                            out val ))
                    style += "text-align: right;";
                else
                    style += "text-align: left;";
            }

            if (elmt is FlowContainer && ((FlowContainer)elmt).FlowDirection == FlowContainer.FlowDirectionEnum.LeftToRight)
            {
                int elements = 0;
                bodyPart.AppendLine(Spaces(level) + "<table cellpadding=\"0\" cellspacing=\"0\" " +
                    "id=\"" + elmt.Name + "\" " +
                    ((elmt.RenderedStyleIndex >= 0) ? "class=\"Report_style" + elmt.RenderedStyleIndex + "\" " : string.Empty) +
                    "style=\"" + style + "\"" +
                    ">" +
                    "<tr>");

                foreach (Element child in ((Container)elmt).Children)
                {
                    bodyPart.AppendLine(Spaces(level + 1) + "<td>");
                    elements += RecurseRender(rpt, bodyPart, child, level + 2, forPrint);
                    bodyPart.AppendLine(Spaces(level + 1) + "</td>");
                }

                bodyPart.AppendLine(Spaces(level) + "</tr></table>");
                if (elements > 0)
                    body.Append(bodyPart.ToString());
                return elements;
            }

            if (elmt is Container || elmt is TextElement || elmt is ImageElement || elmt is ChartElement)
            {
                string divTag =
                    "<div " +
                    "id=\"" + elmt.Name + "\" " +
                    ((elmt.RenderedStyleIndex >= 0) ? "class=\"Report_style" + elmt.RenderedStyleIndex.ToString() + "\" " : string.Empty) +
                    "style=\"" + style + "\"";
                if (elmt is TextElement && ((TextElement)elmt).IsToggle)
                    divTag += " stateToggle=\"" + ((TextElement)elmt).ToggleState.ToString() + "\"";
                divTag += ">";
                bodyPart.AppendLine(Spaces(level) + divTag);
            }

            if (elmt is TextElement)
            {
                TextElement te = elmt as TextElement;

                if (te.IsToggle && !forPrint)
                {
                    bodyPart.AppendLine(Spaces(level + 1) +
                        "<a href=\"javascript:{}\" onclick=\"javascript:ToggleState('" + te.Name + "');\">");
                    bodyPart.AppendLine(Spaces(level + 1) +
                        "<img id=\"" + te.Name + "_img\" src=" + ((te.ToggleState == TextElement.ToggleStateEnum.open) ? _minusGif : _plusGif) + " border=\"0\" style=\"float: left;\" />");
                    bodyPart.AppendLine(Spaces(level + 1) + "</a>");
                }
            }

            if (elmt is ActionElement)
            {
                ActionElement ae = (ActionElement)elmt;
                if (ae.DrillThroughReportName != null)
                {
                    bodyPart.AppendLine(Spaces(level + 1) +
                        "<a href=\"javascript:{}\" onclick=\"javascript:Action('" + ae.Name + "');\">");
                    hasAction = true;
                }
                else if (ae.Hyperlink != null)
                {
                    bodyPart.AppendLine(Spaces(level + 1) +
                        "<a href=\"" + ae.Hyperlink + "\" >");
                    hasAction = true;
                }
                else if (ae.BookmarkLink != null)
                {
                    bodyPart.AppendLine(Spaces(level + 1) +
                        "<a href=\"#" + ae.BookmarkLink + "\" >");
                    hasAction = true;
                }
            }

            if (elmt is TextElement)
            {
                TextElement te = elmt as TextElement;

                string text = te.Text;
                bodyPart.AppendLine(Spaces(level) + text);
            }

            if (elmt is ImageElement)
            {
                ImageElement ie = elmt as ImageElement;

                bodyPart.AppendLine(Spaces(level) +
                    "<img id=\"img_" + elmt.Name + "\" alt=\"\" border=\"0\" src=\"\">");
                _script.AppendLine(Spaces(level) +
                    "document.getElementById('img_" + elmt.Name + "').src = " +
                    GetImageUrl(elmt.Name, "SizedImage", elmt._imageIndex.ToString()) + ";" 
                    );
            }

            if (elmt is ChartElement)
            {
                _charts.Add(elmt.Name, (ChartElement)elmt);
                bodyPart.AppendLine(Spaces(level) +
                    "<img id=\"img_" + elmt.Name + "\" alt=\"\" border=\"0\" src=\"\">");
                _script.AppendLine(Spaces(level) +
                    "document.getElementById('img_" + elmt.Name + "').src = " +
                    GetImageUrl(elmt.Name, "Chart", elmt.Name) + ";" 
                    );
            }

            if (elmt is Container)
            {
                if (elmt.Height == 0)
                    return 0;
                int elements = 0;
                foreach (Element child in ((Container)elmt).Children)
                    elements += RecurseRender(rpt, bodyPart, child, level + 1, forPrint);
            }

            if (hasAction)
            {
                bodyPart.Append("</a>");
            }

            if (elmt is Container || elmt is TextElement || elmt is ImageElement || elmt is ChartElement)
            {
                bodyPart.AppendLine(Spaces(level) +
                    "</div>");
            }

            body.Append(bodyPart.ToString());
            return 1;
        }

        public Dictionary<string, ChartElement> Charts
        {
            get { return _charts; }
        }

        private string Spaces(int level)
        {
            return string.Empty.PadRight(level << 1);
            //return string.Empty;
        }

        private decimal ElementWidth(Rdl.Engine.Report rpt, Element elmt)
        {
            decimal width = elmt.Width;
            BoxStyle bs = rpt.StyleList[elmt.StyleIndex];
            if (bs.BorderStyle.Left != Rdl.Engine.BorderStyle.BorderStyleEnum.None)
                width -= bs.BorderWidth.Left.points;
            if (bs.BorderStyle.Right != Rdl.Engine.BorderStyle.BorderStyleEnum.None)
                width -= bs.BorderWidth.Right.points;
            width -= bs.PaddingLeft.points;
            width -= bs.PaddingRight.points;
            return width;
        }

        private decimal ElementHeight(Rdl.Engine.Report rpt, Element elmt)
        {
            decimal height = elmt.Height;
            BoxStyle bs = rpt.StyleList[elmt.StyleIndex];
            if (bs.BorderStyle.Top != Rdl.Engine.BorderStyle.BorderStyleEnum.None)
                height -= bs.BorderWidth.Top.points;
            if (bs.BorderStyle.Bottom != Rdl.Engine.BorderStyle.BorderStyleEnum.None)
                height -= bs.BorderWidth.Bottom.points;
            height -= bs.PaddingTop.points;
            height -= bs.PaddingBottom.points;
            return height;
        }

        protected int AddStyles(Rdl.Engine.Report rpt, int styleBase)
        {
            for (int i = 0; i < rpt.StyleList.Count; i++)
            {
                AddStyle(rpt.StyleList[i], styleBase + i);
            }
            return rpt.StyleList.Count;
        }

        protected void RecurseAddStyles(Element elmt, int styleBase, ref int styleTop)
        {
            elmt.RenderedStyleIndex = elmt.StyleIndex + styleBase;
            if (elmt.ReportElement is Rdl.Engine.SubReport)
            {
                styleBase = styleTop;
                Rdl.Engine.Report rpt = ((Rdl.Engine.SubReport)elmt.ReportElement).GetSubReport();
                styleTop += AddStyles(rpt, styleBase);
            }

            if (elmt is Container)
                foreach (Element child in ((Container)elmt).Children)
                {
                    RecurseAddStyles(child, styleBase, ref styleTop);
                }
        }

        protected void AddStyle(BoxStyle style, int index)
        {
            _styles.AppendLine(".Report_style" + index.ToString() + " {");
            _styles.AppendLine("    border-left: " +
                style.BorderWidth.Left.points.ToString() + "pt " +
                style.BorderStyle.Left.ToString() + " " +
                style.BorderColor.Left + ";");
            _styles.AppendLine("    border-right: " +
                style.BorderWidth.Right.points.ToString() + "pt " +
                style.BorderStyle.Right.ToString() + " " +
                style.BorderColor.Right + ";");
            _styles.AppendLine("    border-top: " +
                style.BorderWidth.Top.points.ToString() + "pt " +
                style.BorderStyle.Top.ToString() + " " +
                style.BorderColor.Top + ";");
            _styles.AppendLine("    border-bottom: " +
                style.BorderWidth.Bottom.points.ToString() + "pt " +
                style.BorderStyle.Bottom.ToString() + " " +
                style.BorderColor.Bottom + ";");
            _styles.AppendLine("    Padding: " +
                style.PaddingTop.points.ToString() + "pt " +
                style.PaddingRight.points.ToString() + "pt " +
                style.PaddingBottom.points.ToString() + "pt " +
                style.PaddingLeft.points.ToString() + "pt;");
            _styles.AppendLine("    background-color: " +
                style.BackgroundColor + ";");
            if (style.BackgroundImage != null)
            {
                //_styles.AppendLine("    background-image: URL(" + GetImageUrl("Image", style.BackgroundImage.Name) + ");");
                _styles.AppendLine("    background-repeat: " +
                    style.BackgroundImage.ImageRepeat.ToString() + ";");
            }
            _styles.AppendLine("    color: " +
                style.Color + ";" );
            if (style is TextStyle)
            {
                TextStyle ts = style as TextStyle;

                _styles.AppendLine("    font-style: " +
                    ts.FontStyle.ToString() + ";");
                _styles.AppendLine("    font-family: " +
                    ts.FontFamily + ";");
                _styles.AppendLine("    font-size: " +
                    ts.FontSize.points.ToString() + "pt;");
                _styles.AppendLine("    font-weight: " +
                    ts.FontWeight.ToString().Replace("_", "") + ";");
                _styles.AppendLine("    text-decoration: " +
                    ts.TextDecoration.ToString() + ";");
                if (ts.TextAlign.ToString().ToLower() != "general")
                    _styles.AppendLine("    text-align: " +
                        ts.TextAlign.ToString() + ";");
                _styles.AppendLine("    vertical-align: " +
                    ts.VerticalAlign.ToString() + ";");
                _styles.AppendLine("    line-height: " +
                    ts.LineHeight.points.ToString() + "pt;" );
                _styles.AppendLine("    direction: " +
                    ts.Direction.ToString() + ";");
                _styles.AppendLine("    unicode-bidi: " +
                    ts.UnicodeBiDi.ToString() + ";");
            }

            _styles.AppendLine("}");
        }

        string GetImageUrl(string elementName, string source, string imageName)
        {
            ImageUrlArgs imageArgs = new ImageUrlArgs();
            imageArgs.ElementName = elementName;
            imageArgs.Source = source;
            imageArgs.ImageName = imageName;
            if (ImageUrl != null)
                ImageUrl(this, imageArgs);
            //return "expression('" + imageArgs.Url + "' + '&width=' + this.clientWidth + '&height=' + this.clientHeight)";
            return imageArgs.Url;
        }
    }
}
