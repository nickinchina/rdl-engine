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
    public partial class ReportViewer : System.Web.UI.WebControls.WebControl, ICallbackEventHandler, INamingContainer
    {
        protected string _RenderType = "html";
        protected Rdl.Render.RenderToHtml _htmlReport = null;
        private static int reportIndex = 0;
        private string _reportSessionID = string.Empty;

        protected override void OnLoad(EventArgs e)
        {
            // do something here
            base.OnLoad(e);

            _htmlReport = (Rdl.Render.RenderToHtml)Page.Session[ReportSessionID];
        }


        private string ReportSessionID
        {
            get
            {
                _reportSessionID = ((HiddenField)FindControl("tbReportID")).Value;
                if (_reportSessionID == string.Empty)
                {
                    _reportSessionID = "RenderedReport_" + (reportIndex++).ToString();
                    ((HiddenField)FindControl("tbReportID")).Value = _reportSessionID;
                }
                return _reportSessionID;
            }
        }

        void btnAction_Click(object sender, EventArgs e)
        {
            if (FindControl("tbAction") != null)
            {
                // Get the ID of the element which triggered the action
                string actionID = ((HiddenField)FindControl("tbAction")).Value;
                // Prevent recursion 
                if (actionID != string.Empty)
                {
                    // Find the named text action element.
                    Rdl.Render.ActionElement ae = (Rdl.Render.ActionElement)_htmlReport.SourceReport.BodyContainer.FindNamedElement(actionID);

                    if (ae != null)
                    {
                        // If the action is a drill-through, then load the new report,
                        // set the parameters and open the report.
                        if (ae.DrillThroughReportName != null)
                        {
                            string reportName = ae.DrillThroughReportName;
                            if (!reportName.Contains("\\"))
                                reportName = _htmlReport.SourceReport.Report.ReportPath + reportName;
                            if (!reportName.Contains(".rdl"))
                            {
                                if (File.Exists(reportName + ".rdl"))
                                    reportName += ".rdl";
                                else if (File.Exists(reportName + ".rdlc"))
                                    reportName += ".rdlc";
                            }
                            if (!File.Exists(reportName))
                                throw new Exception("Unable to locate sub report " + reportName);

                            Rdl.Engine.Report rpt = new Rdl.Engine.Report();
                            FileStream fs = new FileStream(reportName,
                                FileMode.Open, FileAccess.Read, FileShare.Read);
                            rpt.Load(fs,
                                _htmlReport.SourceReport.Report.ReportPath);
                            fs.Close();
                            fs.Dispose();

                            foreach (Rdl.Render.ActionElement.ActionParameter parm in ae.DrillThroughParameterList)
                            {
                                rpt.ReportParameters[parm.Name].Value = parm.Value;
                                rpt.ReportParameters[parm.Name].Hidden = true;
                            }
                            Rdl.Render.GenericRender render = rpt.Run();

                            _reportSessionID = "RenderedReport_" + (reportIndex++).ToString();
                            ((HiddenField)FindControl("tbReportID")).Value = _reportSessionID;

                            SetReport(render);
                            return;
                        }
                    }
                }
            }
        }

        protected override void RenderContents(HtmlTextWriter writer)
        {
            base.RenderContents(writer);

            string body = html;
            if (_htmlReport != null)
            {
                body = body.Replace("<%report%>", _htmlReport.Body);
                body = body.Replace("<%buttons%>", htmlButtons);
            }
            else
            {
                body = body.Replace("<%report%>", string.Empty);
                body = body.Replace("<%buttons%>", string.Empty);
            }
            body = body.Replace("<%btnAction%>", FindControl("btnAction").ClientID);
            body = body.Replace("<%tbAction%>", FindControl("tbAction").ClientID);
            body = body.Replace("<%ReportSessionID%>", ReportSessionID);
            writer.Write(body);
        }

        protected override void CreateChildControls()
        {
            base.CreateChildControls();

            // Add a hidden button to handle actions
            Button btnAction = new Button();
            btnAction.ID = "btnAction";
            btnAction.Style.Add(HtmlTextWriterStyle.Display, "none");
            btnAction.Text = "";
            btnAction.Click += new EventHandler(btnAction_Click);
            Controls.Add(btnAction);

            // Add a hidden text box to hold the action elements ID
            HiddenField tbAction = new HiddenField();
            tbAction.ID = "tbAction";
            tbAction.Value = "";
            Controls.Add(tbAction);

            HiddenField tbReportId = new HiddenField();
            tbReportId.ID = "tbReportID";
            tbReportId.Value = "";
            Controls.Add(tbReportId);
        }

        /// <summary>
        /// Sets the report to view in the control
        /// </summary>
        /// <param name="report"></param>
        public void SetReport(Rdl.Render.GenericRender report)
        {
            // Render the report to streaming html
            _htmlReport = new Rdl.Render.RenderToHtml();
            _htmlReport.ImageUrl += new Rdl.Render.RenderToHtml.ImageUrlEventHandler(htmlRender_ImageUrl);
            _htmlReport.Render(report);

            Page.Session[ReportSessionID] = _htmlReport;

//            OnLoad(null);
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
                this, "arguments", "ToggleStateData", "");
            Page.ClientScript.RegisterClientScriptBlock(
                this.GetType(), "ToggleState",
                "function ToggleStateCallback(arguments) {" + cbReference + "}", true);
        }

        void htmlRender_ImageUrl(object sender, Rdl.Render.RenderToHtml.ImageUrlArgs args)
        {
            args.Url = "'image." + ReportServer._extension + "?source=" + HttpUtility.UrlEncode(args.Source) +
                "&name=" + HttpUtility.UrlEncode(args.ImageName) +
                "&reportSessionID=" + HttpUtility.UrlEncode(ReportSessionID.ToString());
            if (args.Source == "SizedImage" || args.Source == "Chart")
                args.Url += "&width=' + document.getElementById('" + args.ElementName + "').clientWidth + " +
                    "'&height=' + document.getElementById('" + args.ElementName + "').clientHeight";
            else
                args.Url += "'";
        }

        #region ICallbackEventHandler Members

        public string GetCallbackResult()
        {
            return _htmlReport.Body;
        }

        public void RaiseCallbackEvent(string eventArgument)
        {
            string[] args = eventArgument.Split(null);

            _htmlReport = (Rdl.Render.RenderToHtml)Context.Session[ReportSessionID];

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
        var url = 'PdfExport." + ReportServer._extension + @"?ReportSessionID=<%ReportSessionID%>';
        window.location = url;
        //window.open(url,'_blank','');
        //window.opener=top;
    }
    if (selectedIndex == 2)
    {
        var url = 'XlsExport." + ReportServer._extension + @"?ReportSessionID=<%ReportSessionID%>';
        window.location = url;
        //window.open(url,'_blank','');
        //window.opener=top;
    }
    if (selectedIndex == 3)
    {
        var url = 'TxtExport." + ReportServer._extension + @"?ReportSessionID=<%ReportSessionID%>';
        window.location = url;
        //window.open(url,'_blank','');
        //window.opener=top;
    }
    if (selectedIndex == 4)
    {
        var url = 'CsvExport." + ReportServer._extension + @"?ReportSessionID=<%ReportSessionID%>';
        window.location = url;
        //window.open(url,'_blank','');
        //window.opener=top;
    }
    document.getElementById('Export').selectedIndex = 0;
}    

