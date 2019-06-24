using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;


namespace JournalWork
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            //EmbeddedAssembly.Load("JournalWork.FastReport.dll", "FastReport.dll");


            //AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(CurrentDomain_AssemblyResolve);

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }

        //static System.Reflection.Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        //{
        //    return EmbeddedAssembly.Get(args.Name);
        //}
    }
}
