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
using System.Text.RegularExpressions;
using System.Xml;
using System.IO;
using System.CodeDom.Compiler;
using System.Reflection;
using System.Resources;
using Microsoft.VisualBasic;
using Rdl.Render;

namespace Rdl.Engine
{
    /// <summary>
    /// Represents an RDL report.  Provides methods for loading a report from
    /// a report definition file, compiling the report into a named assembly,
    /// or running the report to an internal rendering.
    /// </summary>
    public class Report : ReportElement
    {
        private enum DataElementStyleEnum
        {
            AttributeNormal,
            ElementNormal
        };

        public class CompileException : Exception
        {
            public System.Collections.Specialized.StringCollection Output;
            public System.CodeDom.Compiler.CompilerErrorCollection Errors;

            public CompileException(System.Collections.Specialized.StringCollection output, System.CodeDom.Compiler.CompilerErrorCollection errors)
                : base("Compile exception")
            {
                Output = output;
                Errors = errors;
            }
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
        private PageHeaderFooter _pageHeader = null;
        private PageHeaderFooter _pageFooter = null;
        private Int32 _filterIndex = 1;
        private DataSet _reportDataSet = null;
        private Dictionary<string, ReportItem> _reportItems = new Dictionary<string, ReportItem>();
        private string _code = null;
        private List<string> _codeModules = new List<string>();
        private string _xmlNameSpace = string.Empty;
        private Dictionary<string, EmbeddedImage> _embeddedImages = new Dictionary<string, EmbeddedImage>();
        private string _reportPath = string.Empty;

        private List<string> _functions = new List<string>();
        public Rdl.Runtime.RuntimeBase Rtb = null;

        /// <summary>
        /// Used with the <see cref="InitializeDataSet"/> event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public delegate void InitializeDataSetEventHandler( object sender, Rdl.Runtime.InitializeDataSetEventArgs args );
        /// <summary>
        /// Raised from the Run method to allow the user to load the report with custom data.
        /// </summary>
        public event InitializeDataSetEventHandler InitializeDataSet;
        /// <summary>
        /// Used with the <see cref="CredentialsPrompt"/> event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public delegate void CredentialsPromptEventHandler(object sender, Rdl.Runtime.CredentialsPromptEventArgs args);
        /// <summary>
        /// Raised from the Run method to allow the user to be prompted for database credentials.
        /// </summary>
        public event CredentialsPromptEventHandler CredentialsPrompt;

        internal Report(XmlNode node, ReportElement parent)
            : base(node, parent)
        {
            Initialize();
        }

        /// <summary>
        /// The default constructor
        /// </summary>
        public Report()
            : base(null, null)
        {
            Initialize();
        }

        private void Initialize()
        {
            _language = new Expression("en-us", this);
        }

        public void Load(Rdl.Runtime.RuntimeBase rtb, string report)
        {
            _doc.LoadXml(report);

            parse(_doc.DocumentElement);

            Rtb = rtb;
            //LoadDefaults();
        }

        /// <summary>
        /// Loads a report from a report definition file
        /// </summary>
        /// <param name="input">The Stream containing the report definition</param>
        /// <param name="path">The path to referenced files (subreports, shared data sources, etc)</param>
        /// <exception cref="CompileException">
        /// Raised if errors occur compiling the VB code contained in the report.
        /// </exception>
        public void Load(Stream input, string path)
        {
            _doc.Load(input);
            _reportPath = path;
            if (_reportPath[_reportPath.Length - 1] != '\\')
                _reportPath += "\\";

            parse(_doc.DocumentElement);

            Compile(null, null);
        }

        public void LoadDefaults()
        {
            // If there are any report parameters then load the default and valid values
            // from the data sources.
            foreach (ReportParameter r in _reportParameters.Values)
                r.LoadDefaultValues(this);
            foreach (ReportParameter r in _reportParameters.Values)
                r.LoadValidValues(this);
        }

        /// <summary>
        /// Compile the given report to an assembly.
        /// </summary>
        /// <param name="input">The Stream containing the report definition</param>
        /// <param name="path">The path to referenced files (subreports, shared data sources, etc)</param>
        /// <param name="assemblyPath">The location to save the compiled assembly to</param>
        /// <param name="reportName">The name to give the resulting Class in the assembly.
        /// The compiled class will be derived from the Rdl.Runtime.RunTimeBase class.
        /// </param>
        /// <exception cref="CompileException">
        /// Raised if errors occur compiling the VB code contained in the report.
        /// </exception>
        public void CompileToAssembly(Stream input, string path,
            string assemblyPath, string reportName)
        {
            _doc.Load(input);
            _reportPath = path;
            if (_reportPath[_reportPath.Length - 1] != '\\')
                _reportPath += "\\";

            parse(_doc.DocumentElement);

            Compile(assemblyPath, reportName);
        }

