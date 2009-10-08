using System;
using System.ComponentModel;
using System.Data;
using System.Collections.Generic;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Security.Permissions;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Web.SessionState;

namespace RdlAsp
{
    /// <summary>
    /// Used in the <see cref="Parameters.ViewReport"/> event.fl
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
    /// A dynamic parameter entry control.
    /// When the SetReport function is called this control fill in with entry fields
    /// for each parameter in the report.  If default values are available then
    /// the control will be a drop-down list populated with the default values.
    /// When the View button is pressed, the <see cref="Rdl.Engine.Report"/> 
    /// <see cref="Rdl.Engine.ReportParameter"/> are filled with values from this control.
    /// </summary>
    [
    AspNetHostingPermission(SecurityAction.Demand,
        Level = AspNetHostingPermissionLevel.Minimal),
    AspNetHostingPermission(SecurityAction.InheritanceDemand,
        Level = AspNetHostingPermissionLevel.Minimal),
    ToolboxData("<{0}:Register runat=\"server\"> </{0}:Register>"),
    ]
    public class Parameters : System.Web.UI.WebControls.CompositeControl
    {
        protected Rdl.Engine.Report _report = null;
        private Dictionary<string, Control> _parameterControls = new Dictionary<string, Control>();
        private Button btnView;

        private static readonly object EventViewReport =
            new object();

        protected override void CreateChildControls()
        {
            Controls.Clear();
            if (_report == null)
                _report = (Rdl.Engine.Report)Page.Session["RdlReport"];

            if (_report != null)
            {
                Controls.Add(new LiteralControl("<div>"));
                foreach (Rdl.Engine.ReportParameter parm in _report.ReportParameters.Values)
                {
                    if (!parm.Hidden)
                    {
                        Controls.Add(new LiteralControl("<div style=\"float:left; margin: 5px;\">"));
                        Label label = new Label();
                        label.Text = parm.Prompt;
                        Controls.Add(label);

                        if (parm.ValidValues.Count > 0)
                        {
                            if (parm.MultiValue)
                            {
                                CheckBoxList lb = new CheckBoxList();
                                Controls.Add(lb);
                                foreach (Rdl.Engine.ParameterValue value in parm.ValidValues)
                                    lb.Items.Add(new ListItem(value.Label, value.Value));
                                lb.ID = parm.Name;

                                if (parm.DefaultValue != null)
                                {
                                    foreach (string s in parm.DefaultValue)
                                        foreach (ListItem li in lb.Items)
                                            if (li.Value == s)
                                                li.Selected = true;
                                }
                            }
                            else
                            {
                                DropDownList ddl = new DropDownList();
                                Controls.Add(ddl);
                                if (parm.Nullable)
                                    ddl.Items.Add(new ListItem("", ""));
                                foreach (Rdl.Engine.ParameterValue value in parm.ValidValues)
                                    ddl.Items.Add(new ListItem(value.Label, value.Value));
                                ddl.ID = parm.Name;

                                if (parm.DefaultValue != null)
                                {
                                    foreach (ListItem li in ddl.Items)
                                        if (li.Value == parm.DefaultValue[0])
                                            ddl.Text = li.Text;
                                }
                            }
                        }
                        else
                            switch (parm.DataType.Name)
                            {
                                case "Boolean":
                                    CheckBox cb = new CheckBox();
                                    Controls.Add(cb);
                                    if (parm.DefaultValue != null)
                                        cb.Checked = bool.Parse(parm.DefaultValue[0]);
                                    cb.ID = parm.Name;
                                    break;
                                case "DateTime":
                                    TextBox tbCal = new TextBox();
                                    tbCal.ID = parm.Name;
                                    Controls.Add(tbCal);
                                    if (parm.DefaultValue != null && parm.DefaultValue.Length > 0)
                                        tbCal.Text = parm.DefaultValue[0];
                                    ImageButton ib = new ImageButton();
                                    Controls.Add(ib);
                                    ib.ID = parm.Name + "_ib";
                                    ib.ImageUrl = "image." + ReportServer._extension + "?source=resource&name=calendar.bmp";
                                    AjaxControlToolkit.CalendarExtender ce = new AjaxControlToolkit.CalendarExtender();
                                    Controls.Add(ce);
                                    ce.PopupButtonID = parm.Name + "_ib";
                                    ce.TargetControlID = parm.Name;
                                    ce.ID = parm.Name + "_extender";
                                    break;
                                case "Int32":
                                    TextBox tb = new TextBox();
                                    Controls.Add(tb);
                                    tb.ID = parm.Name;
                                    if (parm.DefaultValue != null && parm.DefaultValue.Length > 0)
                                        tb.Text = parm.DefaultValue[0];
                                    break;
                                case "Single":
                                    TextBox tb2 = new TextBox();
                                    Controls.Add(tb2);
                                    tb2.ID = parm.Name;
                                    if (parm.DefaultValue != null && parm.DefaultValue.Length > 0)
                                        tb2.Text = parm.DefaultValue[0];
                                    break;
                                case "String":
                                    TextBox tb3 = new TextBox();
                                    Controls.Add(tb3);
                                    tb3.ID = parm.Name;
                                    if (parm.DefaultValue != null && parm.DefaultValue.Length > 0)
                                        tb3.Text = parm.DefaultValue[0];
                                    break;
                            }

                        // Add some space before the next control.
                        Controls.Add(new LiteralControl("</div>"));
                    }
                    //Controls.Add(new LiteralControl("&nbsp&nbsp&nbsp&nbsp"));
                }
                Controls.Add(new LiteralControl("</div>"));
                btnView = new Button();
                this.Controls.Add(btnView);
                btnView.Text = "View";
                btnView.ID = "ViewReport";
                btnView.Click += new EventHandler(btnView_Click);
            }
        }

