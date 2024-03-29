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
                    _commandText = new Expression(attr, this, false);
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
                string commandText = _commandText.ExecAsString(context);

                foreach (QueryParameter parm in _queryParameters)
                {
                    object parmValue = parm.Value(context);
                    if (parmValue is Array)
                    {
                        int i = 0;
                        string parmNameList = string.Empty;

                        // For arrays build a list of parameters to pass to the database.
                        foreach (string s in (string[])parmValue)
                        {
                            System.Data.SqlClient.SqlParameter sqlParm =
                                new System.Data.SqlClient.SqlParameter(parm.Name + "_" + i.ToString(), s);
                            cmm.Parameters.Add(sqlParm);
                            parmNameList += ((parmNameList.Length == 0) ? string.Empty : ",") + parm.Name + "_" + (i++).ToString();
                        }

                        commandText = commandText.Replace(parm.Name, parmNameList);
                    }
                    else
                    {
                        if (parmValue == null)
                            parmValue = DBNull.Value;
                        System.Data.SqlClient.SqlParameter sqlParm =
                            new System.Data.SqlClient.SqlParameter(parm.Name, parmValue);
                        cmm.Parameters.Add(sqlParm);
                    }
                }


                cmm.CommandText = commandText;

                System.Data.SqlClient.SqlDataAdapter da =
                    new System.Data.SqlClient.SqlDataAdapter(cmm);

                if (dsRet.Tables[name] != null)
                    dsRet.Tables[name].Clear();
                da.Fill(dsRet, name);
            }

            ds.Close();
        }
    }
}
