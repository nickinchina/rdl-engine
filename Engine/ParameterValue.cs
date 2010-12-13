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
    public class ParameterValue : ReportElement, IEquatable<ParameterValue>
    {
        private Expression _value = null;
        private Expression _label = null;

        public ParameterValue(XmlNode node, ReportElement parent)
            : base(node, parent)
        {
        }

        public ParameterValue(string value, string label) : base(null, null)
        {
            _value = new Expression(value, this);
            _label = new Expression(label, this);
        }

        protected override void ParseAttribute(XmlNode attr)
        {
            switch (attr.Name.ToLower())
            {
                case "value":
                    _value = new Expression(attr, this, false);
                    break;
                case "label":
                    _label = new Expression(attr, this, false);
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Gets the value for this default value.
        /// </summary>
        public string Value
        {
            get
            {
                if (_value == null)
                    return null;
                else
                    return _value.ExecAsString(new Rdl.Runtime.Context(null, null, null, null, null));
            }
        }

        /// <summary>
        /// Gets the label for this default value.
        /// </summary>
        public string Label
        {
            get
            {
                if (_label == null)
                    return Value;
                else
                    return _label.ExecAsString(new Rdl.Runtime.Context(null, null, null, null, null));
            }
        }

        #region IEquatable<ParameterValue> Members

        public bool Equals(ParameterValue other)
        {
            if (ReferenceEquals(other, null))
                return false;
            if (ReferenceEquals(other, this))
                return true;
            return (other.Value == this.Value && other.Label == this.Label);
        }

        #endregion

        public override bool Equals(object obj)
        {
            return base.Equals((ParameterValue)obj);
        }

        public static bool operator ==(ParameterValue parm1, ParameterValue parm2)
        {
            return parm1.Equals(parm2);
        }

        public static bool operator !=(ParameterValue parm1, ParameterValue parm2)
        {
            return !parm1.Equals(parm2);
        }
    }
}
