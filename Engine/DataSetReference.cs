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
    class DataSetReference : ReportElement
    {
        private string _dataSetName = null;
        private string _valueField = null;
        private string _labelField = null;

        public DataSetReference(XmlNode node, ReportElement parent)
            : base(node, parent)
        {
        }

        protected override void ParseAttribute(XmlNode attr)
        {
            switch (attr.Name.ToLower())
            {
                case "datasetname":
                    _dataSetName = attr.InnerText;
                    break;
                case "valuefield":
                    _valueField = attr.InnerText;
                    break;
                case "labelfield":
                    _labelField = attr.InnerText;
                    break;
                default:
                    break;
            }
        }

        public string DataSetName
        {
            get { return _dataSetName; }
        }

        public string ValueField
        {
            get { return _valueField; }
        }

        public string LableField
        {
            get { return _labelField; }
        }
    }
}