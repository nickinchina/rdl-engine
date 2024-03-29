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
using System.Text;
using System.Xml;
using System.IO;

namespace Rdl.Engine
{
    internal class SubReport : ReportItem
    {
        private string _reportName = string.Empty;
        private List<Parameter> _parameters = null;
        private Expression _noRows = null;
        private bool _mergeTransactions = false;
        Report _subReport = null;

        public SubReport(XmlNode node, ReportElement parent)
            : base(node, parent)
        {
        }

        protected override void ParseAttribute(XmlNode attr)
        {
            switch (attr.Name.ToLower())
            {
                case "reportname":
                    _reportName = attr.InnerText;
                    _subReport = new Report();
                    if (!_reportName.Contains("\\"))
                        _reportName = Report.ReportPath + _reportName;
                    if (!_reportName.Contains(".rdl"))
                    {
                        if (File.Exists(_reportName + ".rdl"))
                            _reportName += ".rdl";
                        else if (File.Exists(_reportName + ".rdlc"))
                            _reportName += ".rdlc";
                    }
                    if (!File.Exists(_reportName))
                        throw new Exception("Unable to locate sub report " + _reportName);
                    FileStream fs = new FileStream(_reportName, FileMode.Open, FileAccess.Read, FileShare.Read);

                    _subReport.Load(fs, Path.GetDirectoryName(_reportName));
                    _subReport.Parent = this;
                    fs.Close();
                    fs.Dispose();

                    _subReport.CredentialsPrompt += new Report.CredentialsPromptEventHandler(_subReport_CredentialsPrompt);
                    _subReport.InitializeDataSet += new Report.InitializeDataSetEventHandler(_subReport_InitializeDataSet);
                    break;
                case "parameters":
                    _parameters = new List<Parameter>();
                    foreach (XmlNode child in attr.ChildNodes)
                        _parameters.Add(new Parameter(child, this));
                    break;
                case "norows":
                    _noRows = new Expression(attr.InnerText, this);
                    break;
                case "mergetransactions":
                    _mergeTransactions = bool.Parse(attr.InnerText);
                    break;
                default:
                    break;
            }
        }

        void _subReport_InitializeDataSet(object sender, Rdl.Runtime.InitializeDataSetEventArgs args)
        {
            Report.OnInitializeDataSet(args);
        }

        void _subReport_CredentialsPrompt(object sender, Rdl.Runtime.CredentialsPromptEventArgs args)
        {
            Report.OnCredentialsPrompt(args);
        }

        public Report GetSubReport()
        {
            return _subReport;
        }

        protected override void Render1(Rdl.Render.Container parentBox, Rdl.Runtime.Context context, bool visible)
        {
            if (visible)
            {
                _box = parentBox.AddFixedContainer(this, Style, context);
                _box.Name = _name;
            }
        }

        protected override void Render2(Rdl.Runtime.Context context)
        {
            foreach (Parameter parm in _parameters)
                try
                {
                    _subReport.ReportParameters[parm.Name].Value = parm.Value(context);
                }
                catch (Exception err)
                {
                    throw new Exception("Error setting subreport parameter " + parm.Name, err);
                }

            _subReport.Render((Rdl.Render.Container)_box, context);
        }
    }
}