        private void Compile(string assemplyPath, string reportName)
        {
            // Set up the compiler
            CompilerParameters cp = new CompilerParameters();
            cp.GenerateExecutable = false;
            if (assemplyPath != null && assemplyPath != string.Empty)
            {
                cp.OutputAssembly = assemplyPath;
                cp.GenerateInMemory = false;
                cp.IncludeDebugInformation = true;
            }
            else
                cp.GenerateInMemory = true;
            cp.TreatWarningsAsErrors = false;
            cp.ReferencedAssemblies.Add("System.Xml.dll");
            LoadModule(cp, "RdlEngine.dll");

            foreach (string codeModule in _codeModules)
                LoadModule(cp, codeModule);

            // Replace the aggregate function calls with deletage calls.
            //int ct = _functions.Count;
            //for (int i = 0; i < ct; i++)
            //    _functions[i] = ReplaceAggregatesWithDelegates(_functions[i]);

            // Build the source code for the named functions.
            string addFns = string.Empty;
            string functions = string.Empty;
            for (int i = 0; i < _functions.Count; i++)
            {
                addFns += "     AddFunction( AddressOf fn_" + i.ToString() + " )\n";
                functions += _functions[i] + "\n";
            }

            string source = Rdl.RdlResource.RuntimeCode;
            source = source.Replace(@"' Add Functions", addFns);
            source = source.Replace(@"' Functions", functions);
            if (reportName != null && reportName != string.Empty)
                source = source.Replace(@"RunTimeReportName", reportName);
            if (!cp.GenerateInMemory)
            {
                string tempName = Path.Combine(Path.GetTempPath(), "Rdl.Runtime." + reportName + ".resources");
                ResourceWriter rw = new ResourceWriter(new FileStream(tempName, FileMode.Create));
                rw.AddResource("ReportSource", _doc.InnerXml);
                rw.Close();

                cp.EmbeddedResources.Add(tempName);
            }

            if (_code != null)
            {
                source +=
                    "\nnamespace Rdl.Runtime\n" +
                    "   Public Class Code\n" +
                    _code + "\n" +
                    "   end class\n" +
                    "end namespace\n";
            }


            // Compile the runtime source
            CodeDomProvider provider = new VBCodeProvider();
            CompilerResults cr = provider.CompileAssemblyFromSource(cp, new string[] { source });

            // Delete the temporary resources.
            foreach (string s in cp.EmbeddedResources)
                System.IO.File.Delete(s);

            if (cr.Errors.Count > 0)
                throw new CompileException(cr.Output, cr.Errors);

            if (cp.GenerateInMemory)
            {
                // Get a reference to a RunTimeBase object.
                Assembly CodeAssembly = cr.CompiledAssembly;
                Type ty = CodeAssembly.GetType("Rdl.Runtime.RunTimeReportName");
                Rtb = (Rdl.Runtime.RuntimeBase)Activator.CreateInstance(ty,
                    new object[] { this });
                //LoadDefaults();
            }
        }

