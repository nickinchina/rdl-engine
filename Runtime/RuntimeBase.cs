using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Rdl.Engine;

public delegate object RdlExpressionDeletage();

namespace Rdl.Runtime
{
    public partial class RuntimeBase
    {
        // 
        protected Rdl.Engine.Report _rpt;
        // The list of Field elements.  This is set in Report.Render
        protected Fields Fields;
        // The list of ReportParameters elements.  This is set in Report.Render
        protected Parameters Parameters;
        protected Globals Globals;
        protected User User;
        // The list of named expressions.  This is built in Report.Render
        private List<RdlExpressionDeletage> _expressionList = new List<RdlExpressionDeletage>();
        // At every expression the current context is set to the 
        // current DataSet Filters so the Fields can be
        // resolved appropriately.
        private Context _currentContext;
        private object _thisLock = new object();
        private System.Threading.Mutex mutex = new System.Threading.Mutex(false);

        public struct SaveContextState
        {
            internal Context context;
            internal ContextState contextState;
        }
        private Stack<SaveContextState> _contextStack = new Stack<SaveContextState>();

        public delegate object AggrFn();

        /// <summary>
        /// This constructure is used with compiled reports.  There should 
        /// be an embedded resource with the report definition.
        /// </summary>
        public RuntimeBase()
        {
        }

        protected void Initialize(Rdl.Engine.Report rpt)
        {
            _rpt = rpt;
            Fields = new Fields(this);
            Parameters = new Parameters(_rpt);
            Globals = new Globals(_rpt);
            User = new User(_rpt);
            _rpt.LoadDefaults();
        }

        public void AddFunction(RdlExpressionDeletage expression)
        {
            _expressionList.Add(expression);
        }

        internal Context CurrentContext
        {
            get { return _currentContext; }
        }

        /// <summary>
        /// Gets the <see cref="Report"/> instance associated with this instance.
        /// </summary>
        public Rdl.Engine.Report Report
        {
            get { return _rpt; }
        }

        public Dictionary<string, ReportItem> ReportItems
        {
            get { return _rpt.ReportItems; }
        }

        internal object Exec(Int32 key, Context ctxt)
        {
            lock(_thisLock)
            {
                _currentContext = ctxt;
                return _expressionList[key]();
            }
        }

        internal string ExecAsString(Int32 key, Context ctxt)
        {
            lock(_thisLock)
            {
                _currentContext = ctxt;
                object val = _expressionList[key]();
                if (Convert.IsDBNull(val) || val == null)
                    return null;
                else
                    return val.ToString();
            }
        }

        internal string ExecAsString(Int32 key, Context ctxt, string format)
        {
            lock(_thisLock)
            {
                _currentContext = ctxt;
                object val = _expressionList[key]();
                if (Convert.IsDBNull(val) || val == null)
                    return null;
                else if (format == null || format == string.Empty)
                    return val.ToString();
                else
                    return String.Format("{0:" + format + "}", val);
            }
        }

        internal bool ExecAsBoolean(Int32 key, Context ctxt)
        {
            lock(_thisLock)
            {
                _currentContext = ctxt;
                return bool.Parse(_expressionList[key]().ToString());
            }
        }

        internal Int32 ExecAsInt(Int32 key, Context ctxt)
        {
            lock(_thisLock)
            {
                _currentContext = ctxt;
                return Convert.ToInt32(_expressionList[key]());
            }
        }

        internal decimal ExecAsDecimal(Int32 key, Context ctxt)
        {
            lock(_thisLock)
            {
                _currentContext = ctxt;
                return Convert.ToDecimal(_expressionList[key]());
            }
        }

        internal void PushContextState()
        {
            SaveContextState scs = new SaveContextState();
            scs.context = _currentContext;
            if (_currentContext != null)
                scs.contextState = _currentContext.ContextState;
            _contextStack.Push(scs);
        }

        internal void PopContextState()
        {
            SaveContextState scs = _contextStack.Pop();
            if (scs.context != null)
            {
                _currentContext = scs.context;
                _currentContext.ContextState = scs.contextState;
            }
        }
    }
}
