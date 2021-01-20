using System;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace DemoCleaner3
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the app.
        /// </summary>
        [STAThread]
        static void Main(String[] argg) 
        {
            AppDomain.CurrentDomain.AssemblyResolve += (sender, arg) => {
                if (arg.Name.StartsWith("LinqBridge")) return Assembly.Load(Properties.Resources.LinqBridge);
                return null;
            };

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            FileInfo demoFile = null;
            bool xmlOutput = false;
            if (argg.Length == 1) {
                demoFile = new FileInfo(argg[0]);
            } else if ((argg.Length == 2) && (argg[0] == "--xml")) {
                demoFile = new FileInfo(argg[1]);
                xmlOutput = true;
            }
            if (demoFile != null && demoFile.Exists && demoFile.Extension.ToLowerInvariant().StartsWith(".dm_")) {
                DemoInfoForm demoInfoForm = new DemoInfoForm();
                demoInfoForm.demoFile = demoFile;
                if (xmlOutput) {
                    Demo demo = Demo.GetDemoFromFileRaw(demoFile);
                    var xmlString = XmlUtils.FriendlyInfoToXmlString(demo.rawInfo.getFriendlyInfo());
                    Console.WriteLine(xmlString);
                } else {
                    Application.Run(demoInfoForm);
                }
            } else {
                Application.Run(new Form1());
            }
        }
    }

}
