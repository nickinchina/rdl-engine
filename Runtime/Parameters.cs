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
                if (_rpt.ReportParameters[key] == null)
                    throw new Exception("Invalid report parameter " + key);
                return _rpt.ReportParameters[key];
            }
        }
    }
}
