using DemoCleaner3.DemoParser.structures;
using DemoCleaner3.DemoParser.utils;
using DemoCleaner3.ExtClasses;
using DemoCleaner3.structures;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using static DemoCleaner3.DemoParser.utils.Q3Utils;

namespace DemoCleaner3.DemoParser.parser
{
    public class RawInfo
    {
        public static string keyDemoName = "demoname";
        public static string keyPlayer = "player";
        public static string keyPlayerNum = "playerNum";
        public static string keyGameInfo = "gameInfo";
        public static string keyClient = "client";
        public static string keyGame = "game";
        public static string keyRecord = "record";
        public static string keyTriggers = "triggers";
        public static string keyRecordTime = "time";
        public static string keyRecordDate = "date";
        public static string keyRaw = "raw";
        public static string keyConsole = "console";
        public static string keyErrors = "errors";
        public static string keyBestTime = "bestTime";
        public static string keyMaxSpeed = "maxSpeed";

        public Dictionary<short, string> rawConfig;

        public enum TimeType {
            OFFLINE_NORMAL,
            ONLINE_NORMAL,
            OFFLINE_OLD1,
            OFFLINE_OLD2,
            OFFLINE_OLD3
        }

        public ListMap<TimeType, string> allTimes = new ListMap<TimeType, string>();
        public List<TimeStringInfo> timeStrings = new List<TimeStringInfo>();

        public List<string> dateStamps = new List<string>();
        
        public string demoPath;
        public ClientConnection clc;
        public List<ClientEvent> clientEvents = new List<ClientEvent>();
        Dictionary<string, string> playerConfig = new Dictionary<string, string>();
        Dictionary<string, Dictionary<string, string>> friendlyInfo;

        Dictionary<short, Dictionary<string, string>> allPlayersConfigs = new Dictionary<short, Dictionary<string, string>>();

        public KeyValuePair<int, ClientEvent>? fin = null;

        public Dictionary<string, string> kPlayer = null;
        public GameInfo gameInfo = null;
        public int maxSpeed = 0;
        public bool isLongStart = false;

        public RawInfo(string demoName, ClientConnection clientConnection, List<ClientEvent> clientEvents, int maxSpeed) {
            this.demoPath = demoName;
            this.clc = clientConnection;
            this.rawConfig = clientConnection.configs;
            this.clientEvents = clientEvents;
            this.fin = getCorrectFinishEvent();
            this.maxSpeed = maxSpeed;

            fillTimes(clientConnection.console);
            timeStrings = getTimeStrings();
        }

