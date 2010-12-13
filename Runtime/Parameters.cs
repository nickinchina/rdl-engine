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
    public class Parameters
    {
        private Rdl.Engine.Report _rpt;

        public Parameters(Rdl.Engine.Report rpt)
        {
            _rpt = rpt;
        }

        public object this[string key]
        {
            get 
            {
                if (!_rpt.ReportParameters.ContainsKey(key))
                    throw new Exception("Invalid report parameter " + key);
                return _rpt.ReportParameters[key];
            }
        }
    }
}
