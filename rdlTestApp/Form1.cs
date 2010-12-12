using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace rdlTestApp
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            parametersEntry1.ViewReport += new EventHandler<RdlViewer.ParametersEntry.ViewReportEventArgs>(ViewReport);
        }

        static void rpt_InitializeDataSet(object sender, Rdl.Runtime.InitializeDataSetEventArgs args)
        {
            switch(args.DataSetName)
            {
                default:
                    break;
            }
        }

        private void ViewReport(object sender, RdlViewer.ParametersEntry.ViewReportEventArgs args)
        {
            args.Report.Run();
            reportViewer1.SetReport(args.Report);
        }

        private void buttonCompanySales_Click(object sender, EventArgs e)
        {
            parametersEntry1.Hide();
            Rdl.Engine.Report rpt = new Rdl.Engine.Report();
            rpt.Load(new FileStream(@"C:\Documents and Settings\MEmerson.VITALBASICS\My Documents\Visual Studio Projects\rdlTest\SampleReports\Company Sales.rdl", FileMode.Open, FileAccess.Read, FileShare.Read),
                @"C:\Documents and Settings\MEmerson.VITALBASICS\My Documents\Visual Studio Projects\rdlTest\SampleReports");

            rpt.Run();

            reportViewer1.SetReport(rpt);
        }

        private void btnEmployeeSalesSummary_Click(object sender, EventArgs e)
        {
            Rdl.Engine.Report rpt = new Rdl.Engine.Report();
            rpt.Load(new FileStream(@"C:\Documents and Settings\MEmerson.VITALBASICS\My Documents\Visual Studio Projects\rdlTest\SampleReports\Employee Sales Summary.rdl", FileMode.Open, FileAccess.Read, FileShare.Read),
                @"C:\Documents and Settings\MEmerson.VITALBASICS\My Documents\Visual Studio Projects\rdlTest\SampleReports");

            parametersEntry1.Show();
            parametersEntry1.SetReport(rpt);
        }

        private void btnProductCatalog_Click(object sender, EventArgs e)
        {
            Rdl.Engine.Report rpt = new Rdl.Engine.Report();
            rpt.Load(new FileStream(@"C:\Documents and Settings\MEmerson.VITALBASICS\My Documents\Visual Studio Projects\rdlTest\SampleReports\Product Catalog.rdl", FileMode.Open, FileAccess.Read, FileShare.Read),
                @"C:\Documents and Settings\MEmerson.VITALBASICS\My Documents\Visual Studio Projects\rdlTest\SampleReports");

            rpt.Run();

            reportViewer1.SetReport(rpt);
        }

        private void btnSalesOrderDetail_Click(object sender, EventArgs e)
        {
            Rdl.Engine.Report rpt = new Rdl.Engine.Report();
            rpt.Load(new FileStream(@"C:\Documents and Settings\MEmerson.VITALBASICS\My Documents\Visual Studio Projects\rdlTest\SampleReports\Sales Order Detail.rdl", FileMode.Open, FileAccess.Read, FileShare.Read),
                @"C:\Documents and Settings\MEmerson.VITALBASICS\My Documents\Visual Studio Projects\rdlTest\SampleReports");

            RdlViewer.ParametersDialog dlg = new RdlViewer.ParametersDialog(rpt);
            dlg.Title = "Sales Order Detail Parameters";
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                rpt.Run();

                reportViewer1.SetReport(rpt);
            }
        }
    }
}