        public Dictionary<string, Dictionary<string, string>> getFriendlyInfo() {
            if (friendlyInfo != null) {
                return friendlyInfo;
            }

            //All players
            var keyP = Q3Const.Q3_DEMO_CFG_FIELD_PLAYER;
            for (short i = 0; i < 32; i++) {
                var k1 = (short)(keyP + i);
                if (rawConfig.ContainsKey(k1)) { 
                    allPlayersConfigs.Add(k1, split_config_player(rawConfig[k1]));
                }
            }

            //Current player
            if (fin.HasValue) {
                kPlayer = getPlayerInfoByPlayerNum(fin.Value.Value.playerNum);
            }
            if (kPlayer == null && clientEvents.Count > 0) {
                //if spectator view player and there was no finish
                var lastEvent = clientEvents.LastOrDefault();
                if (lastEvent != null) {
                    kPlayer = getPlayerInfoByPlayerNum(lastEvent.playerNum);
                }
            }
            if (kPlayer == null) {
                kPlayer = getPlayerInfoByPlayerNum(clc.clientNum);
            }
            var keys = allPlayersConfigs.Keys.ToList();
            if (kPlayer == null && keys.Count == 1) {
                kPlayer = allPlayersConfigs[keys[0]];
            }

            friendlyInfo = new Dictionary<string, Dictionary<string, string>>();
            
            //console Times
            Dictionary<string, string> times = new Dictionary<string, string>();
            times.Add(keyDemoName, new FileInfo(demoPath).Name);
            if (timeStrings.Count > 0) {
                var strInfo = GetGoodTimeStringInfo();
                if (strInfo != null) {
                    if (!string.IsNullOrEmpty(strInfo.recordDateString)) {
                        times.Add(keyRecordDate, strInfo.recordDateString);
                    }
                    times.Add(keyRecordTime, strInfo.timeString);
                } else {
                    for (int i = 0; i < timeStrings.Count; i++) {
                        var timeInfo = timeStrings[i];
                        if (!string.IsNullOrEmpty(timeInfo.recordDateString)) {
                            string keyDate = timeStrings.Count > 1 ? keyRecordDate + " " + (i + 1) : keyRecordDate;
                            times.Add(keyDate, timeInfo.recordDateString);
                        }
                        string keyTime = timeStrings.Count > 1 ? keyRecordTime + " " + (i + 1) : keyRecordTime;
                        times.Add(keyTime, timeInfo.timeString);
                    }
                }
            }
            if (fin != null) {
                string bestTime = getTimeByMillis(fin.Value.Value.time);
                bool hasTr = fin.Value.Key > 1;
                string trAdd = hasTr ? " (Time reset)" : "";
                times.Add(keyBestTime, bestTime + trAdd);
            }
            if (maxSpeed > 0) {
                times.Add(keyMaxSpeed, maxSpeed.ToString());
            }

            friendlyInfo.Add(keyRecord, times);

            //demo triggers
            if (clientEvents != null && clientEvents.Count > 0) {
                Dictionary<string, string> triggers = new Dictionary<string, string>();
                long startFileServerTime = 0;
                long startTimerServerTime = 0;
                try {
                    int stCount = 0;
                    int trCount = 0;
                    int cpCount = 0;
                    int ftCount = 0;
                    int pmCount = 0;
                    int cuCount = 0;
                    int tCount = 0;

                    for (int i = 0; i < clientEvents.Count; i++) {
                        ClientEvent ce = clientEvents[i];
                        string diff = "";
                        if (i > 0) {
                            var prev = clientEvents[i - 1];
                            long t = ce.serverTime - prev.serverTime;
                            if (t > 0 && prev.serverTime > 0) {
                                diff = string.Format(" (+{0})", getDiffByMillis(t));
                            }
                        }
                        if (ce.eventStartFile) {
                            var user = getPlayerInfoByPlayerNum(clc.clientNum);
                            if (user == null) {
                                user = getPlayerInfoByPlayerNum(ce.playerNum);
                            }
                            string username = user == null ? null : Ext.GetOrNull(user, "name");
                            string userString = string.IsNullOrEmpty(username) ? "" : "Client: " + username;
                            triggers.Add("StartFile", userString);
                            startFileServerTime = ce.serverTime;
                        }
                        if (ce.eventStartTime) {
                            var user = getPlayerInfoByPlayerNum(ce.playerNum);
                            string username = user == null ? null : Ext.GetOrNull(user, "name");
                            string userString = string.IsNullOrEmpty(username) ? "" : "Player: " + username;
                            triggers.Add("StartTimer" + getNumKey(++stCount), userString + diff);

                            //check only first start timer
                            if (startTimerServerTime == 0) {
                                startTimerServerTime = ce.serverTime;
                            }
                        }
                        if (ce.eventTimeReset) {
                            var user = getPlayerInfoByPlayerNum(ce.playerNum);
                            string username = user == null ? null : Ext.GetOrNull(user, "name");
                            string userString = string.IsNullOrEmpty(username) ? "" : "Player: " + username;
                            triggers.Add("TimeReset" + getNumKey(++trCount), userString + diff);
                        }
                        if (ce.eventFinish) {
                            triggers.Add("FinishTimer" + getNumKey(++ftCount), getTimeByMillis(ce.time) + diff);
                        }
                        if (ce.eventCheckPoint) {
                            triggers.Add("CheckPoint" + getNumKey(++cpCount), getTimeByMillis(ce.time) + diff);
                        }
                        if (ce.eventChangePmType) {
                            string pmString = ce.playerMode < ClientEvent.pmTypesStrings.Length
                                ? ClientEvent.pmTypesStrings[ce.playerMode] : ce.playerMode.ToString();
                            triggers.Add("ChangePlayerMode" + getNumKey(++pmCount), pmString + diff);
                        }
                        if (ce.eventSomeTrigger && ce.playerMode == (int)ClientEvent.PlayerMode.PM_NORMAL) {
                            triggers.Add("EventTrigger" + getNumKey(++tCount), getTimeByMillis(ce.time) + diff);
                        }
                        if (ce.eventChangeUser) {
                            var user = getPlayerInfoByPlayerNum(ce.playerNum);
                            string username = user == null ? null : Ext.GetOrNull(user, "name");
                            string userString = string.IsNullOrEmpty(username) ? "" : "Player: " + username;
                            triggers.Add("ChangeUser" + getNumKey(++cuCount), userString + diff);
                        }
                    }
                } catch (Exception ex) {
                    Q3Utils.PrintDebug(clc.errors, ErrorType.Unknown2);
                }

                //detection demos with very long start
                var timeMillis = startTimerServerTime - startFileServerTime;
                if (timeMillis > 0) {
                    var time = TimeSpan.FromMilliseconds(timeMillis);
                    if (time.TotalSeconds > 20) {

                        var sTime = TimeSpan.FromMilliseconds(startTimerServerTime);
                        var sTimeString = string.Format("{0:D2}:{1:D2}:{2:D3}", (int)sTime.TotalMinutes, sTime.Seconds, sTime.Milliseconds);

                        friendlyInfo[keyRecord].Add("lateStart", $"{(int)time.TotalSeconds} sec (servertime: {sTimeString})");
                        isLongStart = true;
                    }
                }

                friendlyInfo.Add(keyTriggers, triggers);
            }

            if (rawConfig == null) {
                return friendlyInfo;
            }

            //Player
            if (kPlayer != null) {
                friendlyInfo.Add(keyPlayer, kPlayer);
            } else {
                for (int i = 0; i < keys.Count; i++) {
                    friendlyInfo.Add(keyPlayer + " " + (i + 1).ToString(), allPlayersConfigs[keys[i]]);
                }
            }

            Dictionary<string, string> clInfo = null;
            Dictionary<string, string> gInfo = null;
            if (rawConfig.ContainsKey(Q3Const.Q3_DEMO_CFG_FIELD_CLIENT)) {
                clInfo = Q3Utils.split_config(rawConfig[Q3Const.Q3_DEMO_CFG_FIELD_CLIENT]);
            }
            if (rawConfig.ContainsKey(Q3Const.Q3_DEMO_CFG_FIELD_GAME)) {
                gInfo = split_config_game(rawConfig[Q3Const.Q3_DEMO_CFG_FIELD_GAME]);
            }

            //Gametype
            var parameters = Ext.Join(clInfo, gInfo);
            gameInfo = new GameInfo(parameters);
            var gameInfoDict = new Dictionary<string, string>();
            if (parameters.Count > 0) {
                bool diff = gameInfo.gameName.ToLowerInvariant() != gameInfo.gameNameShort.ToLowerInvariant();
                if (diff) {
                    gameInfoDict.Add("gameName", string.Format("{0} ({1})", gameInfo.gameName, gameInfo.gameNameShort));
                } else {
                    gameInfoDict.Add("gameName", gameInfo.gameName);
                }
                gameInfoDict.Add("gameType", string.Format("{0} ({1})", gameInfo.gameType, gameInfo.gameTypeShort));
                if (!string.IsNullOrEmpty(gameInfo.gameplayTypeShort)) {
                    gameInfoDict.Add("gameplay", string.Format("{0} ({1})", gameInfo.gameplayType, gameInfo.gameplayTypeShort));
                }
                if (!string.IsNullOrEmpty(gameInfo.modType)) {
                    gameInfoDict.Add("modType", string.Format("{0} ({1})", gameInfo.modTypeName, gameInfo.modType));
                }
            }

            //Game
            var game = Ext.Join(gameInfoDict, gInfo);
            if (game.Count > 0) {
                friendlyInfo.Add(keyGame, game);
            }

            //Client
            if (clInfo != null) {
                friendlyInfo.Add(keyClient, clInfo);
            }

            //Raw configs
            Dictionary<string, string> raw = new Dictionary<string, string>();
            foreach (var r in rawConfig) {
                raw.Add(r.Key.ToString(), r.Value);
            }
            friendlyInfo.Add(keyRaw, raw);

            //Console commands
            if (clc.console.Count > 0) {
                Dictionary<string, string> conTexts = new Dictionary<string, string>();
                foreach(var kv in clc.console) {
                    conTexts.Add(kv.Key.ToString(), removeColors(kv.Value.ToString()));
                }
                friendlyInfo.Add(keyConsole, conTexts);
            }
            if (clc.errors.Count > 0) {
                Dictionary<string, string> errTexts = new Dictionary<string, string>();
                int i = 0;
                foreach (var kv in clc.errors) {
                    errTexts.Add((++i).ToString(), kv.Key.ToString());
                }
                friendlyInfo.Add(keyErrors, errTexts);
            }

            return friendlyInfo;
        }

