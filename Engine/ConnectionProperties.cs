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
    class ConnectionProperties : ReportElement
    {
        private string _dataProvider = null;
        private Expression _connectString = null;
        private bool _integratedSecurity = false;
        private string _prompt = null;

        public ConnectionProperties(XmlNode node, ReportElement parent)
            : base(node, parent)
        {
        }

        protected override void ParseAttribute(XmlNode attr)
        {
            switch (attr.Name.ToLower())
            {
                case "dataprovider":
                case "extension":
                    _dataProvider = attr.InnerText;
                    break;
                case "connectstring":
                    _connectString = new Expression(attr, this, false);
                    break;
                case "integratedsecurity":
                    _integratedSecurity = bool.Parse(attr.InnerText);
                    break;
                case "prompt":
                    _prompt = attr.InnerText;
                    break;
                default:
                    break;
            }
        }

        public string DataProvider
        {
            get { return _dataProvider; }
        }

        public string ConnectString(Rdl.Runtime.Context context)
        {
            return _connectString.ExecAsString(context);
        }

        public bool IntegratedSecurity
        {
            get { return _integratedSecurity; }
        }

        public string Promt
        {
            get { return _prompt; }
        }
    }
}
