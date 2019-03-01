using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using DemoCleaner2.DemoParser.parser;
using System.Text.RegularExpressions;
using System.Globalization;

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
        public DateTime recordTime;

        string dfType;
        string physic;
        string modNum;
        bool cheats;

        public string demoNewName {
            get {
                string demoname = "";
                if (time.TotalMilliseconds > 0)
                {   
                    //если есть тайм, то пишем нормальный name для демки
                    demoname = string.Format("{0}[{1}]{2:D2}.{3:D2}.{4:D3}({5})",
                    mapName, modphysic, time.Minutes, time.Seconds, time.Milliseconds, playerName);
                }
                else
                {
                    //если нет тайма, то мучаемся с генерацией текста
                    string oldName = file.Name;
                    oldName = oldName.Substring(0, oldName.Length - file.Extension.Length); //убираем расширение
                    oldName = removeSubstr(oldName, mapName);                               //убираем имя карты
                    oldName = removeSubstr(oldName, playerName);                            //убираем имя игрока
                    oldName = removeSubstr(oldName, physic);                                //убираем физику
                    oldName = removeDouble(oldName);                                        //убираем двойные символы
                    oldName = Regex.Replace(oldName, "(^[^[a-zA-Z0-9\\(\\)\\]\\[]|[^[a-zA-Z0-9\\(\\)\\]\\[]$)", "");    //убираем хрень в начале и в конце
                    if (oldName.Length > 0)
                    {
                        oldName = "_" + oldName;    //добавляем инфу о читах если они есть
                    }
                    demoname = string.Format("_{0}[{1}]({2}){3}",
                    mapName, modphysic, playerName, oldName);
                }

                if (cheats)
                {
                    demoname = demoname + "_cheats";
                }
                return demoname + file.Extension;
            }
        }

        //убирание двойных небуквенных символов и замена на первый
        //например: test__abc-_.xy -> test_abc-xy
        string removeDouble(string input)
        {
            var dup = Regex.Match(input, "[^[a-zA-Z0-9\\(\\)\\]\\[]{2,}");
            if (dup.Success && dup.Groups.Count > 0)
            {
                var symbol = dup.Groups[0].Value[0];
                return removeDouble(input.Substring(0, dup.Groups[0].Index) + symbol + input.Substring(dup.Groups[0].Index + dup.Groups[0].Length));
            }
            else {
                return input;
            }
        }

        //убирание подстроки с граничащими символами например: test_abc.xy -> test.xy
        //берётся последний символ если их 2, и первый если только слева: test_abcxy -> test_xy
        string removeSubstr(string input, string include) {
            if (!input.Contains(include)) {
                return input;
            }
            var symbol = "";

            int cropstart = 0;
            int cropend = 0;
            int pos = input.IndexOf(include);
            if (pos > 0) {
                symbol = input[pos - 1] + "";
                cropstart = char.IsLetterOrDigit(input[pos - 1]) ? 0 : 1;
            }
            if (pos + include.Length + 1 < input.Length) {
                cropend = char.IsLetterOrDigit(input[pos + include.Length]) ? 0 : 1;
                symbol = input[pos + include.Length] + "";
            }
            return input.Substring(0, pos - cropstart) + symbol + input.Substring(pos + include.Length + cropend);
        }

        //Получаем из имени файла детали демки
        public static Demo GetDemoFromFile(FileInfo file)
        {
            Demo demo = new Demo();
            demo.file = file;
            demo.recordTime = demo.file.CreationTime;

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


        //Получаем детали демки из детальной инфы выдернутой из демки
        public static Demo GetDemoFromRawInfo(RawInfo raw)
        {
            var file = new FileInfo(raw.demoPath);

            var frConfig = raw.getFriendlyInfo();

            Demo demo = new Demo();

            //файл
            demo.file = file;

            //Имя
            demo.playerName = frConfig[RawInfo.keyPlayer]["dfn"];

            //Карта
            demo.mapName = frConfig[RawInfo.keyClient]["mapname"];

            //время
            if (raw.performedTimes.Count > 0)
            {
                double maxMillis = double.MaxValue;
                for (int i = 0; i < raw.performedTimes.Count; i++)
                {
                    var time = getTimeSpanForDemo(raw.performedTimes[i]);
                    if (time.TotalMilliseconds < maxMillis) {
                        demo.time = time;
                        if (i < raw.dateStamps.Count) {
                            demo.recordTime = getDateForDemo(raw.dateStamps[i]);
                        }
                    }
                }
            }
            else if (raw.onlineTimes.Count > 0)
            {
                double maxMillis = double.MaxValue;
                foreach (var timeString in raw.onlineTimes)
                {
                    var time = getOnlineTimeSpanForDemo(timeString, demo.playerName);
                    if (time.HasValue && time.Value.TotalMilliseconds < maxMillis)
                    {
                        demo.time = time.Value;
                    }
                }
            }

            //геймтайп
            int gType = int.Parse(frConfig[RawInfo.keyClient]["defrag_gametype"]);
            switch (gType) {
                case 1: demo.dfType = "df"; break;
                case 2: demo.dfType = "fs"; break;
                case 3: demo.dfType = "fc"; break;

                case 5: demo.dfType = "mdf"; break;
                case 6: demo.dfType = "mfs"; break;
                case 7: demo.dfType = "mfc"; break;
            }

            //промод
            var promode = frConfig[RawInfo.keyClient]["df_promode"];
            demo.physic = int.Parse(promode) == 1 ? "cpm" : "vq3";                  //vq3, cpm

            //мод для fastcaps и freestyle
            var gametype = int.Parse(frConfig[RawInfo.keyClient]["defrag_mode"]);   //>=0 = mode
            demo.modNum = (gType != 1 && gType != 5) ? string.Format(".{0}", gametype) : "";         //.0 - .7

            //комбинируем
            demo.modphysic = string.Format("{0}.{1}{2}", demo.dfType, demo.physic, demo.modNum);

            //если есть читы, то пишем в демку
            demo.cheats = int.Parse(frConfig[RawInfo.keyGame]["sv_cheats"]) > 0;
            return demo;
        }



        //получение времени из онлайн надписи
        static TimeSpan? getOnlineTimeSpanForDemo(string demoTimeCmd, string dfName)
        {
            //print \"Rom^7 reached the finish line in ^23:38:208^7\n\"
            demoTimeCmd = Regex.Replace(demoTimeCmd, "(\\^[0-9]|\\\"|\\n|\")", "");          //print Rom reached the finish line in 3:38:208
            string name = demoTimeCmd.Substring(6, demoTimeCmd.LastIndexOf(" reached") - 6); //Rom
            string demoTime = demoTimeCmd.Substring(demoTimeCmd.LastIndexOf("in") + 3);      //3:38:208

            if (!name.Equals(dfName)) {
                return null;
            }
            return getTimeSpan(demoTime);
        }

        //получение времени из онффлайн надписи
        static TimeSpan getTimeSpanForDemo(string demoTimeCmd) {
            //print "Time performed by ^2uN-DeaD!Enter^7 : ^331:432^7 (v1.91.23 beta)\n"

            //TODO проверить, что будет если в df_name будет со спецсимволами, например двоеточие, вопрос, кавычки
            demoTimeCmd = Regex.Replace(demoTimeCmd, "(\\^[0-9]|\\\"|\\n|\")", "");     //print Time performed  by uN-DeaD!Enter : 31:432 (v1.91.23 beta)

            demoTimeCmd = demoTimeCmd.Substring(demoTimeCmd.IndexOf(':') + 2);      //31:432 (v1.91.23 beta)
            demoTimeCmd = demoTimeCmd.Substring(0, demoTimeCmd.IndexOf(' ')).Trim();    //31:432

            return getTimeSpan(demoTimeCmd);
        }

        static TimeSpan getTimeSpan(string timeString) {
            var times = timeString.Split(':').Reverse().ToList();
            //так как мы реверснули таймы, милисекунды будут в начале
            return new TimeSpan(0, 0,
                        times.Count > 2 ? int.Parse(times[2]) : 0,
                        times.Count > 1 ? int.Parse(times[1]) : 0,
                        times.Count > 0 ? int.Parse(times[0]) : 0);
        }

        //получение даты записи демки если она есть
        static DateTime getDateForDemo(string s) {
            //print "Date: 10-25-14 02:43\n"
            string dateString = s.Substring(13).Replace("\n", "").Replace("\"", "").Trim();

            return DateTime.ParseExact(dateString, "MM-dd-yy HH:mm", CultureInfo.InvariantCulture);
        }
    }
}
