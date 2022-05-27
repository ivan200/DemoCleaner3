using DemoCleaner3.ExtClasses;
using System;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace DemoCleaner3
{
    static class Program
    {
        enum RunType {
            DEFAULT,
            XML,
            REC,
            JSON
        }

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
            RunType runType = RunType.DEFAULT;

            if (argg.Length == 1) {
                demoFile = new FileInfo(argg[0]);
            } else if (argg.Length == 2 && argg[0] == "--xml") {
                demoFile = new FileInfo(argg[1]);
                runType = RunType.XML;
            } else if (argg.Length == 2 && argg[0] == "--json") {
                demoFile = new FileInfo(argg[1]);
                runType = RunType.JSON;
            } else if ((argg.Length == 3 || argg.Length == 2) && argg[0] == "--rec") {
                demoFile = new FileInfo(argg[1]);
                runType = RunType.REC;
            }
            if (demoFile != null && demoFile.Exists && demoFile.Extension.ToLowerInvariant().StartsWith(".dm_")) {
                Demo demo = null;
                switch (runType) {
                    case RunType.DEFAULT:
                        DemoInfoForm demoInfoForm = new DemoInfoForm();
                        demoInfoForm.demoFile = demoFile;
                        Application.Run(demoInfoForm);
                        break;
                    case RunType.XML:
                        try {
                            demo = Demo.GetDemoFromFileRaw(demoFile);
                            var xmlString = XmlUtils.FriendlyInfoToXmlString(demo.rawInfo.getFriendlyInfo());
                            Console.WriteLine(xmlString);
                        } catch (Exception ex) {
                            Console.WriteLine("Can not parse demo");
                            Console.WriteLine(ex.ToString());
                        }
                        break;
                    case RunType.JSON:
                        try
                        {
                            demo = Demo.GetDemoFromFileRaw(demoFile);

                            Dictionary<string, string> name = new Dictionary<string, string>();
                            name.Add("new", demo.demoNewName);
                            name.Add("old", demoFile.Name);

                            Dictionary<string, Dictionary<string, string>> friendlyInfo = demo.rawInfo.getFriendlyInfo();
                            friendlyInfo.Add("name", name);

                            var jsonString = JsonConvert.SerializeObject(friendlyInfo);
                            Console.Out.WriteLine(jsonString);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Can not parse demo");
                            Console.WriteLine(ex.ToString());
                        }
                        break;
                    case RunType.REC:
                        try {
                            demo = Demo.GetDemoFromFileRaw(demoFile);
                        } catch (Exception ex) {
                            Console.WriteLine("Can not parse demo");
                        }
                        if (demo != null) {
                            var saver = new RecFileSaver(demo);
                            if (saver.canSave) {
                                try {
                                    if (argg.Length == 3) {
                                        saver.Save(argg[2]);
                                    }
                                    if (argg.Length == 2) {
                                        saver.Save();
                                    }
                                    System.Diagnostics.Debug.WriteLine("rec file saved");
                                } catch (Exception ex) {
                                    Console.WriteLine(ex.Message);
                                }
                            }
                            Console.WriteLine("Can not create rec file for current demo");
                        }
                        break;
                }
            } else {
                Application.Run(new Form1());
            }
        }
    }
}
