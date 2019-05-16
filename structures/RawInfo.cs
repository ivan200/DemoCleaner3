using DemoCleaner3.DemoParser.utils;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace DemoCleaner3.DemoParser.parser
{
    public class RawInfo
    {
        public static string keyDemoName = "demoname";
        public static string keyPlayer = "player";
        public static string keyClient = "client";
        public static string keyGame = "game";
        public static string keyRecord = "record";
        public static string keyRecordTime = "time";
        public static string keyRecordDate = "date";
        public static string keyRaw = "raw";

        public Dictionary<short, string> rawConfig;
        public List<string> performedTimes;
        public List<string> onlineTimes;
        public List<string> oldOfflineTimes;
        public List<string> oldOfflineTimes2;
        public List<string> dateStamps;
        public string demoPath;

        Dictionary<string, Dictionary<string, string>> friendlyInfo;

        public RawInfo(
            string demoName,
            Dictionary<short, string> rawConfig,
            List<string> dateStamps,
            List<string> performedTimes,
            List<string> onlineTimes,
            List<string> oldOfflineTimes,
            List<string> oldOfflineTimes2
            )
        {
            this.demoPath = demoName;
            this.rawConfig = rawConfig;
            this.performedTimes = performedTimes;
            this.dateStamps = dateStamps;
            this.onlineTimes = onlineTimes;
            this.oldOfflineTimes = oldOfflineTimes;
            this.oldOfflineTimes2 = oldOfflineTimes2;
        }

        public Dictionary<string, Dictionary<string, string>> getFriendlyInfo()
        {
            if (friendlyInfo != null) {
                return friendlyInfo;
            }

            friendlyInfo = new Dictionary<string, Dictionary<string, string>>();

            Dictionary<string, string> times = new Dictionary<string, string>();
            times.Add(keyDemoName, new FileInfo(demoPath).Name);

            if (dateStamps.Count > 0 || performedTimes.Count > 0 || onlineTimes.Count > 0 || oldOfflineTimes.Count > 0 || oldOfflineTimes2.Count > 0) {
                for (int i = 0; i < dateStamps.Count; i++) {
                    string key = dateStamps.Count > 1 ? keyRecordDate + " " + (i + 1) : keyRecordDate;
                    times.Add(key, dateStamps[i].Replace("print ", ""));
                }
                for (int i = 0; i < performedTimes.Count; i++) {
                    string key = performedTimes.Count > 1 ? keyRecordTime + " " + (i + 1) : keyRecordTime;
                    times.Add(key, performedTimes[i].Replace("print ", ""));
                }
                for (int i = 0; i < onlineTimes.Count; i++) {
                    string key = onlineTimes.Count > 1 ? keyRecordTime + " " + (i + 1) : keyRecordTime;
                    times.Add(key, onlineTimes[i].Replace("print ", ""));
                }
                for (int i = 0; i < oldOfflineTimes.Count; i++) {
                    string key = oldOfflineTimes.Count > 1 ? keyRecordTime + " " + (i + 1) : keyRecordTime;
                    times.Add(key, oldOfflineTimes[i]);
                }
                for (int i = 0; i < oldOfflineTimes2.Count; i++) {
                    string key = oldOfflineTimes2.Count > 1 ? keyRecordTime + " " + (i + 1) : keyRecordTime;
                    times.Add(key, oldOfflineTimes2[i].Replace("print ", ""));
                }
            }
            friendlyInfo.Add(keyRecord, times);

            if (rawConfig == null) {
                return friendlyInfo;
            }

            if (rawConfig.ContainsKey(Q3Const.Q3_DEMO_CFG_FIELD_PLAYER)) {
                friendlyInfo.Add(keyPlayer, Q3Utils.split_config(rawConfig[Q3Const.Q3_DEMO_CFG_FIELD_PLAYER]));
            } else {
                for (int i = 1; i < 5; i++) {
                    var key = (short)(Q3Const.Q3_DEMO_CFG_FIELD_PLAYER + i);
                    if (rawConfig.ContainsKey(key)) {
                        friendlyInfo.Add(keyPlayer, Q3Utils.split_config(rawConfig[key]));
                        break;
                    }
                }
            }

            if (rawConfig.ContainsKey(Q3Const.Q3_DEMO_CFG_FIELD_CLIENT)) {
                friendlyInfo.Add(keyClient, Q3Utils.split_config(rawConfig[Q3Const.Q3_DEMO_CFG_FIELD_CLIENT]));
            }
            if (rawConfig.ContainsKey(Q3Const.Q3_DEMO_CFG_FIELD_GAME)) {
                friendlyInfo.Add(keyGame, Q3Utils.split_config(rawConfig[Q3Const.Q3_DEMO_CFG_FIELD_GAME]));
            }

            Dictionary<string, string> raw = new Dictionary<string, string>();
            foreach (var r in rawConfig) {
                raw.Add(r.Key.ToString(), r.Value);
            }
            friendlyInfo.Add(keyRaw, raw);

            return friendlyInfo;
        }


        //получение времени из онлайн надписи
        public static string getOnlineName(string demoTimeCmd)
        {
            //print \"Rom^7 reached the finish line in ^23:38:208^7\n\"
            demoTimeCmd = Regex.Replace(demoTimeCmd, "(\\^[0-9]|\\\"|\\n|\")", "");          //print Rom reached the finish line in 3:38:208
            string name = demoTimeCmd.Substring(6, demoTimeCmd.LastIndexOf(" reached") - 6); //Rom
            return normalizeName(name);
        }

        public static TimeSpan getOnlineTimeSpan(string demoTimeCmd)
        {
            //print \"Rom^7 reached the finish line in ^23:38:208^7\n\"
            demoTimeCmd = Regex.Replace(demoTimeCmd, "(\\^[0-9]|\\\"|\\n|\")", "");          //print Rom reached the finish line in 3:38:208
            string demoTime = demoTimeCmd.Substring(demoTimeCmd.LastIndexOf("in") + 3);      //3:38:208
            return getTimeSpan(demoTime);
        }

        //получение времени из оффлайн надписи
        public static TimeSpan getTimeSpanForDemo(string demoTimeCmd)
        {
            //print "Time performed by ^2uN-DeaD!Enter^7 : ^331:432^7 (v1.91.23 beta)\n"

            //TODO проверить, что будет если в df_name будет со спецсимволами, например двоеточие, вопрос, кавычки
            demoTimeCmd = Regex.Replace(demoTimeCmd, "(\\^[0-9]|\\\"|\\n|\")", "");     //print Time performed  by uN-DeaD!Enter : 31:432 (v1.91.23 beta)

            demoTimeCmd = demoTimeCmd.Substring(demoTimeCmd.IndexOf(':') + 2);      //31:432 (v1.91.23 beta)
            demoTimeCmd = demoTimeCmd.Substring(0, demoTimeCmd.IndexOf(' ')).Trim();    //31:432

            return getTimeSpan(demoTimeCmd);
        }

        //получение времени из онлайн надписи
        public static string getOldOfflineName(string demoTimeCmd) {
            //NewTime -971299442 7:200 \"defrag 1.80\" \"Viper\" route ya->->rg
            demoTimeCmd = Regex.Replace(demoTimeCmd, "(\\^[0-9]|\\\"|\\n|\")", "");  //NewTime -971299442 7:200 defrag 1.80 Viper route ya->->rg
            var parts = demoTimeCmd.Split(' ');
            var name = parts[5];
            return normalizeName(name);
        }

        //получение времени из старой оффлайн надписи
        public static TimeSpan getTimeForOldText(string demoTimeCmd)
        {
            //NewTime -971299442 7:200 \"defrag 1.80\" \"Viper\" route ya->->rg
            demoTimeCmd = Regex.Replace(demoTimeCmd, "(\\^[0-9]|\\\"|\\n|\")", ""); //NewTime -971299442 7:200 defrag 1.80 Viper route ya->->rg
            var parts = demoTimeCmd.Split(' ');
            demoTimeCmd = parts[2];
            return getTimeSpan(demoTimeCmd);
        }

        //получение времени из строки
        public static TimeSpan getTimeSpan(string timeString)
        {
            var times = timeString.Split('-', '.', ':').Reverse().ToList();
            //так как мы реверснули таймы, милисекунды будут в начале
            return TimeSpan.Zero
                .Add(TimeSpan.FromMilliseconds(times.Count > 0 ? int.Parse(times[0]) : 0))
                .Add(TimeSpan.FromSeconds(times.Count > 1 ? int.Parse(times[1]) : 0))
                .Add(TimeSpan.FromMinutes(times.Count > 2 ? int.Parse(times[2]) : 0));
            //делаем через таймспаны чтобы если минут больше 60, они корректно добавлялись
        }

        //получение даты записи демки если она есть
        public static DateTime? getDateForDemo(string s)
        {
            //print "Date: 10-25-14 02:43\n"
            string dateString = s.Substring(13).Replace("\n", "").Replace("\"", "").Trim();
            try {
                return DateTime.ParseExact(dateString, "MM-dd-yy HH:mm", CultureInfo.InvariantCulture);
            } catch (FormatException) {
                try {
                    return DateTime.ParseExact(dateString, "MM-dd-yy H:mm", CultureInfo.InvariantCulture);
                } catch (FormatException) { }
            }
            return null;
        }

        //убираем цвета из раскрашеного ника
        public static string removeColors(string name)
        {
            return string.IsNullOrEmpty(name)
                ? name
                : Regex.Replace(name, "\\^.", "");
        }

        //имя которое можно использовать в названии файла
        public static string normalizeName(string name)
        {
            return string.IsNullOrEmpty(name)
                ? name
                : Regex.Replace(name, "[^a-zA-Z0-9\\!\\#\\$\\%\\&\\'\\(\\)\\+\\,\\-\\.\\;\\=\\[\\]\\^_\\{\\}]", "");
        }
    }
}