        TimeStringInfo GetGoodTimeStringInfo() {
            long time = 0;
            if (fin.HasValue) {
                time = fin.Value.Value.time;
            }
            var tmpNames = new DemoNames();
            tmpNames.setNamesByPlayerInfo(kPlayer);

            if (time > 0) {
                for (int i = 0; i < timeStrings.Count; i++) {
                    if (!string.IsNullOrEmpty(timeStrings[i].oName)) {
                        var sameName = (timeStrings[i].oName == tmpNames.uName || timeStrings[i].oName == tmpNames.dfName);
                        if (timeStrings[i].time.TotalMilliseconds == time && sameName) {
                            return timeStrings[i];
                        }
                    } else {
                        if (timeStrings[i].time.TotalMilliseconds == time) {
                            return timeStrings[i];
                        }
                    }
                }
            } else {
                var userStrings = timeStrings.Where(x => !string.IsNullOrEmpty(x.oName)
                    && (x.oName == tmpNames.uName || x.oName == tmpNames.dfName)).ToList();
                if (userStrings.Count > 0) {
                    return Ext.MinOf(userStrings, x => (long)x.time.TotalMilliseconds);
                }
            }
            return null;
        }


        static string getNumKey(int num) {
            if (num <= 1) {
                return "";
            } else {
                return " " + num.ToString();
            }
        }

