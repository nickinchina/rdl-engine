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
