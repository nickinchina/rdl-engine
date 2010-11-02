using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.IO;
using System.CodeDom.Compiler;
using System.Reflection;
using System.Resources;
using Microsoft.VisualBasic;
using RdlRender;

namespace RdlEngine
{
    public class Report : ReportElement
    {
        private enum DataElementStyleEnum
        {
            AttributeNormal,
            ElementNormal
        };
        private XmlDocument _doc = new XmlDocument();
        private int _autoRefresh;
        private Size _width = new Size("0 in");
        private Size _pageHeight = new Size("11 in");
        private Size _pageWidth = new Size("8.5 in");
        private Size _interactiveHeight = null;
        private Size _interactiveWidth = null;
        private Size _leftMargin = new Size("0");
        private Size _rightMargin = new Size("0");
        private Size _topMargin = new Size("0");
        private Size _bottomMargin = new Size("0");
        private Expression _language = null;
        private Dictionary<string, ReportParameter> _reportParameters = new Dictionary<string, ReportParameter>();
        private DataSets _dataSets = null;
        private Dictionary<string, DataSource> _dataSources = new Dictionary<string, DataSource>();
        private string _dataSchema = null;
        private string _dataElementName = null;
        private DataElementStyleEnum _dataElementStyle = DataElementStyleEnum.AttributeNormal;
        private Body _body = null;
        private Int32 _filterIndex = 1;
        private DataSet _reportDataSet = null;
        private Dictionary<string, ReportItem> _reportItems = new Dictionary<string, ReportItem>();

        private List<string> _functions = new List<string>();
        internal RdlRuntime.RuntimeBase Rtb = null;

        public delegate void InitializeDataSetEventHandler( object sender, RdlRuntime.InitializeDataSetEventArgs args );
        public event InitializeDataSetEventHandler InitializeDataSet;
        public delegate void CredentialsPromptEventHandler(object sender, RdlRuntime.CredentialsPromptEventArgs args);
        public event CredentialsPromptEventHandler CredentialsPrompt;

        public Report(XmlNode node, ReportElement parent)
            : base(node, parent)
        {
            Initialize();
        }

        public Report()
            : base(null, null)
        {
            Initialize();
        }

        private void Initialize()
        {
            _language = new Expression("en-us", this);
        }

        public void Load(string report)
        {
            _doc.LoadXml(report);

            parse(_doc.DocumentElement);
        }

        public void Load(Stream input)
        {
            _doc.Load(input);

            parse(_doc.DocumentElement);
        }

        private void parse(XmlElement report)
        {
            foreach(XmlAttribute attr in report.Attributes)
                ParseAttribute(attr);

            foreach (XmlNode child in report.ChildNodes)
                ParseAttribute(child);

            Parse2();
        }

        protected override void ParseAttribute(XmlNode attr)
        {
            switch (attr.Name.ToLower())
            {
                case "autorefresh":
                    _autoRefresh = int.Parse(attr.InnerText);
                    break;
                case "datasources":
                    foreach (XmlNode child in attr.ChildNodes)
                    {
                        DataSource ds = new DataSource(child, this);
                        _dataSources.Add(ds.Name, ds);
                    }
                    break;
                case "datasets":
                    _dataSets = new DataSets(attr, this);
                    break;
                case "body":
                    _body = new Body(attr, this);
                    break;
                case "reportparameters":
                    foreach (XmlElement parm in attr.ChildNodes)
                    {
                        ReportParameter param = new ReportParameter(parm, this);
                        _reportParameters.Add(param.Name, param);
                    }
                    break;
                case "custom":
                    break;
                case "width":
                    _width = new Size(attr.InnerText);
                    break;
                case "pageheader":
                    break;
                case "pagefooter":
                    break;
                case "pageheight":
                    _pageHeight = new Size(attr.InnerText);
                    break;
                case "pagewidth":
                    _pageWidth = new Size(attr.InnerText);
                    break;
                case "interactiveheight":
                    _interactiveHeight = new Size(attr.InnerText);
                    break;
                case "interactivewidth":
                    _interactiveWidth = new Size(attr.InnerText);
                    break;
                case "leftmargin":
                    _leftMargin = new Size(attr.InnerText);
                    break;
                case "rightmargin":
                    _rightMargin = new Size(attr.InnerText);
                    break;
                case "topmargin":
                    _topMargin = new Size(attr.InnerText);
                    break;
                case "bottommargin":
                    _bottomMargin = new Size(attr.InnerText);
                    break;
                case "embeddedimages":
                    break;
                case "language":
                    _language = new Expression(attr, this);
                    break;
                case "codemodules":
                    break;
                case "classes":
                    break;
                case "datatransform":
                    break;
                case "dataschema":
                    _dataSchema = attr.InnerText;
                    break;
                case "dataelementname":
                    _dataElementName = attr.InnerText;
                    break;
                case "dataelementstyle":
                    _dataElementStyle = (DataElementStyleEnum)Enum.Parse(typeof(DataElementStyleEnum), attr.InnerText,true);
                    break;
                default:
                    break;
            }
        }

