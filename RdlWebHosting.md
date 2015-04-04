#How to include RDL reports in an ASP.NET web page

# Introduction #
Using the rdl-engine in an ASP.NET web site is relatively straight forward, but does
require some set up.  The web site project needs to reference RDLASP.DLL, and because
RDLASP includes an IHTTPHandler class there needs to be entries for it in the web.config
and in the IIS site setup.  Also the Parameter control use AJAX, so you need to have the
AjaxControlToolkit installed and referenced on your pages.

# Web Page #
First make sure that AJAX and RDLASP are referenced at the top of your page.
Include an AJAX ScriptManager somewhere on your form.
Optionally put a Parameters control on your page.  This control is only necessary if your report includes parameters and you want to use the Parameters control to allow the user to fill in the parameter values.
Include the ReportViewer control on your page.

A complete report viewer web page can look like:
```
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="RdlViewer.aspx.cs" Inherits="Reports.RdlViewer" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<%@ Register TagPrefix="uc" Assembly="RdlAsp" Namespace="RdlAsp" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Report Viewer</title>
</head>
<body>
    <form id="form1" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server">
    </asp:ScriptManager>
    <table>
        <tr>
            <td>
                <uc:Parameters ID="ReportParameters" runat="server" OnViewReport="ReportParameters_ViewReport" OnParameterError="ReportParameters_ParameterError" />
            </td>
        </tr>
    </table>
    <div style="color:Red;">
        <asp:Label ID="txtParameterError" runat="server"></asp:Label>
    </div>
    <div>
        <uc:ReportViewer ID="ReportViewer" runat="server" />
    </div>
    </form>
</body>
</html>
```

# Code Behind #
The code behind needs obtain a report either by loading it from a RDL file, or from a linked assembly which was compiled from an RDL file.  If you are using the Parameters control you will need to wire up the ViewReport event for that control and load the report into the ReportViewer at that time.

```
protected void Page_Load(object sender, EventArgs e)
{
   // Load the report from a file
   Report rpt = new Report();
   FileStream fs = new FileStream(@"test.rdl", FileMode.Open, FileAccess.Read, FileShare.Read);
   rpt.Load(fs, "." );
   fs.Close();
   fs.Dispose();
   ReportViewer.SetReport(null);
   // put the report into the Parameters control
   ReportParameters.SetReport(rpt);
}

protected void ReportParameters_ParameterError(object sender, RdlAsp.ParameterErrorEventArgs e)
{
   txtParameterError.Text = e.ErrorMessage;
}

protected void ReportParameters_ViewReport(object sender, RdlAsp.ViewReportEventArgs e)
{
   // Run the report and put into the ReportViewer control
   Report rpt = e.Report;
   Rdl.Render.GenericRender render = rpt.Run();
   ReportViewer.SetReport(render);
   txtParameterError.Text = string.Empty;
}
```

# IIS Setup #
The IIS Setup needs to define an httpHandler for rdlx.  In IIS configuration under Handler Mappings create a new mapping.  The Requested Path should be **.rdlx.  The Type should be RdlAsp.ReportServer.**

# Web.Config #
The web.config needs to include an httpHandler for rdlx.
```
<configuration>
   <system.web>
      <httpHandlers>
         <add verb="*" path="*.rdlx" type="RdlAsp.ReportServer, RdlAsp"/>
      </httpHandlers>
   </system.web>
<configuration>
```