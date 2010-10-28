using System;
using System.Collections.Generic;
using System.Text;

namespace RdlRender
{
    public class RenderToHtml
    {
        protected List<string> _body = new List<string>();
        protected static string _et = string.Empty;
        protected StringBuilder _styles = new StringBuilder();
        protected StringBuilder _script = new StringBuilder();

        public void Render(Element report)
        {
            //_body.Add("<div style=\"height:" + report.Width + "pt; width:" + report.Width + "pt;\">");

            RecurseRender(report, 1);

            AddStyles(report);

            //_body.Add("</div>");
        }

        public string Body
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                foreach (string s in _body)
                    sb.AppendLine(s);
                return sb.ToString();
            }
        }

        public string Style
        {
            get
            {
                _styles.AppendLine("div {");
                _styles.AppendLine(" position: relative;");
                _styles.AppendLine(" overflow: hidden;");
                _styles.AppendLine(" margin: 0;");
                _styles.AppendLine(" padding: 0;");
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

        protected void RecurseRender(Element elmt, int level)
        {
            if (elmt is Container || elmt is TextElement)
            {
                string divTag =
                    "<div " +
                    "id=\"" + elmt.Name + "\" " +
                    ((elmt.StyleIndex >= 0) ? "class=\"Report_style" + elmt.StyleIndex + "\" " : string.Empty) +
                    "style=\"";
                if (elmt is TextElement || elmt is Container)
                    divTag +=
                        "width: " + elmt.Width + "pt;";
                if (elmt is TextElement || elmt is FixedContainer)
                    divTag +=
                        "height: " + elmt.Height + "pt;";
                if (elmt.Parent != null && elmt.Parent is RdlRender.FixedContainer)
                    divTag +=
                        "top: " + elmt.Top + "pt;" +
                        "left: " + elmt.Left + "pt;" +
                        "position: absolute;";
                if (elmt is Container && ((Container)elmt).Toggle != null && ((Container)elmt).Toggle.ToggleState == TextElement.ToggleStateEnum.closed)
                    divTag += "display: none;";
                divTag += "\">";
                _body.Add(_et.PadRight(level << 1) + divTag);
            }

            if (elmt is TextElement)
            {
                TextElement te = elmt as TextElement;

                if (te.ToggleList.Count > 0)
                {
                    string name = te.Name.Replace('-', '_') + "_onclick";
                    _body.Add(_et.PadRight(level << 1) +
                        "<a href=\"javascript:{}\" onclick=\"javascript:" + name + "(this, '" + te.Name + "_img');\" togglestate=\"" + 
                        te.ToggleState.ToString() +
                        "\">");
                    _body.Add(_et.PadRight(level << 1) +
                        "<img id=\"" + te.Name + "_img\" src=\"" + ((te.ToggleState==TextElement.ToggleStateEnum.open)?"minus.gif":"plus.gif") + "\" border=\"0\" />");

                    _script.AppendLine("function " + name + "(element, img_id)");
                    _script.AppendLine("{");
                    _script.AppendLine("   element.togglestate = ((element.togglestate==\"open\")?\"closed\":\"open\");");
                    _script.AppendLine("   document.getElementById(img_id).src = ((element.togglestate==\"open\")?\"minus.gif\":\"plus.gif\");");
                    _script.AppendLine("   display = (element.togglestate==\"open\")?\"block\":\"none\";");
                    foreach (Container b in te.ToggleList)
                    {
                        _script.AppendLine("   document.getElementById(\"" + b.Name + "\").style.display = display;");
                    }
                    _script.AppendLine("    ToggleState('" + te.Name + "',  element.togglestate, '');");
                    _script.AppendLine("}");
                }

                string text = te.Text;
                _body.Add(_et.PadRight(level<<1) + text);

                if (te.ToggleList.Count > 0)
                {
                    _body.Add(_et.PadRight(level << 1) +
                        "</a>");
                }
            }

            if (elmt is Container)
                foreach (Element child in ((Container)elmt).Children)
                    RecurseRender(child, level+1);

            if (elmt is Container || elmt is TextElement)
            {
                _body.Add(_et.PadRight(level<<1) +
                    "</div>");
            }

        }

        protected void AddStyles(Element elmt)
        {
            for (int i = 0; i < elmt.BaseStyleList.Count; i++)
            {
                AddStyle(elmt.BaseStyleList[i], i);
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
            _styles.AppendLine("    background-color: " +
                style.BackgroundColor + ";");
            if (style.BackgroundImage != null)
            {
                _styles.AppendLine("    background-image: URL(Data:" +
                    style.BackgroundImageType + ";base64," +
                    Convert.ToBase64String(style.BackgroundImage) + ");");
                _styles.AppendLine("    background-repeat: " +
                    style.BackgroundImageType + ";");
            }
            _styles.AppendLine("    color: " +
                style.Color + ";" );
            _styles.AppendLine("    Padding: " +
                style.PaddingTop.points.ToString() + "pt, " +
                style.PaddingRight.points.ToString() + "pt, " +
                style.PaddingBottom.points.ToString() + "pt, " +
                style.PaddingLeft.points.ToString() + "pt;");
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
    }
}
