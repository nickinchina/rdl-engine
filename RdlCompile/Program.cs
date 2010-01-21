using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace RdlCompile
{
    /// <summary>
    /// Compile an RDL report into an assembly.
    /// Usage:
    ///     RdlCompile -rreport [-nname] [-ooutput]
    ///     Compiles the given report into a DLL.
    ///     The name if specified will define the class name.  If not specified the class name will be derived from the report name
    /// The output specifies the output location of the DLL.  If not specified the DLL will be named the same as the report in the same directory
    /// </summary>
    class Program
    {
        static int Main(string[] args)
        {
            Args programArgs = new Args(args);
            if (!programArgs.Complete)
            {
                Args.ShowArgs();
                return 1;
            }

            Rdl.Engine.Report rpt = new Rdl.Engine.Report();
            try
            {
                rpt.CompileToAssembly(
                    new FileStream(programArgs.Report, FileMode.Open, FileAccess.Read, FileShare.Read),
                    programArgs.Path,
                    programArgs.OutputName,
                    programArgs.ReportName);
            }
            catch (Rdl.Engine.Report.CompileException err)
            {
                Console.WriteLine(err.Message);
                foreach (string s in err.Output)
                    Console.WriteLine(s);
            }
            catch (Exception err)
            {
                Console.WriteLine(err.Message);
            }

            return 0;
        }
    }

    class Args
    {
        public string Report = null;
        public string ReportName = null;
        public string OutputName = null;
        public string Path = null;
        public bool Complete = false;

        public Args(string[] args)
        {
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i].Length < 2 || (args[i][0] != '-' && args[i][0] != '/'))
                    return;
                switch (args[i][1])
                {
                    case 'r':
                        Report = GetParam(args, ref i);
                        if (!System.IO.File.Exists(Report))
                        {
                            Console.WriteLine("File " + Report + " doesn't exist.");
                            return;
                        }
                        Path = System.IO.Path.GetDirectoryName(Report);
                        
                        string name = System.IO.Path.GetFileNameWithoutExtension(Report);
                        if (ReportName == null)
                            ReportName = name.Replace(' ', '_');
                        if (OutputName == null)
                            OutputName = Path + @"\" + name + ".dll";
                        break;
                    case 'n':
                        ReportName = GetParam(args, ref i);
                        break;
                    case 'o':
                        OutputName = GetParam(args, ref i);
                        break;
                    default:
                        return;
                }
            }
            Complete = true;
        }

        private string GetParam(string[] args, ref int index)
        {
            string ret;
            if (args[index].Length > 2)
                ret = args[index].Substring(2);
            else
                ret = args[++index];
            return ret;
        }

        public static void ShowArgs()
        {
            Console.WriteLine("Usage:\n" +
                "RdlCompile -rreport [-nname] [-ooutput]\n" +
                "Compiles the given report into a DLL.\n" +
                "The name if specified will define the class name.  If not specified the class name will be derived from the report name\n" +
                "The output specifies the output location of the DLL.  If not specified the DLL will be named the same as the report in the same directory\n");
        }
    }
}
