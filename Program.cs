using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace DemoCleaner3
{
    static class Program
    {
        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        static void Main(String[] argg)
        {
            AppDomain.CurrentDomain.AssemblyResolve += (sender, arg) => 
            {
                if (arg.Name.StartsWith("LinqBridge")) return Assembly.Load(Properties.Resources.LinqBridge);
                return null;
            };

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            FileInfo demoFile = null;
            if (argg.Length > 0) {
                demoFile = new FileInfo(argg[0]);
            }
            if (demoFile != null && demoFile.Exists && demoFile.Extension.ToLowerInvariant().StartsWith(".dm_")) {
                DemoInfoForm demoInfoForm = new DemoInfoForm();
                demoInfoForm.demoFile = new FileInfo(argg[0]);
                Application.Run(demoInfoForm);
            } else {
                Application.Run(new Form1());
            }
        }
    }

}
