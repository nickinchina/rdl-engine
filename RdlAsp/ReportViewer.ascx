<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ReportViewer.ascx.cs" Inherits="RdlAsp.ReportViewer" %>
<script language="javascript">
function ToggleStateData(value)
{
}

function ToggleState(textBoxName, toggleState)
{
    ToggleStateCallback( document.getElementById("LabelReportID").innerText + ' ' + textBoxName + ' ' + toggleState );
}

function ExportOnChange(selectedIndex) 
{
    var reportKey = document.getElementById("LabelReportID").innerText;
    if (selectedIndex == 1)
    {
        var url = "PdfExport.rdlx";
        window.open(url,"_blank","");
        window.opener=top;
    }
    if (selectedIndex == 2)
    {
        var url = "XlsExport.rdlx";
        window.open(url,"_blank","");
        window.opener=top;
    }
    if (selectedIndex == 3)
    {
        var url = "TxtExport.rdlx");
        window.open(url,"_blank","");
        window.opener=top;
    }
    if (selectedIndex == 4)
    {
        var url = "CsvExport.rdlx";
        window.open(url,"_blank","");
        window.opener=top;
    }
    document.getElementById("Export").selectedIndex = 0;
}    

function printClick()
{
    var url = "Print.rdlx";
    window.open(url,"_blank","");
    window.opener=top;
}

</script>

<asp:Label ID="LabelReportID" runat="server" Text="" style="display:none;"></asp:Label>
Export To:<select id="Export" onchange="ExportOnChange(this.selectedIndex)">
    <option selected="selected">---</option>
    <option>PDF</option>
    <option>Excel</option>
    <option>Text</option>
    <option>CSV</option>
</select>
&nbsp;&nbsp;&nbsp;&nbsp;
<input type="button" id="btnPrint" onclick="printClick()" value="Print...">
<table>
<tr><td style="width: 3px">  
    <%--report--%>
</td></tr>
</table>
