/*-----------------------------------------------------------------------------------
This file is part of the SawikiSoft RDL Engine.
The SawikiSoft RDL Engine is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

The SawikiSoft RDL Engine is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with Foobar.  If not, see <http://www.gnu.org/licenses/>.
-----------------------------------------------------------------------------------*/
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