﻿using DemoCleaner3.ExtClasses;
using System;
using System.IO;
using System.Windows.Forms;
using System.Text;
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
                        try {
                            var infoString = getDemoInfoString(demoFile);
                            Console.WriteLine(infoString);
                        } catch (Exception ex) {
                            Console.WriteLine("Can not parse demo");
                            Console.WriteLine(ex.ToString());
                        }
                        break;
                    case "--xml-file":
                        if (extraFile == null) {
                            string extension = ".xml";
                            var name = demoFile.Name.Substring(0, demoFile.Name.Length - demoFile.Extension.Length) + extension;
                            var m_exePath = Path.GetDirectoryName(Application.ExecutablePath);
                            extraFile = new FileInfo(Path.Combine(m_exePath, name));
                        }
                        var fileStream = new FileStream(extraFile.FullName, FileMode.CreateNew);
                        var streamWriter = new StreamWriter(fileStream, Encoding.UTF8);
                        try {
                            var infoString = getDemoInfoString(demoFile);
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

        static string getDemoInfoString(FileInfo demoFile) {
            Demo demo = Demo.GetDemoFromFileRaw(demoFile);

            Dictionary<string, string> name = new Dictionary<string, string>();
            name.Add("originalFileName", demoFile.Name);
            name.Add("suggestedFileName", demo.demoNewName);
            name.Add("suggestedFileNameSimple", demo.demoNewNameSimple);

            var friendlyInfo = demo.rawInfo.getFriendlyInfo();
            friendlyInfo.Add("fileName", name);

            return XmlUtils.FriendlyInfoToXmlString(friendlyInfo);
        }
    }
}