        public static string getTimeByMillis(long millis) {
            var time = TimeSpan.FromMilliseconds(millis);
            return string.Format("{0:D2}.{1:D2}.{2:D3}", (int)time.TotalMinutes, time.Seconds, time.Milliseconds);
        }

        public static string getDiffByMillis(long diff) {
            var time = TimeSpan.FromMilliseconds(diff);
            if (time.Seconds < 1) {
                return string.Format("0.{0:D3}", time.Milliseconds);
            } else if ((int)time.TotalMinutes < 1) {
                return string.Format("{0}.{1:D3}", time.Seconds, time.Milliseconds);
            } else {
                return string.Format("{0}.{1:D2}.{2:D3}", (int)time.TotalMinutes, time.Seconds, time.Milliseconds);
            }
        }


        public Dictionary<string, string> getPlayerInfoByPlayerNum(long clientNum) {
            var num = (short)(Q3Const.Q3_DEMO_CFG_FIELD_PLAYER + clientNum);
            if (allPlayersConfigs.ContainsKey(num)) {
                return allPlayersConfigs[num];
            }
            return null;
        }

        public Dictionary<string, string> getPlayerInfoByPlayerName(string playerName) {
            var allPlayersKeys = allPlayersConfigs.Keys.ToList();
            foreach (var key in allPlayersKeys) {
                var userName = Ext.GetOrNull(allPlayersConfigs[key], "name");
                if (!string.IsNullOrEmpty(userName) && !string.IsNullOrEmpty(playerName) && userName == playerName) {
                    return allPlayersConfigs[key];
                }
            }
            return null;
        }


