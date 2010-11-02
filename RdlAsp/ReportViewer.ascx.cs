using System;
using System.Data;
using System.Configuration;
using System.Collections;
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
    public partial class ReportViewer : System.Web.UI.WebControls.WebControl, ICallbackEventHandler
    {
        protected string _RenderType = "html";
        protected Rdl.Render.RenderToHtml _renderedReport = null;

        protected void Page_Load(object sender, EventArgs e)
        {
            _renderedReport = (Rdl.Render.RenderToHtml)Page.Session["RenderedReport"];

            if (_renderedReport != null)
            {
                Page.Header.Controls.Add(new LiteralControl(
                    "<style type='text/css'>\n" +
                    _renderedReport.Style +
                    "</style>"));

                Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "ReportScript", 
                    "<script language=\"javascript\">\n" +
                    _renderedReport.Script +
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
            if (_renderedReport != null)
                body = body.Replace("<%report%>", _renderedReport.Body);
            else
                body = body.Replace("<%report%>", string.Empty);
            writer.Write(body);
        }

        public void SetReport(Rdl.Render.RenderToHtml renderedReport)
        {
            _renderedReport = renderedReport;
            Page.Session["RenderedReport"] = _renderedReport;

            Page_Load(null, null);
        }

        public string GetCallbackResult()
        {
            return null;
        }

        public void RaiseCallbackEvent(string eventArgument)
        {
            string[] args = eventArgument.Split(null);

            Rdl.Render.RenderToHtml htmlReport = (Rdl.Render.RenderToHtml)Context.Session["RenderedReport"];

            // Find the named text element and set the toggle state.
            Rdl.Render.TextElement te = (Rdl.Render.TextElement)htmlReport.SourceReport.FindNamedElement(args[1]);

            if (te != null)
                te.ToggleState = (Rdl.Render.TextElement.ToggleStateEnum)Enum.Parse(typeof(Rdl.Render.TextElement.ToggleStateEnum), args[2]);
        }

        #endregion

        private string html = @"
<script language=""javascript"">
function ToggleStateData(value)
{
}

function ToggleState(textBoxName, toggleState)
{
    ToggleStateCallback( textBoxName + ' ' + toggleState );
}

function ExportOnChange(selectedIndex) 
{
    var reportKey = document.getElementById(""LabelReportID"").innerText;
    if (selectedIndex == 1)
    {
        var url = ""PdfExport.rdlx"";
        window.open(url,""_blank"","""");
        window.opener=top;
    }
    if (selectedIndex == 2)
    {
        var url = ""XlsExport.rdlx"";
        window.open(url,""_blank"","""");
        window.opener=top;
    }
    if (selectedIndex == 3)
    {
        var url = ""TxtExport.rdlx"";
        window.open(url,""_blank"","""");
        window.opener=top;
    }
    if (selectedIndex == 4)
    {
        var url = ""CsvExport.rdlx"";
        window.open(url,""_blank"","""");
        window.opener=top;
    }
    document.getElementById(""Export"").selectedIndex = 0;
}    

function printClick()
{
    var url = ""Print.rdlx"";
    window.open(url,""_blank"","""");
    window.opener=top;
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
<div style=""display: inline; position: static;"">
    <%report%>
</div>
";
    }
}