function Action(a_id)
{
    var elmt = document.getElementById('<%tbAction%>');
    elmt.value = a_id;
    var btn = document.getElementById('<%btnAction%>');
    btn.click();
}

function printClick()
{
    var url = 'Print." + ReportServer._extension + @"?ReportSessionID=<%ReportSessionID%>';
    window.open(url,'_blank','');
    window.opener=top;
}

function imageUrl(elementName, source, name)
{
    var src = 'image." + ReportServer._extension + @"?source=' + source + 
                '&name=' + name +
                '&ReportSessionID=<%ReportSessionID%>';

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

function SelectAll(element) {
    if (element != null) {
        var cbs = element.getElementsByTagName('input');

        for (var i = 0; i < cbs.length; i++) 
            cbs[i].checked = true;

        }
    }

function SelectNone(element) {
    if (element != null) {
        var cbs = element.getElementsByTagName('input');

        for (var i = 0; i < cbs.length; i++) 
            cbs[i].checked = false;
        }
    }

function chkCheckAll_click(checkbox, checkboxlist)
{
    var checked = checkbox.checked;
    if (checked)
        SelectAll(checkboxlist);
    else
        SelectNone(checkboxlist);
}
        
</script>

<asp:Label ID=""LabelReportID"" runat=""server"" Text="""" style=""display:none;""></asp:Label>
<%buttons%>
<div id=""ReportContentDiv"" style=""position: static; overflow: visible;"">
    <%report%>
</div>
";

        private string htmlButtons = @"
Export To:<select id=""Export"" onchange=""ExportOnChange(this.selectedIndex)"">
    <option selected=""selected"">---</option>
    <option>PDF</option>
    <option>Excel</option>
    <option>Text</option>
    <option>CSV</option>
</select>
&nbsp;&nbsp;&nbsp;&nbsp;
<input type=""hidden"" id=""hiddenAction"" name=""hiddenAction"" value="""" />
<input type=""button"" id=""btnPrint"" onclick=""printClick()"" value=""Print..."" />
";

    }
}