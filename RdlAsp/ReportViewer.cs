using System;
using System.Data;
using System.Configuration;
using System.Collections.Generic;
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
    /// <summary>
    /// A ASP.NET viewer control used to view RDL reports.
    /// </summary>
    public partial class ReportViewer : System.Web.UI.WebControls.WebControl, ICallbackEventHandler
    {
        protected string _RenderType = "html";
        protected Rdl.Render.RenderToHtml _htmlReport = null;

        protected void Page_Load(object sender, EventArgs e)
        {
            _htmlReport = (Rdl.Render.RenderToHtml)Page.Session["RenderedReport"];

            if (_htmlReport != null)
            {
                Page.Header.Controls.Add(new LiteralControl(
                    "<style type='text/css'>\n" +
                    _htmlReport.Style +
                    "</style>"));

                Page.ClientScript.RegisterStartupScript(this.GetType(), "ReportScript", 
                    "<script language=\"javascript\">\n" +
                    _htmlReport.Script +
                    "</script>");
            }

            string cbReference = Page.ClientScript.GetCallbackEventReference(
                this, "arguments", "ToggleStateData", "context");
            Page.ClientScript.RegisterClientScriptBlock(
                this.GetType(), "ToggleState",
                "function ToggleStateCallback(arguments, context) {" + cbReference + "}", true);
        }

        #region ICallbackEventHandler Members

        protected override void RenderContents(HtmlTextWriter writer)
        {
            string body = html;
            if (_htmlReport != null)
                body = body.Replace("<%report%>", _htmlReport.Body);
            else
                body = body.Replace("<%report%>", string.Empty);
            writer.Write(body);
        }

        /// <summary>
        /// Sets the report to view in the control
        /// </summary>
        /// <param name="report"></param>
        public void SetReport(Rdl.Engine.Report report)
        {
            // Render the report to streaming html
            _htmlReport = new Rdl.Render.RenderToHtml();
            _htmlReport.ImageUrl += new Rdl.Render.RenderToHtml.ImageUrlEventHandler(htmlRender_ImageUrl);
            _htmlReport.Render(report);

            Page.Session["RenderedReport"] = _htmlReport;

            Page_Load(null, null);
        }

        public string GetCallbackResult()
        {
            return _htmlReport.Body;
        }

        void htmlRender_ImageUrl(object sender, Rdl.Render.RenderToHtml.ImageUrlArgs args)
        {
            args.Url = "image." + ReportServer._extension + "?source=" + HttpUtility.UrlEncode(args.Source) +
                "&name=" + HttpUtility.UrlEncode(args.ImageName);
            if (args.Source == "SizedImage" || args.Source == "Chart")
                args.Url += "&width=' + document.getElementById('" + args.ElementName + "').clientWidth + " +
                    "'&height=' + document.getElementById('" + args.ElementName + "').clientHeight";
        }

        public void RaiseCallbackEvent(string eventArgument)
        {
            string[] args = eventArgument.Split(null);

            _htmlReport = (Rdl.Render.RenderToHtml)Context.Session["RenderedReport"];

            // Find the named text element and set the toggle state.
            Rdl.Render.TextElement te = (Rdl.Render.TextElement)_htmlReport.SourceReport.BodyContainer.FindNamedElement(args[0]);

            if (te != null)
                te.ToggleState = (Rdl.Render.TextElement.ToggleStateEnum)Enum.Parse(typeof(Rdl.Render.TextElement.ToggleStateEnum), args[1]);

            _htmlReport.RenderBody(false);
        }

        #endregion

        private string html = @"
<script language=""javascript"">
function ToggleStateData(value)
{
    document.getElementById('ReportContentDiv').innerHTML = value;
}

function ToggleState(tb_id)
{
    tb = document.getElementById(tb_id);
    ts = ((tb.getAttribute('stateToggle')=='open')?'closed':'open');
    tb.setAttribute('StateToggle', ts);
    //document.getElementById(tb_id + '_img').src = 'image." + ReportServer._extension + @"?source=resource&name=' + ((ts=='open')?'minus' : 'plus');
    
    //alert( tb + ' ' + tb.id + ' ' + ts );
    //document.recalc(true);
    ToggleStateCallback( tb_id + ' ' + ts );
}

function ExportOnChange(selectedIndex) 
{
    var reportKey = document.getElementById('LabelReportID').innerText;
    if (selectedIndex == 1)
    {
        var url = 'PdfExport." + ReportServer._extension + @"';
        window.open(url,'_blank','');
        window.opener=top;
    }
    if (selectedIndex == 2)
    {
        var url = 'XlsExport." + ReportServer._extension + @"';
        window.open(url,'_blank','');
        window.opener=top;
    }
    if (selectedIndex == 3)
    {
        var url = 'TxtExport." + ReportServer._extension + @"';
        window.open(url,'_blank','');
        window.opener=top;
    }
    if (selectedIndex == 4)
    {
        var url = 'CsvExport." + ReportServer._extension + @"';
        window.open(url,'_blank','');
        window.opener=top;
    }
    document.getElementById('Export').selectedIndex = 0;
}    

function printClick()
{
    var url = 'Print." + ReportServer._extension + @"';
    window.open(url,'_blank','');
    window.opener=top;
}

function imageUrl(elementName, source, name)
{
    var src = 'image." + ReportServer._extension + @"?source=' + source + 
                '&name=' + name;

    // Get the size of the DIV element containing the image
    var elmt = document.getElementById(elementName);
    src += '&width=' + elmt.clientWidth +
            '&height=' + elmt.clientHeight;
    
    // Set the SRC for the image 
    elmt = document.getElementById('img_' + elementName);
    elmt.src = src;
    
    //var img = new Image();
    //img.src = src;
    //return src;
}

</script>

<asp:Label ID=""LabelReportID"" runat=""server"" Text="""" style=""display:none;""></asp:Label>
Export To:<select id=""Export"" onchange=""ExportOnChange(this.selectedIndex)"">
    <option selected=""selected"">---</option>
    <option>PDF</option>
    <option>Excel</option>
    <option>Text</option>
    <option>CSV</option>
</select>
&nbsp;&nbsp;&nbsp;&nbsp;
<input type=""button"" id=""btnPrint"" onclick=""printClick()"" value=""Print..."">
<div id=""ReportContentDiv"" style=""position: static; overflow: visible;"">
    <%report%>
</div>
";
    }
}