        public static void replaceKeys(ListMap<string, string> src, Dictionary<string, string> replaces) {
            for (int i = 0; i < src.Count; i++) {
                var str = src[i];
                if (replaces.ContainsKey(str.Key.ToLowerInvariant())) {
                    src[i] = new KeyValuePair<string, string>(replaces[str.Key], str.Value);
                }
            }
        }

        public static Dictionary<string, string> split_config_game(string src) {
            var split = new ListMap<string, string>(Q3Utils.split_config(src));

            Dictionary<string, string> replaces = new Dictionary<string, string>();
            replaces.Add("defrag_clfps", "com_maxfps");
            replaces.Add("defrag_svfps", "sv_fps");
            
            replaceKeys(split, replaces);
            return split.ToDictionary(); 
        }

        public static Dictionary<string, string> split_config_player(string src) {
            var split = new ListMap<string, string>(Q3Utils.split_config(src));
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

            replaceKeys(split, replaces);
            var nameIndex = Ext.IndexOf(split, x => x.Key.ToLowerInvariant() == "name");
            if (nameIndex >= 0) {
                var name = split[nameIndex].Value;
                string unColoredName = removeColors(name);
                if (!name.Equals(unColoredName)) {
                    split.Insert(nameIndex + 1, new KeyValuePair<string, string>("uncoloredName", unColoredName));
                }
            }

            Dictionary<string, string> res = split.ToDictionary();

            if (res.ContainsKey("team")) {
                int teamvalue;
                int.TryParse(res["team"], out teamvalue);
                if (teamvalue >= 0 && teamvalue < teamsFrendlyInfo.Length) {
                    res["team"] = teamsFrendlyInfo[teamvalue];
                }
            }
            return res;
        }

        static string[] teamsFrendlyInfo = new string[4] {
            "free","red","blue","spectators"
        };

