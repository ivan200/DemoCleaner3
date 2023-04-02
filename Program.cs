using DemoCleaner3.ExtClasses;
using System;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using System.Text;
using Newtonsoft.Json;
using System.Collections.Generic;

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
                if (arg.Name.StartsWith("Newtonsoft.Json")) return Assembly.Load(Properties.Resources.Newtonsoft_Json);
                return null;
            };

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            FileInfo demoFile = null;
            string argument = null;
            FileInfo extraFile = null;

            if (argg.Length == 1) {
                demoFile = new FileInfo(argg[0]);
            } else if (argg.Length == 2) {
                argument = argg[0];
                demoFile = new FileInfo(argg[1]);
            } else if (argg.Length == 3) {
                argument = argg[0];
                demoFile = new FileInfo(argg[1]);
                extraFile = new FileInfo(argg[2]);
            }
            if (demoFile != null && demoFile.Exists && demoFile.Extension.ToLowerInvariant().StartsWith(".dm_")) {
                Demo demo = null;
                switch (argument) {
                    default:
                        DemoInfoForm demoInfoForm = new DemoInfoForm();
                        demoInfoForm.demoFile = demoFile;
                        Application.Run(demoInfoForm);
                        break;
                    case "--xml":
                    case "--json":
                        try {
                            var infoString = getDemoInfoString(demoFile, argument == "--xml");
                            Console.WriteLine(infoString);
                        } catch (Exception ex) {
                            Console.WriteLine("Can not parse demo");
                            Console.WriteLine(ex.ToString());
                        }
                        break;
                    case "--xml-file":
                    case "--json-file":
                        var isXml = argument == "--xml-file";
                        if (extraFile == null) {
                            string extension;
                            if (isXml) { extension = ".xml"; } else { extension = ".json"; }
                            var name = demoFile.Name.Substring(0, demoFile.Name.Length - demoFile.Extension.Length) + extension;
                            var m_exePath = Path.GetDirectoryName(Application.ExecutablePath);
                            extraFile = new FileInfo(Path.Combine(m_exePath, name));
                        }
                        var fileStream = new FileStream(extraFile.FullName, FileMode.CreateNew);
                        var streamWriter = new StreamWriter(fileStream, Encoding.UTF8);
                        try {
                            var infoString = getDemoInfoString(demoFile, isXml);
                            streamWriter.Write(infoString);
                        } catch (Exception ex) {
                            streamWriter.Write("Can not parse demo: " + ex.Message);
                        }
                        streamWriter.Close();
                        fileStream.Close();
                        break;
                    case "--rec":
                        try {
                            demo = Demo.GetDemoFromFileRaw(demoFile);
                        } catch (Exception ex) {
                            Console.WriteLine("Can not parse demo");
                            Console.WriteLine(ex.ToString());
                        }
                        if (demo != null) {
                            var saver = new RecFileSaver(demo);
                            if (saver.canSave) {
                                try {
                                    if (extraFile != null) {
                                        saver.Save(extraFile.FullName);
                                    } else {
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

        static string getDemoInfoString(FileInfo demoFile, bool xml) {
            Demo demo = Demo.GetDemoFromFileRaw(demoFile);

            Dictionary<string, string> name = new Dictionary<string, string>();
            name.Add("originalFileName", demoFile.Name);
            name.Add("suggestedFileName", demo.demoNewName);
            name.Add("suggestedFileNameSimple", demo.demoNewNameSimple);

            var friendlyInfo = demo.rawInfo.getFriendlyInfo();
            friendlyInfo.Add("fileName", name);

            if (xml) {
                return XmlUtils.FriendlyInfoToXmlString(friendlyInfo);
            } else {
                return JsonConvert.SerializeObject(friendlyInfo, Formatting.Indented);
            }
        }
    }
}
