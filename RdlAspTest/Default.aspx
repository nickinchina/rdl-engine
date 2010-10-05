<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="RdlAspTest._Default" EnableSessionState="true" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<%@ Register Assembly="System.Web.Extensions, Version=1.0.61025.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35"
    Namespace="System.Web.UI" TagPrefix="asp" %>
<%@ Register TagPrefix="uc" Assembly="RdlAsp" Namespace="RdlAsp" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Untitled Page</title>
</head>
<body>
    <form id="form1" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server">
    </asp:ScriptManager>
    <div>
        <asp:Button ID="Button1" runat="server" OnClick="CompanySales_Click" Text="Company Sales" />
        <asp:Button ID="Button3" runat="server" OnClick="EmployeeSalesSummary_Click" Text="Employee Sales Summary" />
        <asp:Button ID="Button4" runat="server" OnClick="EmployeeSales_Click" Text="Employee Sales" />
        <asp:Button ID="Button5" runat="server" OnClick="ProductCatalog_Click" Text="Product Catalog" />
        <asp:Button ID="Button6" runat="server" OnClick="ProductLineSales_Click" Text="Product Line Sales" />
        <asp:Button ID="Button7" runat="server" OnClick="ProductsOrdered_Click" Text="Products Ordered" />
        <asp:Button ID="Button8" runat="server" OnClick="SalesOrderDetail_Click" Text="Sales Order Detail" />
        <asp:Button ID="Button2" runat="server" OnClick="TerritorySalesDrilldown_Click" Text="Territory Sales Drilldown" />
        <asp:TextBox ID="TextBox1" runat="server"></asp:TextBox>
        <table>
            <tr>
                <td>
                <uc:Parameters ID="ReportParameters" runat="server" OnViewReport="ReportParameters_ViewReport" />
                </td>
            </tr>
            <tr>
                <td style="height: 30px">
                <uc:ReportViewer ID="ReportViewer" runat="server" />
                </td>
            </tr>
        </table>
        &nbsp;
    </div>
    </form>
</body>
</html>
