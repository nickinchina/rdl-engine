using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Web.SessionState;
using System.IO;

namespace RdlAsp
{
    public class ReportServer : IHttpHandler, IReadOnlySessionState
    {
        #region IHttpHandler Members
        private Rdl.Render.RenderToHtml _renderedReport;
        private string _sessionId;
        internal static string _extension = 
            (ConfigurationManager.AppSettings["CustomExtension"] == null) ? "rdlx" : 
            ConfigurationManager.AppSettings["CustomExtension"];

        public bool IsReusable
        {
            get { return true; }
        }

        public void ProcessRequest(HttpContext context)
        {
            _sessionId = context.Request.QueryString["ReportSessionID"];
            _renderedReport = (Rdl.Render.RenderToHtml)context.Session[_sessionId];
            string path = context.Request.Url.LocalPath;
            path = path.Substring(context.Request.Url.LocalPath.LastIndexOf('/')+1);
            if (path.IndexOf('.') >= 0)
                path = path.Substring(0, path.IndexOf('.'));
            switch (path)
            {
                case "image":
                    string name = context.Request.QueryString["name"];
                    if (context.Request.QueryString["source"] == "resource")
                    {
                        if (name.Contains("."))
                            name = name.Substring(0, name.IndexOf('.'));
                        switch (context.Request.QueryString["name"])
                        {
                            case "plus.gif":
                                context.Response.ContentType = "image/gif";
                                RdlAsp.plus.Save(context.Response.OutputStream, RdlAsp.plus.RawFormat);
                                break;
                            case "minus.gif":
                                context.Response.ContentType = "image/gif";
                                RdlAsp.minus.Save(context.Response.OutputStream, RdlAsp.minus.RawFormat);
                                break;
                            case "calendar.bmp":
                                context.Response.ContentType = "image/bmp";
                                MemoryStream ms = new MemoryStream();
                                RdlAsp.calendar.Save(ms, RdlAsp.calendar.RawFormat);
                                ms.WriteTo(context.Response.OutputStream);
                                break;
                        }
                    }
                    else if (context.Request.QueryString["source"] == "SizedImage")
                    {
                        Rdl.Render.ImageData id = _renderedReport.SourceReport.ImageList[int.Parse(name)];
                        context.Response.ContentType = id.MimeType;
                        if (context.Request.QueryString["width"] != null)
                            context.Response.BinaryWrite(
                                id.GetSizedImageData(int.Parse(context.Request.QueryString["width"]),
                                    int.Parse(context.Request.QueryString["height"])));
                        else
                            context.Response.BinaryWrite(id.GetImageData());
                        //id.imageData.Save(context.Response.OutputStream, id.imageData.RawFormat);
                    }
                    else if (context.Request.QueryString["source"] == "Chart")
                    {
                        Rdl.Render.ChartElement cc = _renderedReport.Charts[name];
                        if (cc != null)
                        {
                            MemoryStream ms = new MemoryStream();

                            System.Drawing.Image img = cc.RenderChart( int.Parse(context.Request.QueryString["width"]),
                                int.Parse(context.Request.QueryString["height"]), 1, 1);
                            if (img != null)
                            {
                                img.Save(ms, System.Drawing.Imaging.ImageFormat.Gif);

                                context.Response.BinaryWrite(ms.ToArray());
                            }
                        }
                    }
                    break;
                case "XlsExport":
                case "XlsExportAll":
                    {
                        byte[] xlsReport = null;
                        try
                        {
                            Rdl.Render.RenderToXls xlsRender = new Rdl.Render.RenderToXls();
                            xlsReport = xlsRender.Render(_renderedReport.SourceReport, path=="XlsExportAll");
                        }
                        catch (System.Security.SecurityException)
                        {
                            context.Response.Write("<html xmlns=\"http://www.w3.org/1999/xhtml\" >");
                            context.Response.Write("<body>");
                            context.Response.Write("XlsRender threw a security exception<br/>");
                            context.Response.Write("XlsRender currently writes temporary files which may not be available in hosting environments with limited rights<br/>");
                            context.Response.Write("</body>");
                            break;
                        }
                        context.Response.ContentType = "application/excel";
                        context.Response.AddHeader("content-disposition", "attachment;filename=report.xls");
                        context.Response.AddHeader("Content-Length", xlsReport.Length.ToString());
                        context.Response.BinaryWrite(xlsReport);
                        context.Response.Flush();
                    }
                    break;
                case "PdfExport":
                    {
                        string pdfReport = null;
                        try
                        {
                            Rdl.Render.PageRender pageRender = new Rdl.Render.PageRender();
                            pageRender.Render(_renderedReport.SourceReport);
                            Rdl.Render.RenderPagesToPdf pdfRender = new Rdl.Render.RenderPagesToPdf();
                            pdfReport = pdfRender.Render(_renderedReport.SourceReport, pageRender);
                        }
                        catch (System.Security.SecurityException)
                        {
                            context.Response.Write("<html xmlns=\"http://www.w3.org/1999/xhtml\" >");
                            context.Response.Write("<body>");
                            context.Response.Write("PdfRender threw a security exception<br/>");
                            context.Response.Write("PdfRender currently required WinGdi which may not be available in hosting environments with limited rights<br/>");
                            context.Response.Write("</body>");
                            break;
                        }
                        context.Response.ContentType = "application/pdf";
                        context.Response.AddHeader("content-disposition", "filename=report.pdf");
                        context.Response.AddHeader("Content-Length", pdfReport.Length.ToString());
                        context.Response.Write(pdfReport);
                        context.Response.Flush();
                    }
                    break;
                case "TxtExport":
                    {
                        context.Response.ContentType = "application/text";
                        context.Response.AddHeader("content-disposition", "attachment;filename=report.txt");
                        Rdl.Render.RenderToText textRender = new Rdl.Render.RenderToText();
                        string textReport = textRender.Render(_renderedReport.SourceReport);
                        context.Response.AddHeader("Content-Length", textReport.Length.ToString());
                        context.Response.Write(textReport);
                        context.Response.Flush();
                    }
                    break;
                case "CsvExport":
                    {
                        context.Response.ContentType = "application/text";
                        context.Response.AddHeader("content-disposition", "attachment;filename=report.txt");
                        Rdl.Render.RenderToCsv textRender = new Rdl.Render.RenderToCsv();
                        string textReport = textRender.Render(_renderedReport.SourceReport);
                        context.Response.AddHeader("Content-Length", textReport.Length.ToString());
                        context.Response.Write(textReport);
                        context.Response.Flush();
                    }
                    break;
                case "Print":
                    {
                        Rdl.Render.PageRender pageRender = new Rdl.Render.PageRender();
                        pageRender.Render(_renderedReport.SourceReport);
                        Rdl.Render.RenderPagesToHtml htmlRender = new Rdl.Render.RenderPagesToHtml();
                        htmlRender.ImageUrl += new Rdl.Render.RenderToHtml.ImageUrlEventHandler(htmlRender_ImageUrl);
                        htmlRender.Render(_renderedReport.SourceReport, pageRender, true);
                        string html = _printHtml;
                        html = html.Replace("<!--style-->", htmlRender.Style);
                        html = html.Replace("<!--script-->", htmlRender.Script);
                        html = html.Replace("<!--body-->", htmlRender.Body);
                        context.Response.Write(html);
                    }
                    break;
            }
        }

