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
    class DataSource : ReportElement
    {
        private string _name = null;
        private bool _transaction = false;
        private ConnectionProperties _connectionProperties = null;
        private string _dataSourceReference = null;
        private System.Data.SqlClient.SqlConnection _sqlConn = null;
        private System.Data.SqlClient.SqlTransaction _sqlTran = null;

        public DataSource(XmlNode node, ReportElement parent)
            : base(node, parent)
        {
            // If the connection properties are defined in a DataSourceReference then
            // move them into a ConnectionProperties so compiled reports don't need
            // the external DataSource file.
            if (_dataSourceReference != null)
            {
                // Remove the DataSourceReference
                for( int i=0; i < node.ChildNodes.Count;)
                    if (node.ChildNodes[i].Name.ToLower() == "datasourcereference")
                        node.RemoveChild(node.ChildNodes[i]);
                    else
                        i++;

                // Add the ConnectionProperties.
                XmlNode newNode = node.OwnerDocument.CreateElement("ConnectionProperties");
                node.AppendChild(newNode);
                XmlAttribute attr = node.OwnerDocument.CreateAttribute("DataProvider");
                attr.Value = _connectionProperties.DataProvider;
                newNode.Attributes.Append(attr);
                attr = node.OwnerDocument.CreateAttribute("ConnectString");
                attr.Value = _connectionProperties.ConnectString(new Rdl.Runtime.Context(null, null, null, null, null));
                newNode.Attributes.Append(attr);
                attr = node.OwnerDocument.CreateAttribute("IntegratedSecurity");
                attr.Value = _connectionProperties.IntegratedSecurity.ToString();
                newNode.Attributes.Append(attr);
                attr = node.OwnerDocument.CreateAttribute("Prompt");
                attr.Value = _connectionProperties.Promt;
                newNode.Attributes.Append(attr);
            }
        }

        protected override void ParseAttribute(XmlNode attr)
        {
            switch (attr.Name.ToLower())
            {
                case "name":
                    _name = attr.InnerText;
                    break;
                case "transaction":
                    _transaction = bool.Parse(attr.InnerText);
                    break;
                case "connectionproperties":
                    _connectionProperties = new ConnectionProperties(attr, this);
                    break;
                case "datasourcereference":
                    // Find the referenced DataSourceReference file
                    _dataSourceReference = attr.InnerText;
                    FileStream fs = null;
                    if (File.Exists(_dataSourceReference))
                        fs = new FileStream(_dataSourceReference, FileMode.Open, FileAccess.Read, FileShare.Read);
                    else if (File.Exists(_dataSourceReference + ".rds"))
                        fs = new FileStream(_dataSourceReference + ".rds", FileMode.Open, FileAccess.Read, FileShare.Read);
                    else if (File.Exists(Report.ReportPath + _dataSourceReference))
                        fs = new FileStream(Report.ReportPath + _dataSourceReference, FileMode.Open, FileAccess.Read, FileShare.Read);
                    else if (File.Exists(Report.ReportPath + _dataSourceReference + ".rds"))
                        fs = new FileStream(Report.ReportPath + _dataSourceReference + ".rds", FileMode.Open, FileAccess.Read, FileShare.Read);
                    else
                        throw new Exception("Error locating shared DataSourceReference " + _dataSourceReference);

                    XmlDocument doc = new XmlDocument();
                    doc.Load(fs);
                    fs.Close();
                    fs.Dispose();

                    XmlNamespaceManager nsmgr = new XmlNamespaceManager(doc.NameTable);
                    if (doc.NamespaceURI != string.Empty)
                        nsmgr.AddNamespace("bk", doc.NamespaceURI);

                    XmlNode connProperties = doc.SelectSingleNode("/RptDataSource[Name='" + _dataSourceReference + "']/ConnectionProperties", nsmgr);

                    _connectionProperties = new ConnectionProperties(connProperties, this);
                    break;
                default:
                    break;
            }
        }

        public System.Data.Common.DbConnection Connect(Rdl.Runtime.Context context)
        {
            if (_connectionProperties.DataProvider == "SQL")
            {
                _sqlConn = new System.Data.SqlClient.SqlConnection();
                _sqlConn.ConnectionString = _connectionProperties.ConnectString(context);
                if (_connectionProperties.IntegratedSecurity)
                    _sqlConn.ConnectionString += ";Trusted_Connection=true;";
                if (_connectionProperties.Promt != null && _connectionProperties.Promt != String.Empty)
                {
                    Rdl.Runtime.CredentialsPromptEventArgs args = new Rdl.Runtime.CredentialsPromptEventArgs();
                    args.Prompt = _connectionProperties.Promt;
                    Report.OnCredentialsPrompt(args);

                    _sqlConn.ConnectionString += ";User ID=" + args.UserID + ";Password=" + args.Password;
                }
                _sqlConn.Open();

                if (_transaction)
                    _sqlTran = _sqlConn.BeginTransaction();
            }
            return null;
        }

        public void Close()
        {
            if (_connectionProperties.DataProvider == "SQL")
            {
                _sqlConn.Close();
                _sqlConn.Dispose();
                _sqlConn = null;
            }
        }

        public string Name
        {
            get { return _name; }
        }

        public System.Data.SqlClient.SqlConnection sqlConn
        {
            get { return _sqlConn; }
        }

        public ConnectionProperties ConnectionProperties
        {
            get { return _connectionProperties; }
        }
    }
}