        public static string getNameOnline(string demoTimeCmd)
        {
            //print \"Rom^7 reached the finish line in ^23:38:208^7\n\"
            demoTimeCmd = Regex.Replace(demoTimeCmd, "(\\^[0-9]|\\\"|\\n|\")", "");          //print Rom reached the finish line in 3:38:208
            string name = demoTimeCmd.Substring(6, demoTimeCmd.LastIndexOf(" reached") - 6); //Rom
            return DemoNames.normalizeName(name);
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

        public static string getNameOffline(string demoTimeCmd)
        {
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
            if (times.Count > 0 && times[0].Length != 3) {
                return TimeSpan.Zero;
            }
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

        //remove the color from the string
        public static string removeColors(string text)
        {
            return string.IsNullOrEmpty(text)
                ? text
                : Regex.Replace(text, "\\^.", "");
        }

        private KeyValuePair<int, ClientEvent>? getCorrectFinishEvent() {
            var correctFinishes = new ListMap<int, ClientEvent>();
            for (int i = clientEvents.Count - 1; i >= 0; i--) {
                int isCorrect = isEventCorrect(clientEvents, i);
                if (isCorrect > 0) {
                    correctFinishes.Add(isCorrect, clientEvents[i]);
                }
            }
            if (correctFinishes.Count > 0) {
                return Ext.MinOf(correctFinishes, x => x.Value.time);
            } else {
                return null;
            }
        }

        private static int isEventCorrect(List<ClientEvent> clientEvents, int index) {
            //0 - incorrect, 1 = correct start, 2 - correct tr
            if (!clientEvents[index].eventFinish) {
                return 0;
            }
            for (int i = index - 1; i >= 0; i--) {
                var prev = clientEvents[i];
                if (prev.eventChangePmType) {
                    return 0;
                }
                if (prev.eventTimeReset) {
                    return 2;
                }
                if (prev.eventStartTime) {
                    return 1;
                }
                //it is possible to start file in one frame with start timer, so check for start file is after
                //if change user and start timer was in one frame, so demo is normal
                if (prev.eventStartFile || prev.eventChangeUser) { 
                    return 0;
                }
            }
            return 0;
        }

        private void fillTimes(Dictionary<long, string> consoleCommands) {
            foreach (var kv in consoleCommands) {
                var value = kv.Value;
                if (value.StartsWith("print")) {

                    //print "Date: 10-25-14 02:43\n"
                    if (value.StartsWith("print \"Date:")) {
                        dateStamps.Add(value);
                    }

                    //print "Time performed by ^2uN-DeaD!Enter^7 : ^331:432^7 (v1.91.23 beta)\n"
                    if (value.StartsWith("print \"Time performed by")) {
                        allTimes.Add(TimeType.OFFLINE_NORMAL, value);   //defrag 1.9+
                    }

                    //"print \"^3Time Performed: 25:912 (defrag 1.5)\n^7\""
                    if (value.StartsWith("print \"^3Time Performed:")) {
                        allTimes.Add(TimeType.OFFLINE_OLD2, value);     //defrag 1.5
                    }

                    //print \"Rom^7 reached the finish line in ^23:38:208^7\n\"
                    if (value.Contains("reached the finish line in")) {
                        allTimes.Add(TimeType.ONLINE_NORMAL, value);
                    }

                } else if (value.StartsWith("NewTime")) {
                    //"NewTime -971299442 7:200 \"defrag 1.80\" \"Viper\" route ya->->rg"
                    allTimes.Add(TimeType.OFFLINE_OLD1, value);     //defrag 1.80
                } else if (value.StartsWith("newTime")) {
                    //newTime 47080
                    allTimes.Add(TimeType.OFFLINE_OLD3, value);     //defrag 1.42
                }
            }
        }

        private List<TimeStringInfo> getTimeStrings() {
            List<TimeStringInfo> infos = new List<TimeStringInfo>();
            if (allTimes.Count > 0) {
                for (int i = 0; i < allTimes.Count; i++) {
                    var demoTimeCmd = allTimes[i];
                    TimeSpan time = new TimeSpan();
                    string oName = "";  //online or offline name derived from a line in the console

                    switch (demoTimeCmd.Key) {
                        case TimeType.OFFLINE_NORMAL:
                            time = getTimeOfflineNormal(demoTimeCmd.Value);
                            oName = getNameOffline(demoTimeCmd.Value);
                            break;
                        case TimeType.ONLINE_NORMAL:
                            time = getTimeOnline(demoTimeCmd.Value);
                            oName = getNameOnline(demoTimeCmd.Value);
                            break;
                        case TimeType.OFFLINE_OLD1:
                            time = getTimeOld1(demoTimeCmd.Value);
                            oName = getNameOfflineOld1(demoTimeCmd.Value);
                            break;
                        case TimeType.OFFLINE_OLD2:
                            time = getTimeOfflineNormal(demoTimeCmd.Value);
                            break;
                        case TimeType.OFFLINE_OLD3:
                            time = getTimeOld3(demoTimeCmd.Value);
                            break;
                    }

                    var info = new TimeStringInfo();
                    info.time = time;
                    info.oName = oName;
                    info.timeString = demoTimeCmd.Value;
                    if (i < dateStamps.Count) {
                        info.recordDateString = dateStamps[i];
                        info.recordDate = getDateForDemo(dateStamps[i]);
                    }
                    infos.Add(info);
                }
            }
            return infos;
        }
    }
}
