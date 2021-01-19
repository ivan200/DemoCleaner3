using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using System.Xml;

namespace DemoCleaner3
{
    static class Program
    {

        static XmlNode ExportXml(XmlDocument doc, Dictionary<String, System.Collections.Generic.Dictionary<string, string>> map)
        {
            var node = doc.CreateNode("element", "demoFile", "");
            foreach(var item in map)
            {
                var el = doc.CreateNode("element", item.Key, "");
                foreach(var subItem in item.Value)
                {
                    var attr = doc.CreateAttribute(subItem.Key.Replace(' ', '-'));
                    attr.Value = subItem.Value;
                    el.Attributes.Append(attr);
                }
                node.AppendChild(el);
            }
            return node;
        }

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
            bool xmlOutput = false;
            if (argg.Length == 1) {
                demoFile = new FileInfo(argg[0]);
            }else if ((argg.Length == 2) && (argg[0] == "--xml")) {
                demoFile = new FileInfo(argg[1]);
                xmlOutput = true;
            }
            if (demoFile != null && demoFile.Exists && demoFile.Extension.ToLowerInvariant().StartsWith(".dm_")) {
                DemoInfoForm demoInfoForm = new DemoInfoForm();
                demoInfoForm.demoFile = demoFile;
                if (xmlOutput) {
                    Demo demo = Demo.GetDemoFromFileRaw(demoFile);
                    XmlDocument doc = new XmlDocument();
                    Console.WriteLine(ExportXml(doc, demo.rawInfo.getFriendlyInfo()).OuterXml);
                } else {
                    Application.Run(demoInfoForm);
                }
            } else {
                Application.Run(new Form1());
            }
        }
    }

}
