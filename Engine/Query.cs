using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Rdl.Engine
{
    class Query : ReportElement
    {
        private enum CommandTypeEnum
        {
            Text,
            StoredProcedure,
            TableDirect
        };


        private string _dataSourceName = null;
        private CommandTypeEnum _commandType = CommandTypeEnum.Text;
        private Expression _commandText = null;
        private List<QueryParameter> _queryParameters = new List<QueryParameter>();
        private Int32 _timeout = 0;

        public Query(XmlNode node, ReportElement parent)
            : base(node, parent)
        {
        }

        protected override void ParseAttribute(XmlNode attr)
        {
            switch (attr.Name.ToLower())
            {
                case "datasourcename":
                    _dataSourceName = attr.InnerText;
                    break;
                case "commandtype":
                    _commandType = (CommandTypeEnum)Enum.Parse(typeof(CommandTypeEnum), attr.InnerText, true);
                    break;
                case "commandtext":
                    _commandText = new Expression(attr, this);
                    break;
                case "queryparameters":
                    foreach(XmlNode child in attr.ChildNodes)
                        _queryParameters.Add(new QueryParameter(child, this));
                    break;
                case "timeout":
                    _timeout = Int32.Parse(attr.InnerText);
                    break;
                default:
                    break;
            }
        }

        public void Exec(System.Data.DataSet dsRet, string name, Rdl.Runtime.Context context)
        {
            DataSource ds = Report.DataSource(_dataSourceName);
            if (ds == null)
                throw new Exception("Invalid DataSourceName " + _dataSourceName + " in DataSet " + ((DataSet)Parent).Name);

            ds.Connect(context);

            if (ds.ConnectionProperties.DataProvider == "SQL")
            {
                System.Data.SqlClient.SqlCommand cmm =
                    ds.sqlConn.CreateCommand();

                switch (_commandType)
                {
                    case CommandTypeEnum.Text :
                        cmm.CommandType = System.Data.CommandType.Text;
                        break;
                    case CommandTypeEnum.StoredProcedure:
                        cmm.CommandType = System.Data.CommandType.StoredProcedure;
                        break;
                    case CommandTypeEnum.TableDirect:
                        cmm.CommandType = System.Data.CommandType.TableDirect;
                        break;
                }

                cmm.CommandTimeout = _timeout;

                foreach (QueryParameter parm in _queryParameters)
                {
                    object parmValue = parm.Value(context);
                    if (parmValue == null)
                        parmValue = DBNull.Value;
                    if (parmValue is Array)
                    {
                        string s = (((Array)parmValue).Length > 0) ? ((Array)parmValue).GetValue(0).ToString() : String.Empty;
                        for( int i=1; i < ((Array)parmValue).Length; i++)
                            s += "," + ((Array)parmValue).GetValue(i).ToString();
                        parmValue = s;
                    }
                    System.Data.SqlClient.SqlParameter sqlParm = 
                        new System.Data.SqlClient.SqlParameter(parm.Name, parmValue);
                    cmm.Parameters.Add(sqlParm);
                }

                cmm.CommandText = _commandText.ExecAsString(context);

                System.Data.SqlClient.SqlDataAdapter da =
                    new System.Data.SqlClient.SqlDataAdapter(cmm);

                da.Fill(dsRet, name);
            }
        }
    }
}
