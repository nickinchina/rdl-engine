using System;
using System.Collections.Generic;
using System.Text;
using Rdl.Engine;
using System.IO;
using System.Data;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace rdlTestApp
{
    class Program
    {
        public delegate string fn();

        static void test(fn f)
        {
            Console.WriteLine("{0}", f());
        }

        [STAThread]
        static void Main(string[] args)
        {
            Form2 frm = new Form2();

            Application.Run(frm);

            Application.Exit();

        }
    }
}