        void htmlRender_ImageUrl(object sender, Rdl.Render.RenderToHtml.ImageUrlArgs args)
        {
            //args.Url = "'image." + _extension + "?source=" + HttpUtility.UrlEncode(args.Source) +
            //    "&name=" + HttpUtility.UrlEncode(args.ImageName) +
            //    "&reportSessionID=" + HttpUtility.UrlEncode(_sessionId.ToString()) + "'";

            args.Url = "'image." + _extension + "?source=" + HttpUtility.UrlEncode(args.Source) +
                "&name=" + HttpUtility.UrlEncode(args.ImageName) +
                "&reportSessionID=" + HttpUtility.UrlEncode(_sessionId.ToString());
            if (args.Source == "SizedImage" || args.Source == "Chart")
                args.Url += "&width=' + document.getElementById('" + args.ElementName + "').clientWidth + " +
                    "'&height=' + document.getElementById('" + args.ElementName + "').clientHeight";
            else
                args.Url += "'";
        }


        private string _printHtml = @"
<!DOCTYPE html PUBLIC ""-//W3C//DTD XHTML 1.0 Transitional//EN"" ""http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd"">
<html xmlns=""http://www.w3.org/1999/xhtml"" >
<head runat=""server"">
    <title>Untitled Page</title>
    <style type=""text/css"">
    <!--style-->
    </style>
    <script language=""javascript"">
    function body_onload()
    {
    window.print();
    }

    function imageUrl(elementName, source, name)
    {
        var src = 'image." + _extension + @"?source=' + source + 
                    '&name=' + name;

        // Get the size of the DIV element containing the image
        var elmt = document.getElementById(elementName);
        src += '&width=' + elmt.clientWidth +
                '&height=' + elmt.clientHeight;
        
        // Set the SRC for the image 
        elmt = document.getElementById('img_' + elementName);
        elmt.src = src;
        return src;
    }
    </script>
</head>
<body onload=""body_onload();"">
<!--body-->
</body>
    <script language=""javascript"">
    <!--script-->
    </script>
</html>
";

        #endregion
    }
}
