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
using System.IO;
using System.Security.Permissions;

namespace RdlAspTest
{
    public partial class _Default : System.Web.UI.Page
    {
        private string reportPath = "/SampleReports";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (ConfigurationManager.AppSettings["ReportPath"] != null)
                reportPath = ConfigurationManager.AppSettings["ReportPath"];

            if (reportPath.Contains("/"))
                reportPath = Server.MapPath(reportPath);
        }

        protected void CompanySales_Click(object sender, EventArgs e)
        {
            Rdl.Engine.Report rpt = new Rdl.Engine.Report();
            rpt.Load(new FileStream(@"C:\Documents and Settings\MEmerson.VITALBASICS\My Documents\Visual Studio Projects\rdlTest\SampleReports\Company Sales.rdl", FileMode.Open, FileAccess.Read, FileShare.Read),
                @"C:\Documents and Settings\MEmerson.VITALBASICS\My Documents\Visual Studio Projects\rdlTest\SampleReports");

            rpt.Run();
            ReportParameters.SetReport(null);
            ReportViewer.SetReport(rpt);
        }

        protected void EmployeeSalesSummary_Click(object sender, EventArgs e)
        {
            Rdl.Engine.Report rpt = new Rdl.Engine.Report();
            rpt.Load(new FileStream(@"C:\Documents and Settings\MEmerson.VITALBASICS\My Documents\Visual Studio Projects\rdlTest\SampleReports\Employee Sales Summary.rdl", FileMode.Open, FileAccess.Read, FileShare.Read),
                @"C:\Documents and Settings\MEmerson.VITALBASICS\My Documents\Visual Studio Projects\rdlTest\SampleReports");

            ReportParameters.SetReport(rpt);
        }

        protected void EmployeeSales_Click(object sender, EventArgs e)
        {
            Rdl.Engine.Report rpt = new Rdl.Engine.Report();
            rpt.Load(new FileStream(@"C:\Documents and Settings\MEmerson.VITALBASICS\My Documents\Visual Studio Projects\rdlTest\SampleReports\Employee Sales.rdl", FileMode.Open, FileAccess.Read, FileShare.Read),
                @"C:\Documents and Settings\MEmerson.VITALBASICS\My Documents\Visual Studio Projects\rdlTest\SampleReports");

            rpt.Run();
            ReportParameters.SetReport(null);
            ReportViewer.SetReport(rpt);
        }

        protected void ProductCatalog_Click(object sender, EventArgs e)
        {
            Rdl.Engine.Report rpt = new Rdl.Engine.Report();
            rpt.Load(new FileStream(@"C:\Documents and Settings\MEmerson.VITALBASICS\My Documents\Visual Studio Projects\rdlTest\SampleReports\Product Catalog.rdl", FileMode.Open, FileAccess.Read, FileShare.Read),
                @"C:\Documents and Settings\MEmerson.VITALBASICS\My Documents\Visual Studio Projects\rdlTest\SampleReports");

            rpt.Run();
            ReportParameters.SetReport(null);
            ReportViewer.SetReport(rpt);
        }

        protected void ProductLineSales_Click(object sender, EventArgs e)
        {
            Rdl.Runtime.Product_Line_Sales pls = new Rdl.Runtime.Product_Line_Sales();
            Rdl.Engine.Report rpt = pls.Report;
            //Rdl.Engine.Report rpt = new Rdl.Engine.Report();
            //rpt.Load(new FileStream(@"C:\Documents and Settings\MEmerson.VITALBASICS\My Documents\Visual Studio Projects\rdlTest\SampleReports\Product Line Sales.rdl", FileMode.Open, FileAccess.Read, FileShare.Read),
            //    @"C:\Documents and Settings\MEmerson.VITALBASICS\My Documents\Visual Studio Projects\rdlTest\SampleReports");

            ReportParameters.SetReport(rpt);
        }

        protected void ProductsOrdered_Click(object sender, EventArgs e)
        {
            Rdl.Engine.Report rpt = new Rdl.Engine.Report();
            rpt.Load(new FileStream(@"C:\Documents and Settings\MEmerson.VITALBASICS\My Documents\Visual Studio Projects\rdlTest\SampleReports\ProductsOrdered.rdl", FileMode.Open, FileAccess.Read, FileShare.Read),
                @"C:\Documents and Settings\MEmerson.VITALBASICS\My Documents\Visual Studio Projects\rdlTest\SampleReports");

            rpt.Run();
            ReportParameters.SetReport(null);
            ReportViewer.SetReport(rpt);
        }

        protected void SalesOrderDetail_Click(object sender, EventArgs e)
        {
            Rdl.Engine.Report rpt = new Rdl.Engine.Report();
            rpt.Load(new FileStream(@"C:\Documents and Settings\MEmerson.VITALBASICS\My Documents\Visual Studio Projects\rdlTest\SampleReports\Sales Order Detail.rdl", FileMode.Open, FileAccess.Read, FileShare.Read),
                @"C:\Documents and Settings\MEmerson.VITALBASICS\My Documents\Visual Studio Projects\rdlTest\SampleReports");

            ReportParameters.SetReport(rpt);
        }

        protected void TerritorySalesDrilldown_Click(object sender, EventArgs e)
        {
            Rdl.Engine.Report rpt = new Rdl.Engine.Report();
            rpt.Load(new FileStream(@"C:\Documents and Settings\MEmerson.VITALBASICS\My Documents\Visual Studio Projects\rdlTest\SampleReports\Territory Sales Drilldown.rdl", FileMode.Open, FileAccess.Read, FileShare.Read),
                @"C:\Documents and Settings\MEmerson.VITALBASICS\My Documents\Visual Studio Projects\rdlTest\SampleReports");

            rpt.Run();
            ReportParameters.SetReport(null);
            ReportViewer.SetReport(rpt);
        }


        protected void ReportParameters_ViewReport(object sender, RdlAsp.ViewReportEventArgs e)
        {
            Rdl.Engine.Report rpt = e.Report;
            rpt.Run();
            ReportViewer.SetReport(rpt);
        }
    }
}
