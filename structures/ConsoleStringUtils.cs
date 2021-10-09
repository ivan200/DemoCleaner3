using System;
using System.Text.RegularExpressions;
using System.Globalization;
using System.Linq;

namespace DemoCleaner3.structures {
    public class ConsoleStringUtils {
        public static string getNameOnline(string demoTimeCmd) {
            //print \"Rom^7 reached the finish line in ^23:38:208^7\n\"
            demoTimeCmd = Regex.Replace(demoTimeCmd, "(\\^[0-9]|\\\"|\\n|\")", "");          //print Rom reached the finish line in 3:38:208
            string name = demoTimeCmd.Substring(6, demoTimeCmd.LastIndexOf(" reached") - 6); //Rom
            return DemoNames.normalizeName(name);
        }

        public static TimeSpan getTimeOnline(string demoTimeCmd) {
            //print \"Rom^7 reached the finish line in ^23:38:208^7\n\"
            demoTimeCmd = Regex.Replace(demoTimeCmd, "(\\^[0-9]|\\\"|\\n|\")", "");          //print Rom reached the finish line in 3:38:208
            string demoTime = demoTimeCmd.Substring(demoTimeCmd.LastIndexOf("in") + 3);      //3:38:208

            var estIndex = demoTime.IndexOf(" (est");
            if (estIndex > 0) {
                demoTime = demoTime.Substring(0, estIndex);
            }
            return getTimeSpan(demoTime);
        }

        public static TimeSpan getTimeOfflineNormal(string demoTimeCmd) {
            //print "Time performed by ^2uN-DeaD!Enter^7 : ^331:432^7 (v1.91.23 beta)\n"
            //print "Time performed by ^2GottWarLorD^7 : ^335:752^7 (defrag 1.9)\n"
            //print \"Time performed by Chell ^s: 00:54:184\n\"     //q3xp 2.1

            //TODO проверить, что будет если в df_name будет со спецсимволами, например двоеточие, вопрос, кавычки
            demoTimeCmd = Regex.Replace(demoTimeCmd, "(\\^.|\\\"|\\n|\")", "");     //print Time performed  by uN-DeaD!Enter : 31:432 (v1.91.23 beta)
            demoTimeCmd = demoTimeCmd.Substring(demoTimeCmd.IndexOf(':') + 2);      //31:432 (v1.91.23 beta)
            var spInd = demoTimeCmd.IndexOf(' ');
            if (spInd > 0) {
                demoTimeCmd = demoTimeCmd.Substring(0, spInd).Trim();    //31:432
            }

            return getTimeSpan(demoTimeCmd);
        }

        public static string getNameOffline(string demoTimeCmd) {
            //print "Time performed by ^2uN-DeaD!Enter^7 : ^331:432^7 (v1.91.23 beta)\n"
            //print \"Time performed by Chell ^s: 00:54:184\n\"
            demoTimeCmd = Regex.Replace(demoTimeCmd, "(\\^.|\\\"|\\n|\")", "");          //print Time performed  by uN-DeaD!Enter : 31:432 (v1.91.23 beta)
            demoTimeCmd = demoTimeCmd.Substring(24);                                         //uN-DeaD!Enter : 31:432 (v1.91.23 beta)
            demoTimeCmd = demoTimeCmd.Substring(0, demoTimeCmd.LastIndexOf(" : "));          //uN-DeaD!Enter
            return DemoNames.normalizeName(demoTimeCmd);
        }

        public static string getNameOfflineOld1(string demoTimeCmd) {
            //NewTime -971299442 7:200 \"defrag 1.80\" \"Viper\" route ya->->rg
            demoTimeCmd = Regex.Replace(demoTimeCmd, "(\\^[0-9]|\\\"|\\n|\")", "");  //NewTime -971299442 7:200 defrag 1.80 Viper route ya->->rg
            var parts = demoTimeCmd.Split(' ');
            var name = parts[5];
            return DemoNames.normalizeName(name);
        }

        public static TimeSpan getTimeOld1(string demoTimeCmd) {
            //NewTime -971299442 7:200 \"defrag 1.80\" \"Viper\" route ya->->rg
            demoTimeCmd = Regex.Replace(demoTimeCmd, "(\\^[0-9]|\\\"|\\n|\")", ""); //NewTime -971299442 7:200 defrag 1.80 Viper route ya->->rg
            var parts = demoTimeCmd.Split(' ');
            demoTimeCmd = parts[2];
            return getTimeSpan(demoTimeCmd);
        }

        public static TimeSpan getTimeOld3(string demoTimeCmd) {
            //newTime 47080
            demoTimeCmd = Regex.Replace(demoTimeCmd, "(\\^[0-9]|\\\"|\\n|\")", "");
            var parts = demoTimeCmd.Split(' ');
            demoTimeCmd = parts[1];
            int millis = 0;
            int.TryParse(demoTimeCmd, out millis);
            return TimeSpan.FromMilliseconds(millis);
        }



        /// <summary> Getting time from a string </summary>
        public static TimeSpan getTimeSpan(string timeString) {
            var times = timeString.Split('-', '.', ':').Reverse().ToList();
            if (times.Count > 0 && times[0].Length != 3) {
                return TimeSpan.Zero;
            }
            //since we reversed the times, milliseconds will be in the beginning
            return TimeSpan.Zero
                .Add(TimeSpan.FromMilliseconds(times.Count > 0 ? int.Parse(times[0]) : 0))
                .Add(TimeSpan.FromSeconds(times.Count > 1 ? int.Parse(times[1]) : 0))
                .Add(TimeSpan.FromMinutes(times.Count > 2 ? int.Parse(times[2]) : 0));
            //do through timespan so if minutes greater than 60, they are correctly added
        }

        /// <summary> Getting the date of recording of the demo if it exists </summary>
        public static DateTime? getDateForDemo(string s) {
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

    }
}
