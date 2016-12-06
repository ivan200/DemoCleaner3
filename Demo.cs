using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace DemoCleaner2
{
    public class Demo
    {
        public string mapName;
        public string modphysic;
        public string timeString;
        public TimeSpan time;
        public string playerName;
        public string country;
        public FileInfo file;
        public bool hasError;

        //Получаем из имени файла детали демки
        public static Demo GetDemoFromFile(FileInfo file)
        {
            Demo demo = new Demo();
            demo.file = file;

            var sub = file.Name.Split("[]()".ToArray());
            if (sub.Length >= 4)
            {
                //Карта
                demo.mapName = sub[0];

                //Физика
                demo.modphysic = sub[1];
                if (sub[1].Length < 3)
                {
                    demo.hasError = true;
                }

                //Время
                demo.timeString = sub[2];
                var times = demo.timeString.Split('-', '.');
                if (times.Length == 3)
                {
                    try
                    {
                        demo.time = new TimeSpan(0, 0,
                            int.Parse(times[0]),
                            int.Parse(times[1]),
                            int.Parse(times[2]));
                    }
                    catch (Exception)
                    {
                        demo.hasError = true;
                    }
                }
                else
                {
                    demo.hasError = true;
                }

                //Имя + страна
                var name = sub[3].Split('.');
                if (name.Length > 1)
                {
                    demo.playerName = name[0];
                    demo.country = name[1];
                }
                else
                {
                    demo.playerName = sub[3];
                }
            }
            else
            {
                demo.hasError = true;
            }
            return demo;
        }

        //обработка группировки, если нажата галка с обработкой мдф как дф
        public static string mdfToDf(string mod, bool processIt)
        {
            if (processIt && mod[0] == 'm')
            {
                return mod.Substring(1);
            }
            return mod;
        }

    }
}
