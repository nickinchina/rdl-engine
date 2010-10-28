using System;
using System.Collections.Generic;
using System.Text;

namespace RdlRuntime
{
    public class InitializeDataSetEventArgs : EventArgs
    {
        public string DataSetName;
        public System.Data.DataTable dt = null;
    }
}