        private void parse(XmlElement report)
        {
            _xmlNameSpace = report.NamespaceURI;

            foreach(XmlAttribute attr in report.Attributes)
                ParseAttribute(attr);

            foreach (XmlNode child in report.ChildNodes)
                ParseAttribute(child);
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
                    _pageHeader = new PageHeaderFooter(attr, this, PageHeaderFooter.HeaderFooterEnum.Header);
                    break;
                case "pagefooter":
                    _pageFooter = new PageHeaderFooter(attr, this, PageHeaderFooter.HeaderFooterEnum.Footer);
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
                    foreach (XmlNode child in attr.ChildNodes)
                    {
                        EmbeddedImage e = new EmbeddedImage(child, this);
                        _embeddedImages.Add(e.Name, e);
                    }   
                    break;
                case "language":
                    _language = new Expression(attr, this);
                    break;
                case "codemodules":
                    foreach (XmlNode child in attr.ChildNodes)
                        _codeModules.Add(child.Attributes[0].InnerText);
                    break;
                case "code":
                    _code = attr.InnerText;
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

        /// <summary>
        /// Run the report.  This will generate an internal rendering which can
        /// then be used by one or more of the rendering classes to produce 
        /// a report in an output format.
        /// </summary>
        /// <returns>Rdl.Render.GenericRender</returns> 
        public Rdl.Render.GenericRender Run()
        {
            return Render(null, null);
        }

        internal new Rdl.Render.GenericRender Render(Rdl.Render.Container box, Rdl.Runtime.Context parentContext)
        {
            Rdl.Runtime.Context context = new Rdl.Runtime.Context(
                parentContext,
                null,
                null,
                null,
                null);

            // Initialize the data sets. // Initialized in RunTimeBase
            //_dataSets.Initialize();
            _dataSets.Reset();

            // Set the default DataSet for the report
            if (_dataSchema != null)
                _reportDataSet = _dataSets[_dataSchema];
            else
                _reportDataSet = _dataSets.FirstDataSet;

            // Set up the default data context.
            if (parentContext != null)
                parentContext = parentContext.FindContextByDS(_reportDataSet);
            context = new Rdl.Runtime.Context(
                parentContext, _reportDataSet, null, null, null);

            Rdl.Render.GenericRender render;
            if (box == null)
            {
                render = new Rdl.Render.GenericRender(this, context);
                box = render.BodyContainer;
            }
            else
                render = box.Render;

            if (_pageHeader != null)
                _pageHeader.Render(render.PageHeaderContainer, context);

            if (_pageFooter != null)
                _pageFooter.Render(render.PageFooterContainer, context);

            // Render the report.
            _body.Render(box, context);

            context.LinkToggles();

            return render;
        }

        private static void LoadModule(CompilerParameters cp, string module)
        {
            if (File.Exists(module))
                cp.ReferencedAssemblies.Add(module);
            if (File.Exists(System.AppDomain.CurrentDomain.RelativeSearchPath + @"\" + module))
                cp.ReferencedAssemblies.Add(System.AppDomain.CurrentDomain.RelativeSearchPath + @"\" + module);
            else if (File.Exists(System.AppDomain.CurrentDomain.BaseDirectory + @"\" + module))
                cp.ReferencedAssemblies.Add(System.AppDomain.CurrentDomain.BaseDirectory + @"\" + module);
            else
                throw new Exception("Unable to find " + module);
        }

        /// <summary>
        /// The parameters defined in the report.  The parameter values should be
        /// filled prior to calling the <see cref="Run"/> method.
        /// </summary>
        public Dictionary<string, ReportParameter> ReportParameters
        {
            get { return _reportParameters; }
        }

        /// <exclude/>
        public Int32 AddFunction(string body, string name, bool replaceAggregates)
        {
            if (replaceAggregates)
                body = ReplaceAggregatesWithDelegates(" " + body);
            Int32 index = _functions.Count;
            string fnName = "fn_" + index.ToString();
            string fn = "Private Function " + fnName + "() as Object\n" +
                "Try\n" +
                "   " + fnName + " = " + body + "\n" +
                "Catch err As Exception\n" +
                "   Throw New Exception( \"" + name + "\" + vbCrLf + \"" + body.Replace("\"", "\"\"") + "\", err)\n" +
                "End Try\n" +
                "End Function\n";
            _functions.Add(fn);
            return index;
        }

        private string ReplaceAggregatesWithDelegates(string source)
        {
            source = ReplaceAggFn(source, "Sum");
            source = ReplaceAggFn(source, "Avg");
            source = ReplaceAggFn(source, "Max");
            source = ReplaceAggFn(source, "Min");
            source = ReplaceAggFn(source, "Count");
            source = ReplaceAggFn(source, "CountDistinct");
            source = ReplaceAggFn(source, "StDev");
            source = ReplaceAggFn(source, "StDevP");
            source = ReplaceAggFn(source, "Var");
            source = ReplaceAggFn(source, "VarP");
            source = ReplaceAggFn(source, "First");
            source = ReplaceAggFn(source, "Last");
            source = ReplaceAggFn(source, "Previous");
            source = ReplaceAggFn(source, "RunningValue");
            source = ReplaceAggFn(source, "Aggregate");

            return source;
        }

        private string MatchAggFn(Match m)
        {
            int fn = AddFunction(m.Groups["expression"].Value, "Aggregate", true);
            if (m.Groups["leading"].Value.Trim() == "RunningValue")
            {
                string[] splitParms = m.Groups["scope"].Value.Split(new char[] { ',' });
                // The second parameter of RunningValue needs the RunningValueFunction enum prepended onto it.
                return "RunningValue(AddressOf fn_" + fn.ToString() + ", RunningValueFunction." +
                    splitParms[1].Trim() +
                    ((splitParms.Length > 2) ? ", " + splitParms[2] : string.Empty) +
                    ")";
            }
            else
                return m.Groups["leading"].Value + "(AddressOf fn_" + fn.ToString() + m.Groups["scope"].Value + " )";
        }

        private string ReplaceAggFn(string source, string fn)
        {
            // All of the aggregate functions have the expression to evaluate as the first
            // argument to the function.  We use a regular expression to pull out the expression
            // and create an delegate out of it.  
            Regex regEx = new Regex(
                @"(?<leading>[^\w_.]+           " +
                fn + @"\s*?)\(                  " +// The expression is everything inside the first open paren
                @"     (?<expression>           " +
                @"       (                   " + 
                @"          ""(?<string>[^""\\]*(\\.[^""\\]*)*)""" +  // Skip anything inside quotes
                @"        |                     " +
                @"          [^()""]+?            " + // Match everything except (),"
                @"        |                     " +
                @"          \((?<LEVEL>)        " + // Level up on every (
                @"        |                     " +
                @"          \)(?<-LEVEL>)       " + // Level down on every )
                @"       )*?                     " +
                @"      )                       " +
                @"      (?<scope>(,[^\(\),]*)*) " + // optional comma separated additional parameters.
                @"      (?(LEVEL)(?!))          " + // If the level group is defined then this match will fail indicating an unmatched open paren
                @"     \)"   // Match the closing paren.
                , RegexOptions.IgnorePatternWhitespace | RegexOptions.IgnoreCase);

            MatchCollection coll = regEx.Matches(source);
           return  regEx.Replace(source, new MatchEvaluator(MatchAggFn));
        }

        /// <exclude/>
        public void OnInitializeDataSet(Rdl.Runtime.InitializeDataSetEventArgs args)
        {
            if (InitializeDataSet != null)
                InitializeDataSet(this, args);
        }

        /// <exclude/>
        public void OnCredentialsPrompt(Rdl.Runtime.CredentialsPromptEventArgs args)
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

        /// <exclude/>
        public string Language(Rdl.Runtime.Context context)
        {
            return _language.ExecAsString(context);
        }

        public Size Width
        {
            get { return _width; }
        }

        /// <exclude/>
        public Size PageHeight
        {
            get { return _pageHeight; }
        }

        /// <exclude/>
        public Size PageWidth
        {
            get { return _pageWidth; }
        }

        /// <exclude/>
        public Size InteractiveHeight
        {
            get { return _interactiveHeight; }
        }

        /// <exclude/>
        public Size InteractiveWidth
        {
            get { return _interactiveWidth; }
        }

        /// <exclude/>
        public Size LeftMargin
        {
            get { return _leftMargin; }
        }

        /// <exclude/>
        public Size RightMargin
        {
            get { return _rightMargin; }
        }

        /// <exclude/>
        public Size TopMargin
        {
            get { return _topMargin; }
        }

        /// <exclude/>
        public Size BottomMargin
        {
            get { return _bottomMargin; }
        }

        internal PageHeaderFooter PageHeader
        {
            get { return _pageHeader; }
        }

        internal PageHeaderFooter PageFooter
        {
            get { return _pageFooter; }
        }

        /// <exclude/>
        public Dictionary<string, ReportItem> ReportItems
        {
            get { return _reportItems; }
        }

        /// <exclude/>
        public string XmlNameSpace
        {
            get { return _xmlNameSpace; }
        }

        internal EmbeddedImage GetEmbeddedImage(string name)
        {
            return _embeddedImages[name];
        }

        /// <exclude/>
        public string ReportPath
        {
            get { return _reportPath; }
        }
    }

    public static class ReportExtensions
    {
        public static string ToString(this Dictionary<string, ReportParameter> reportParameters, string separator)
        {
            string ret = string.Empty;
            foreach (ReportParameter parm in reportParameters.Values)
                ret += ((ret.Length > 0) ? separator : string.Empty) + parm.ToString();
            return ret;
        }

        public static string ToString(this string[] strings, string separator)
        {
            string ret = string.Empty;
            foreach (string s in strings)
                ret += ((ret.Length > 0) ? separator : string.Empty) + s;
            return ret;
        }
    }
}
