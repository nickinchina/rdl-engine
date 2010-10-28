using System;
using System.Collections.Generic;
using System.Text;

namespace Rdl.Runtime
{
    public class Globals
    {
        private Rdl.Engine.Report _rpt;

        public Globals(Rdl.Engine.Report rpt)
        {
            _rpt = rpt;
        }

        public object this[string key]
        {
            get 
            {
                switch (key)
                {
                    case "PageNumber":
                        return PageNumber;
                    case "TotalPages":
                        return TotalPages;
                    case "ExecutionTime":
                        return ExecutionTime;
                    case "ReportFolder":
                        return ReportFolder;
                    case "ReportName":
                        return ReportName;
                }
                return null;
            }
        }

        public int PageNumber
        {
            get { return 0; }
        }

        public int TotalPages
        {
            get { return 0; }
        }

        public DateTime ExecutionTime
        {
            get { return DateTime.Now; }
        }

        public string ReportFolder
        {
            get { return string.Empty; }
        }

        public string ReportName
        {
            get { return string.Empty; }
        }
    }
}
