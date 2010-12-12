using System;
using System.Collections.Generic;
using System.Text;

namespace Rdl.Render
{
    public class RenderPagesToHtml : RenderToHtml
    {
        public void Render(Rdl.Render.GenericRender report, PageRender pageRender, bool forPrint)
        {
            _sourceReport = report;
            _styles.AppendLine("@page {");
            _styles.AppendLine("    size: " + (pageRender.PageWidth / 72m).ToString() + "in " +
                (pageRender.PageHeight / 72m).ToString() + "in;");
            _styles.AppendLine("    margin-left: " + (pageRender.LeftMargin / 72m).ToString() + "in;");
            _styles.AppendLine("    margin-right: " + (pageRender.RightMargin / 72m).ToString() + "in;");
            _styles.AppendLine("    margin-top: " + (pageRender.TopMargin / 72m).ToString() + "in;");
            _styles.AppendLine("    margin-bottom: " + (pageRender.BottomMargin / 72m).ToString() + "in;");
            _styles.AppendLine("    padding: 0in;");
            _styles.AppendLine("}");

            _styles.AppendLine(".pagediv {");
            //_styles.AppendLine("    width: " + pageRender.PageWidth.ToString() + "pt;");
            //_styles.AppendLine("    height: " + pageRender.PageHeight.ToString() + "pt;");
            _styles.AppendLine("}");

            int styleTop = AddStyles(report, 0);
            //RecurseAddStyles(report.PageHeaderContainer, 0, ref styleTop);
            //RecurseAddStyles(report.PageFooterContainer, 0, ref styleTop);
            //RecurseAddStyles(report.BodyContainer, 0, ref styleTop);
            //for (int pageNum = 0; pageNum < pageRender.Pages.Count; pageNum++)
            //    RecurseAddStyles(pageRender.Pages[pageNum], 0, ref styleTop);

            decimal top = 0;
            for (int pageNum = 0; pageNum < pageRender.Pages.Count; pageNum++)
            {
                Page page = pageRender.Pages[pageNum];

                _body.AppendLine("<div " + //class=\"pagediv\" " +
                    "style=\"position: static;" + ((pageNum + 1 < pageRender.Pages.Count) ? "page-break-after: always;" : string.Empty) + "\" " +
                    "title=\"page " + (pageNum+1).ToString() + "\">");

                foreach (Rdl.Render.Element elmt in pageRender.Pages[pageNum].Children)
                    RecurseRender(report, _body, elmt, 1, forPrint);

                _body.AppendLine("</div>");
                top += pageRender.PageHeight;
            }
        }
    }
}
