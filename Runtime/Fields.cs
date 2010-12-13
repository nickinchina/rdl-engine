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

namespace Rdl.Runtime
{
    public class Fields
    {
        private RuntimeBase _rtb;

        public Fields(RuntimeBase rtb)
        {
            _rtb = rtb;
        }

        public object this[string key]
        {
            get 
            {
                return new Field(_rtb, key);
            }
        }
    }

    public class Field
    {
        private string _key;
        private RuntimeBase _rtb;

        public Field(RuntimeBase rtb, string key)
        {
            _key = key;
            _rtb = rtb;
        }

        public object Value
        {
            get { return _rtb.CurrentContext.DataSet.Fields[_key].GetValue(_rtb.CurrentContext); }
        }

        public bool IsMissing
        {
            get { return _rtb.CurrentContext.DataSet.Fields[_key].IsMissing(_rtb.CurrentContext); }
        }
    }
}
