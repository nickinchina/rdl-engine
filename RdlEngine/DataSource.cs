using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace RdlEngine
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
                    _dataSourceReference = attr.InnerText;
                    break;
                default:
                    break;
            }
        }

        public System.Data.Common.DbConnection Connect()
        {
            if (_connectionProperties.DataProvider == "SQL")
            {
                _sqlConn = new System.Data.SqlClient.SqlConnection();
                _sqlConn.ConnectionString = _connectionProperties.ConnectString;
                if (_connectionProperties.Promt != null)
                {
                    RdlRuntime.CredentialsPromptEventArgs args = new RdlRuntime.CredentialsPromptEventArgs();
                    args.Prompt = _connectionProperties.Promt;
                    Report.OnCredentialsPrompt(args);

                    _sqlConn.ConnectionString += ";UID=" + args.UserID + ";PWD=" + args.Password;
                }
                _sqlConn.Open();

                if (_transaction)
                    _sqlTran = _sqlConn.BeginTransaction();
            }
            return null;
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
