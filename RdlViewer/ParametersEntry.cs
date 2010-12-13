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
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace RdlViewer
{
    public partial class ParametersEntry : UserControl
    {
        private Rdl.Engine.Report _report = null;

        /// <summary>
        /// Used in the <see cref="ParametersEntry.ViewReport"/> event.fl
        /// </summary>
        public class ViewReportEventArgs : EventArgs
        {
            public Rdl.Engine.Report Report;

            public ViewReportEventArgs(Rdl.Engine.Report report)
            {
                Report = report;
            }
        }

        /// <summary>
        /// Raised when the ViewReport button is clicked.
        /// </summary>
        public EventHandler<ViewReportEventArgs> ViewReport;

        public ParametersEntry()
        {
            InitializeComponent();
        }

        public void SetReport(Rdl.Engine.Report report)
        {
            _report = report;
            Panel1.Controls.Clear();

            foreach (Rdl.Engine.ReportParameter parm in _report.ReportParameters.Values)
            {
                Label label = new Label();
                label.Text = parm.Prompt;
                label.AutoSize = true;
                Panel1.Controls.Add(label);

                if (parm.ValidValues.Count > 0)
                {
                    if (parm.MultiValue)
                    {
                        CheckedListBox lb = new CheckedListBox();
                        foreach (Rdl.Engine.ParameterValue value in parm.ValidValues)
                            lb.Items.Add(value.Label);
                        lb.SelectionMode = SelectionMode.MultiSimple;
                        lb.CheckOnClick = true;
                        lb.Name = parm.Name;

                        if (parm.DefaultValue != null)
                        {
                            foreach (object o in parm.DefaultValue)
                                for (int i = 0; i < parm.ValidValues.Count; i++)
                                    if (parm.ValidValues[i].Value.CompareTo(o) == 0)
                                        lb.SetItemChecked(i, true);
                        }
                        Panel1.Controls.Add(lb);
                    }
                    else
                    {
                        ComboBox combo = new ComboBox();
                        foreach (Rdl.Engine.ParameterValue value in parm.ValidValues)
                            combo.Items.Add(value.Label);
                        combo.Name = parm.Name;

                        if (parm.DefaultValue != null)
                        {
                            foreach (Rdl.Engine.ParameterValue value in parm.ValidValues)
                                if (value.Value.CompareTo(parm.DefaultValue[0]) == 0)
                                    combo.SelectedItem = value.Label;
                        }
                        Panel1.Controls.Add(combo);
                    }
                }
                else
                    switch (parm.DataType.Name)
                    {
                        case "Boolean":
                            CheckBox cb = new CheckBox();
                            if (parm.DefaultValue != null)
                                cb.Checked = bool.Parse(parm.DefaultValue[0]);
                            cb.Name = parm.Name;
                            Panel1.Controls.Add(cb);
                            break;
                        case "DateTime":
                            DateTimePicker dtp = new DateTimePicker();
                            dtp.Name = parm.Name;
                            if (parm.DefaultValue != null && parm.DefaultValue.Length > 0)
                                dtp.Value = DateTime.Parse(parm.DefaultValue[0]);
                            Panel1.Controls.Add(dtp);
                            break;
                        case "Int32":
                            TextBox tb = new TextBox();
                            tb.Name = parm.Name;
                            if (parm.DefaultValue != null && parm.DefaultValue.Length > 0)
                                tb.Text = parm.DefaultValue[0];
                            Panel1.Controls.Add(tb);
                            break;
                        case "Single":
                            TextBox tb2 = new TextBox();
                            tb2.Name = parm.Name;
                            if (parm.DefaultValue != null && parm.DefaultValue.Length > 0)
                                tb2.Text = parm.DefaultValue[0];
                            Panel1.Controls.Add(tb2);
                            break;
                        case "String":
                            TextBox tb3 = new TextBox();
                            tb3.Name = parm.Name;
                            if (parm.DefaultValue != null && parm.DefaultValue.Length > 0)
                                tb3.Text = parm.DefaultValue[0];
                            Panel1.Controls.Add(tb3);
                            break;
                    }
            }
            Panel1.Controls.Add(btnViewReport);
        }

        void WriteParameters()
        {
            foreach (Control ctrl in Panel1.Controls)
            {
                Rdl.Engine.ReportParameter parm = null;
                if (_report.ReportParameters != null && _report.ReportParameters.ContainsKey(ctrl.Name))
                    parm = _report.ReportParameters[ctrl.Name];
                if (parm != null)
                {
                    switch (ctrl.GetType().Name)
                    {
                        case "CheckBox":
                            parm.Value = ((CheckBox)ctrl).Checked;
                            break;
                        case "DateTimePicker":
                            parm.Value = ((DateTimePicker)ctrl).Value;
                            break;
                        case "TextBox":
                            parm.Value = ((TextBox)ctrl).Text;
                            break;
                        case "ComboBox":
                            foreach (Rdl.Engine.ParameterValue value in parm.ValidValues)
                                if (value.Label == ((ComboBox)ctrl).Text)
                                    parm.Value = value.Value;
                            break;
                        case "CheckedListBox":
                            List<object> valueArray = new List<object>();
                            foreach (object o in ((CheckedListBox)ctrl).CheckedItems)
                                foreach (Rdl.Engine.ParameterValue value in parm.ValidValues)
                                    if (value.Label == (string)o)
                                        valueArray.Add(value.Value);
                            parm.Value = valueArray.ToArray();
                            break;
                    }
                }
            }
        }

        private void btnViewReport_Click(object sender, EventArgs e)
        {
            WriteParameters();

            if (ViewReport != null)
                ViewReport(this, new ViewReportEventArgs(_report));
        }

        public string ViewReportButtonText
        {
            get { return btnViewReport.Text; }
            set { btnViewReport.Text = value; }
        }

    }
}
