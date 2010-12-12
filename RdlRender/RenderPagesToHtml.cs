using System;
using System.Collections.Generic;
using System.Text;

namespace RdlRender
{
    public class RenderPagesToHtml : RenderToHtml
    {
        public void Render(Container report, PageRender pageRender)
        {
            _styles.AppendLine("@page {");
            _styles.AppendLine("    size: " + (pageRender.PageWidth / 72m).ToString() + " in " +
                (pageRender.PageHeight / 72m).ToString() + " in;");
            _styles.AppendLine("    margin-left: " + (pageRender.LeftMargin / 72m).ToString() + " in;");
            _styles.AppendLine("    margin-right: " + (pageRender.RightMargin / 72m).ToString() + " in;");
            _styles.AppendLine("    margin-top: " + (pageRender.TopMargin / 72m).ToString() + " in;");
            _styles.AppendLine("    margin-bottom: " + (pageRender.BottomMargin / 72m).ToString() + " in;");
            _styles.AppendLine("    padding: 0 in;");
            _styles.AppendLine("}");

            _styles.AppendLine(".pagediv {");
            //_styles.AppendLine("    width: " + pageRender.PageWidth.ToString() + "pt;");
            //_styles.AppendLine("    height: " + pageRender.PageHeight.ToString() + "pt;");
            _styles.AppendLine("}");

            decimal top = 0;
            for (int pageNum = 0; pageNum < pageRender.Pages.Count; pageNum++)
            {
                Page page = pageRender.Pages[pageNum];

                _body.Add("<div " + //class=\"pagediv\" " +
                    "style=\"position: static;" + ((pageNum + 1 < pageRender.Pages.Count) ? "page-break-after: always;" : string.Empty) + "\" " +
                    "title=\"page " + (pageNum+1).ToString() + "\">");

                foreach (RdlRender.Element elmt in pageRender.Pages[pageNum].Box.Children)
                    RecurseRender(elmt, 1);

                _body.Add("</div>");
                top += pageRender.PageHeight;
            }

            AddStyles(report);
        }
    }
}