        public RdlRender.FixedContainer Render()
        {
            FixedContainer topBox = new FixedContainer(null, this, new BoxStyle(Style.DefaultStyle));

            topBox.Name = "Report";
            if (_width.points > 0)
            {
                topBox.Width = _width.points;
                topBox.CanGrowHorizonally = false;
            }

            // Set up the compiler
            CompilerParameters cp = new CompilerParameters();
            cp.GenerateExecutable = false;
            cp.GenerateInMemory = true;
            cp.TreatWarningsAsErrors = false;
            if (File.Exists(System.AppDomain.CurrentDomain.RelativeSearchPath +  @"\rdlEngine.dll"))
                cp.ReferencedAssemblies.Add(System.AppDomain.CurrentDomain.RelativeSearchPath +  @"\rdlEngine.dll");
            else if (File.Exists(System.AppDomain.CurrentDomain.BaseDirectory +  @"\rdlEngine.dll"))
                cp.ReferencedAssemblies.Add(System.AppDomain.CurrentDomain.BaseDirectory +  @"\rdlEngine.dll");
            else
                throw new Exception("Unable to file rdlEngine.dll");

            // Replace the aggregate function calls with deletage calls.
            int ct = _functions.Count;
            for (int i = 0; i < ct; i++)
                _functions[i] = ReplaceAggregatesWithDelegates(_functions[i]);

            // Build the source code for the named functions.
            string addFns = string.Empty;
            string functions = string.Empty;
            for( int i=0; i < _functions.Count; i++)
            {
                addFns += "     AddFunction( AddressOf fn_" + i.ToString() + " )\n";
                functions += _functions[i] + "\n";
            }

            string source = RdlEngine.RdlResource.RuntimeCode;
            source = source.Replace(@"' Add Functions", addFns);
            source = source.Replace(@"' Functions", functions);

            // Compile the runtime source
            CodeDomProvider provider = new VBCodeProvider();
            CompilerResults cr = provider.CompileAssemblyFromSource(cp, new string[] { source });

            if (cr.Errors.Count > 0)
                throw new Exception("Compile errors");

            // Get a reference to a RunTimeBase object.
            Assembly CodeAssembly = cr.CompiledAssembly;
            Type ty = CodeAssembly.GetType("RdlRuntime.RunTimeClass");
            Rtb = (RdlRuntime.RuntimeBase)Activator.CreateInstance(ty,
                new object[] { this });

            // Initialize the data sets.
            _dataSets.Initialize();

            // Set the default DataSet for the report
            if (_dataSchema != null)
                _reportDataSet = _dataSets[_dataSchema];
            else
                _reportDataSet = _dataSets.FirstDataSet;

            // Set up the default data context.
            _context = new RdlRuntime.Context(
                null, _reportDataSet, null, null, null);
            topBox.ContextBase = true;

            // Render the report.
            _body.Render(topBox);

            topBox.LinkToggles();

            return topBox;
        }

        public Dictionary<string, ReportParameter> ReportParameters
        {
            get { return _reportParameters; }
        }

        public Int32 AddFunction(string body)
        {
            Int32 index = _functions.Count;
            string fnName = "fn_" + index.ToString();
            string fn = "Private Function " + fnName + "() as Object\n" +
                "   " + fnName + " = " + body + "\n" +
                "End Function\n";
            _functions.Add(fn);
            return index;
        }

        private string ReplaceAggregatesWithDelegates(string source)
        {
            source = ReplaceAggFn(source, "sum");
            source = ReplaceAggFn(source, "Count");
            source = ReplaceAggFn(source, "CountDistinct");

            return source;
        }

        private string MatchAggFn(Match m)
        {
            int fn = AddFunction(m.Groups["expression"].Value);
            return m.Groups["leading"].Value + "( AddressOf fn_" + fn.ToString() + " )";
        }

        private string ReplaceAggFn(string source, string fn)
        {

            Regex regEx = new Regex(
                @"(?<leading>[^\w_]+            " +
                fn + @"\s*?)\(                  " +
                @"     (?<expression>           " +
                @"       (?>                    " +
                @"          ""(?<string>[^""\\]*(\\.[^""\\]*)*)""" +
                @"        |                     " +
                @"          [^()""]+            " +
                @"        |                     " +
                @"          \((?<LEVEL>)        " +
                @"        |                     " +
                @"          \)(?<-LEVEL>)       " +
                @"       )*                     " +
                @"      (?(LEVEL)(?!))          " +
                @"     )\)                      "
                , RegexOptions.IgnorePatternWhitespace | RegexOptions.IgnoreCase);

           return  regEx.Replace(source, new MatchEvaluator(MatchAggFn));
        }

        public void OnInitializeDataSet(RdlRuntime.InitializeDataSetEventArgs args)
        {
            if (InitializeDataSet != null)
                InitializeDataSet(this, args);
        }

        public void OnCredentialsPrompt(RdlRuntime.CredentialsPromptEventArgs args)
        {
            if (CredentialsPrompt != null)
                CredentialsPrompt(this, args);
        }

        internal DataSets DataSets
        {
            get { return _dataSets; }
        }

        internal DataSource DataSource(string key)
        {
            return _dataSources[key];
        }

        internal Int32 FilterIndex
        {
            get { return ++_filterIndex; }
        }

        public string Language
        {
            get { return _language.ExecAsString(); }
        }

        public Size PageHeight
        {
            get { return _pageHeight; }
        }

        public Size PageWidth
        {
            get { return _pageWidth; }
        }

        public Size InteractiveHeight
        {
            get { return _interactiveHeight; }
        }

        public Size InteractiveWidth
        {
            get { return _interactiveWidth; }
        }

        public Size LeftMargin
        {
            get { return _leftMargin; }
        }

        public Size RightMargin
        {
            get { return _rightMargin; }
        }

        public Size TopMargin
        {
            get { return _topMargin; }
        }

        public Size BottomMargin
        {
            get { return _bottomMargin; }
        }

        internal Dictionary<string, ReportItem> ReportItems
        {
            get { return _reportItems; }
        }
    }
}
