using System;
using System.Collections.Generic;
using System.Text;

namespace RdlRuntime
{
    public class Parameters
    {
        private RdlEngine.Report _rpt;

        public Parameters(RdlEngine.Report rpt)
        {
            _rpt = rpt;
        }

        public object this[string key]
        {
            get 
            {
                return _rpt.ReportParameters[key].Value;
            }
        }
    }
}
