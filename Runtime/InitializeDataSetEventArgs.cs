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
    /// <summary>
    /// Used in the <see cref="InitializeDataSetEventHandler"/> event.
    /// </summary>
    public class InitializeDataSetEventArgs : EventArgs
    {
        /// <summary>
        /// The name of the dataset from the report definition file.  This can be used 
        /// to identify which data set to load in a report that references multiple
        /// data sets.
        /// </summary>
        public string DataSetName;
        /// <summary>
        /// The DataTable for this DataSetName.  Leave null to have the RDL Engine
        /// use the data set defined in the report definition file.
        /// </summary>
        public System.Data.DataTable dt = null;
    }
}