        /// <summary>
        /// Raised when the ViewReport button is pressed
        /// The <see cref="Rdl.Engine.Report"/> class is passed through this event which can
        /// then in turn be passed to the <see cref="ReportViewer"/> control.
        /// </summary>
        [
        Category("Action"),
        Description("Raised when the user clicks the View button.")
        ]
        public event EventHandler<ViewReportEventArgs> ViewReport
        {
            add
            {
                Events.AddHandler(EventViewReport, value);
            }
            remove
            {
                Events.RemoveHandler(EventViewReport, value);
            }
        }

        // The method that raises the ViewReport event.
        protected virtual void OnViewReport(ViewReportEventArgs e)
        {
            EventHandler<ViewReportEventArgs> ViewReportHandler =
                (EventHandler<ViewReportEventArgs>)Events[EventViewReport];
            if (ViewReportHandler != null)
            {
                ViewReportHandler(this, e);
            }
        }

        void btnView_Click(object sender, EventArgs e)
        {
            EnsureChildControls();
            foreach (Control ctrl in Controls)
            {
                Rdl.Engine.ReportParameter parm = null;
                if (ctrl.ID != null && _report.ReportParameters != null && _report.ReportParameters.ContainsKey(ctrl.ID))
                    parm = _report.ReportParameters[ctrl.ID];
                if (parm != null)
                {
                    switch (ctrl.GetType().Name)
                    {
                        case "CheckBox":
                            parm.Value = ((CheckBox)ctrl).Checked;
                            break;
                        case "Calendar":
                            parm.Value = DateTime.Parse(((TextBox)ctrl).Text);
                            break;
                        case "TextBox":
                            parm.Value = ((TextBox)ctrl).Text;
                            break;
                        case "DropDownList":
                            if (((DropDownList)ctrl).Text == string.Empty)
                            {
                                if (parm.Nullable)
                                    parm.Value = null;
                                else
                                    parm.Value = string.Empty;
                                break;
                            }
                            foreach (Rdl.Engine.ParameterValue value in parm.ValidValues)
                                if (value.Label == ((DropDownList)ctrl).SelectedItem.Text)
                                    parm.Value = value.Value;
                            break;
                        case "CheckBoxList":
                            List<object> valueArray = new List<object>();
                            foreach (ListItem li in ((CheckBoxList)ctrl).Items)
                                if (li.Selected)
                                    foreach (Rdl.Engine.ParameterValue value in parm.ValidValues)
                                        if (value.Label == li.Text)
                                            valueArray.Add(value.Value);
                            parm.Value = valueArray.ToArray();
                            break;
                    }
                }
            }
            OnViewReport(new ViewReportEventArgs(_report));
        }

        /// <summary>
        /// Sets the <see cref="Rdl.Engine.Report"/> to use to fill the parameters.
        /// </summary>
        /// <param name="report"></param>
        public void SetReport(Rdl.Engine.Report report)
        {
            // Load the valid values into the report parameters.
            _report = report;
            if (report != null)
                foreach (Rdl.Engine.ReportParameter r in report.ReportParameters.Values)
                    r.LoadValidValues(report);
            Page.Session["RdlReport"] = _report;
            ChildControlsCreated = false;
        }
    }
}
