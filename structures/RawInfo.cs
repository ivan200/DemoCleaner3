using DemoCleaner3.DemoParser.utils;
using DemoCleaner3.ExtClasses;
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
        public static string keyConsole = "console";

        public Dictionary<short, string> rawConfig;

        public enum TimeType {
            OFFLINE_NORMAL,
            ONLINE_NORMAL,
            OFFLINE_OLD1,
            OFFLINE_OLD2,
            OFFLINE_OLD3
        }

        public ListMap<TimeType, string> allTimes = new ListMap<TimeType, string>();

        public List<string> dateStamps = new List<string>();
        public Dictionary<long, string> console = new Dictionary<long, string>();

        public string demoPath;

        Dictionary<string, Dictionary<string, string>> friendlyInfo;

        public RawInfo(
            string demoName,
            Dictionary<short, string> rawConfig,
            Dictionary<long, string> console) {
            this.demoPath = demoName;
            this.rawConfig = rawConfig;
            this.console = console;

            getTimes(console);
        }

        private void getTimes(Dictionary<long, string> consoleCommands) {
            foreach (var kv in consoleCommands) {
                var value = kv.Value;
                if (value.StartsWith("print")) {

                    //print "Date: 10-25-14 02:43\n"
                    if (value.StartsWith("print \"Date:")) {
                        dateStamps.Add(value);
                    }

                    //print "Time performed by ^2uN-DeaD!Enter^7 : ^331:432^7 (v1.91.23 beta)\n"
                    if (value.StartsWith("print \"Time performed by")) {
                        allTimes.Add(TimeType.OFFLINE_NORMAL, value);
                    }

                    //"print \"^3Time Performed: 25:912 (defrag 1.5)\n^7\""
                    if (value.StartsWith("print \"^3Time Performed:")) {
                        allTimes.Add(TimeType.OFFLINE_OLD2, value);
                    }

                    //print \"Rom^7 reached the finish line in ^23:38:208^7\n\"
                    if (value.Contains("reached the finish line in")) {
                        allTimes.Add(TimeType.ONLINE_NORMAL, value);
                    }

                } else if (value.StartsWith("NewTime")) {
                    //"NewTime -971299442 7:200 \"defrag 1.80\" \"Viper\" route ya->->rg"
                    allTimes.Add(TimeType.OFFLINE_OLD1, value);
                } else if (value.StartsWith("newTime")) {
                    //newTime 47080
                    allTimes.Add(TimeType.OFFLINE_OLD3, value);
                }
            }
        }

        public Dictionary<string, Dictionary<string, string>> getFriendlyInfo()
        {
            if (friendlyInfo != null) {
                return friendlyInfo;
            }

            friendlyInfo = new Dictionary<string, Dictionary<string, string>>();

            Dictionary<string, string> times = new Dictionary<string, string>();
            times.Add(keyDemoName, new FileInfo(demoPath).Name);

            if (dateStamps.Count > 0 || allTimes.Count > 0) {
                for (int i = 0; i < dateStamps.Count; i++) {
                    string key = dateStamps.Count > 1 ? keyRecordDate + " " + (i + 1) : keyRecordDate;
                    times.Add(key, dateStamps[i].Replace("print ", ""));
                }
                for (int i = 0; i < allTimes.Count; i++) {
                    string key = allTimes.Count > 1 ? keyRecordTime + " " + (i + 1) : keyRecordTime;
                    var text = allTimes[i].Value;
                    if (text.StartsWith("print ")){
                        text = text.Substring(6);
                    }
                    times.Add(key, text);
                }
            }
            friendlyInfo.Add(keyRecord, times);

            if (rawConfig == null) {
                return friendlyInfo;
            }

            var keyP = (short)(Q3Const.Q3_DEMO_CFG_FIELD_PLAYER);
            var playersConfigs = new List<string>();
            for (short i = 0; i < 32; i++) {
                var k1 = (short)(keyP + i);
                if (rawConfig.ContainsKey(k1)) {
                    playersConfigs.Add(rawConfig[k1]);
                }
            }
            if (playersConfigs.Count == 1) {
                friendlyInfo.Add(keyPlayer, split_config_player(playersConfigs[0]));
            } else {
                for (int i = 0; i < playersConfigs.Count; i++) {
                    friendlyInfo.Add(keyPlayer + " " + (i+1).ToString(), split_config_player(playersConfigs[i]));
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

            if (console.Count > 0) {
                Dictionary<string, string> conTexts = new Dictionary<string, string>();
                foreach(var kv in console) {
                    conTexts.Add(kv.Key.ToString(), removeColors(kv.Value.ToString()));
                }
                friendlyInfo.Add(keyConsole, conTexts);
            }
            return friendlyInfo;
        }

        public static Dictionary<string, string> split_config_player(string src) {
            var split = Q3Utils.split_config(src);
            Dictionary<string, string> replaces = new Dictionary<string, string>();
            replaces.Add("n", "name");
            replaces.Add("dfn", "df_name");
            replaces.Add("t", "team");
            replaces.Add("c1", "color1");
            replaces.Add("c2", "color2");
            replaces.Add("hc", "maxHealth");
            replaces.Add("w", "wins");
            replaces.Add("l", "losses");
            replaces.Add("tt", "teamTask");
            replaces.Add("tl", "teamLeader");

            Dictionary<string, string> res = new Dictionary<string, string>();
            foreach (var str in split) {
                if (replaces.ContainsKey(str.Key)) {
                    res[replaces[str.Key]] = str.Value;
                } else {
                    res[str.Key] = str.Value;
                }
            }
            return res;
        }


        public static string getNameOnline(string demoTimeCmd)
        {
            //print \"Rom^7 reached the finish line in ^23:38:208^7\n\"
            demoTimeCmd = Regex.Replace(demoTimeCmd, "(\\^[0-9]|\\\"|\\n|\")", "");          //print Rom reached the finish line in 3:38:208
            string name = demoTimeCmd.Substring(6, demoTimeCmd.LastIndexOf(" reached") - 6); //Rom
            return normalizeName(name);
        }

        public static TimeSpan getTimeOnline(string demoTimeCmd)
        {
            //print \"Rom^7 reached the finish line in ^23:38:208^7\n\"
            demoTimeCmd = Regex.Replace(demoTimeCmd, "(\\^[0-9]|\\\"|\\n|\")", "");          //print Rom reached the finish line in 3:38:208
            string demoTime = demoTimeCmd.Substring(demoTimeCmd.LastIndexOf("in") + 3);      //3:38:208
            return getTimeSpan(demoTime);
        }

        public static TimeSpan getTimeOfflineNormal(string demoTimeCmd)
        {
            //print "Time performed by ^2uN-DeaD!Enter^7 : ^331:432^7 (v1.91.23 beta)\n"

            //TODO проверить, что будет если в df_name будет со спецсимволами, например двоеточие, вопрос, кавычки
            demoTimeCmd = Regex.Replace(demoTimeCmd, "(\\^[0-9]|\\\"|\\n|\")", "");     //print Time performed  by uN-DeaD!Enter : 31:432 (v1.91.23 beta)

            demoTimeCmd = demoTimeCmd.Substring(demoTimeCmd.IndexOf(':') + 2);      //31:432 (v1.91.23 beta)
            demoTimeCmd = demoTimeCmd.Substring(0, demoTimeCmd.IndexOf(' ')).Trim();    //31:432

            return getTimeSpan(demoTimeCmd);
        }

        public static string getNameOffline(string demoTimeCmd)
        {
            //print "Time performed by ^2uN-DeaD!Enter^7 : ^331:432^7 (v1.91.23 beta)\n"
            demoTimeCmd = Regex.Replace(demoTimeCmd, "(\\^[0-9]|\\\"|\\n|\")", "");          //print Time performed  by uN-DeaD!Enter : 31:432 (v1.91.23 beta)
            demoTimeCmd = demoTimeCmd.Substring(24);                                         //uN-DeaD!Enter : 31:432 (v1.91.23 beta)
            demoTimeCmd = demoTimeCmd.Substring(0, demoTimeCmd.LastIndexOf(" : "));          //uN-DeaD!Enter
            return normalizeName(demoTimeCmd);
        }

        public static string getNameOfflineOld1(string demoTimeCmd) {
            //NewTime -971299442 7:200 \"defrag 1.80\" \"Viper\" route ya->->rg
            demoTimeCmd = Regex.Replace(demoTimeCmd, "(\\^[0-9]|\\\"|\\n|\")", "");  //NewTime -971299442 7:200 defrag 1.80 Viper route ya->->rg
            var parts = demoTimeCmd.Split(' ');
            var name = parts[5];
            return normalizeName(name);
        }

        public static TimeSpan getTimeOld1(string demoTimeCmd)
        {
            //NewTime -971299442 7:200 \"defrag 1.80\" \"Viper\" route ya->->rg
            demoTimeCmd = Regex.Replace(demoTimeCmd, "(\\^[0-9]|\\\"|\\n|\")", ""); //NewTime -971299442 7:200 defrag 1.80 Viper route ya->->rg
            var parts = demoTimeCmd.Split(' ');
            demoTimeCmd = parts[2];
            return getTimeSpan(demoTimeCmd);
        }

        public static TimeSpan getTimeOld3(string demoTimeCmd)
        {
            //newTime 47080
            demoTimeCmd = Regex.Replace(demoTimeCmd, "(\\^[0-9]|\\\"|\\n|\")", ""); 
            var parts = demoTimeCmd.Split(' ');
            demoTimeCmd = parts[1];
            int millis = 0;
            int.TryParse(demoTimeCmd, out millis);
            return TimeSpan.FromMilliseconds(millis);
        }

        //getting time from a string
        public static TimeSpan getTimeSpan(string timeString)
        {
            var times = timeString.Split('-', '.', ':').Reverse().ToList();
            //since we reversed the times, milliseconds will be in the beginning
            return TimeSpan.Zero
                .Add(TimeSpan.FromMilliseconds(times.Count > 0 ? int.Parse(times[0]) : 0))
                .Add(TimeSpan.FromSeconds(times.Count > 1 ? int.Parse(times[1]) : 0))
                .Add(TimeSpan.FromMinutes(times.Count > 2 ? int.Parse(times[2]) : 0));
            //do through timespan to if minutes greater than 60, they are correctly added
        }

        //getting the date of recording of the demo if it exists
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

        //remove the color from the painted nick
        public static string removeColors(string name)
        {
            return string.IsNullOrEmpty(name)
                ? name
                : Regex.Replace(name, "\\^.", "");
        }

        //name that can be used in the file name
        public static string normalizeName(string name)
        {
            return string.IsNullOrEmpty(name)
                ? name
                : Regex.Replace(name, "[^a-zA-Z0-9\\!\\#\\$\\%\\&\\'\\(\\)\\+\\,\\-\\.\\;\\=\\[\\]\\^_\\{\\}]", "");
        }
    }
}
