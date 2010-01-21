using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace RdlViewer
{
    public partial class ParametersDialog : Form
    {
        public ParametersDialog(Rdl.Engine.Report report)
        {
            InitializeComponent();

            parametersEntry1.SetReport(report);
            parametersEntry1.ViewReport += new EventHandler<ParametersEntry.ViewReportEventArgs>(ViewReport);
            DialogResult = DialogResult.Cancel;
        }

        public void ViewReport(object sender, ParametersEntry.ViewReportEventArgs args)
        {
            DialogResult = DialogResult.OK;
            this.Hide();
        }

        public string Title
        {
            get { return this.Text; }
            set { this.Text = value; }
        }
    }
}