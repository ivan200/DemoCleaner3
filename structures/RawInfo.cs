using DemoCleaner3.DemoParser.structures;
using DemoCleaner3.DemoParser.utils;
using DemoCleaner3.ExtClasses;
using DemoCleaner3.structures;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace DemoCleaner3.DemoParser.parser {
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
        public static string keyOtherPlayers = "other players";
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
        public enum FinishType {
            INCORRECT,
            CORRECT_START,
            CORRECT_TR
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

        public KeyValuePair<FinishType, ClientEvent>? fin = null;

        public Dictionary<string, string> kPlayer = null;
        public GameInfo gameInfo = null;
        public int maxSpeed = 0;
        public bool isLongStart = false;
        public bool isSpectator = false;
        public List<long> cpData = new List<long>();

        public RawInfo(string demoName, ClientConnection clientConnection, List<ClientEvent> clientEvents, int maxSpeed) {
            this.demoPath = demoName;
            this.clc = clientConnection;
            this.rawConfig = clientConnection.configs;
            this.clientEvents = clientEvents;
            this.fin = getCorrectFinishEvent();
            this.maxSpeed = maxSpeed;

            fillTimes(clientConnection.console);
            timeStrings = getTimeStrings();

            cpData = fillCpData(clientEvents, fin);
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
            long playerNum = -1;
            if (fin.HasValue) {
                playerNum = fin.Value.Value.playerNum;
                kPlayer = getPlayerInfoByPlayerNum(playerNum);
            }
            if (kPlayer == null && clientEvents.Count > 0) {
                //if spectator view player and there was no finish
                var lastEvent = clientEvents.LastOrDefault();
                if (lastEvent != null) {
                    playerNum = lastEvent.playerNum;
                    kPlayer = getPlayerInfoByPlayerNum(playerNum);
                }
            }
            if (kPlayer == null) {
                playerNum = clc.clientNum;
                kPlayer = getPlayerInfoByPlayerNum(playerNum);
            }
            var keys = allPlayersConfigs.Keys.ToList();
            if (kPlayer == null && keys.Count == 1) {
                playerNum = keys[0];
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
            if (fin != null && fin.HasValue) {
                string bestTime = getTimeByMillis(fin.Value.Value.timeNoError);
                bool hasTr = fin.Value.Key == FinishType.CORRECT_TR;
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
                string startFileUserName = null;
                string startTimerUserName = null;
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
                            startFileUserName = user == null ? null : Ext.GetOrNull(user, "name");
                            string userString = string.IsNullOrEmpty(startFileUserName) ? "" : "Client: " + startFileUserName;
                            triggers.Add("StartFile", userString);
                            startFileServerTime = ce.serverTime;
                        }
                        if (ce.eventStartTime) {
                            var user = getPlayerInfoByPlayerNum(ce.playerNum);
                            startTimerUserName = user == null ? null : Ext.GetOrNull(user, "name");
                            string userString = string.IsNullOrEmpty(startTimerUserName) ? "" : "Player: " + startTimerUserName;
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
                    Q3Utils.PrintDebug(clc.errors, ex);
                }

                //detection demos recorded by spectator
                if (!string.IsNullOrEmpty(startFileUserName) 
                    && !string.IsNullOrEmpty(startTimerUserName) 
                    && startFileUserName != startTimerUserName) {
                    friendlyInfo[keyRecord].Add("spectatorRecorded", "true");
                    isSpectator = true;
                }

                //detection demos with very long start
                var timeMillis = startTimerServerTime - startFileServerTime;
                if (timeMillis > 0) {
                    var time = TimeSpan.FromMilliseconds(timeMillis);
                    if (time.TotalSeconds > 20) {
                        isLongStart = true;

                        var sTime = TimeSpan.FromMilliseconds(startTimerServerTime);
                        var sTimeString = string.Format("{0:D2}:{1:D2}:{2:D3}", (int)sTime.TotalMinutes, sTime.Seconds, sTime.Milliseconds);
                        friendlyInfo[keyRecord].Add("lateStart", $"{(int)time.TotalSeconds} sec (servertime: {sTimeString})");
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

            //Other Players
            if (allPlayersConfigs.Count > 1 && playerNum >= 0) {
                var playerKey = playerNum + Q3Const.Q3_DEMO_CFG_FIELD_PLAYER;
                var pls = new Dictionary<string, string>();
                foreach (var player in allPlayersConfigs) {
                    if (player.Key != playerKey) {
                        var somePlayer = player.Value;
                        var tt = new string[] { "name", "df_name", "nick" }.Select(x => Ext.GetOrNull(somePlayer, x)).ToArray();
                        var name = DemoNames.chooseName(tt);
                        pls.Add(player.Key.ToString(), name);
                    }
                }
                friendlyInfo.Add(keyOtherPlayers, pls);
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
                    var asciiText = ConsoleStringUtils.removeNonAscii(kv.Value.ToString());
                    conTexts.Add(kv.Key.ToString(), asciiText);
                }
                friendlyInfo.Add(keyConsole, conTexts);
            }

            //Errors
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


        public static Dictionary<string, string> split_config_game(string src) {
            var split = new ListMap<string, string>(Q3Utils.split_config(src));

            Dictionary<string, string> replaces = new Dictionary<string, string>();
            replaces.Add("defrag_clfps", "com_maxfps");
            replaces.Add("defrag_svfps", "sv_fps");
            
            Ext.replaceKeys(split, replaces);
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

            Ext.replaceKeys(split, replaces);
            var nameIndex = Ext.IndexOf(split, x => x.Key.ToLowerInvariant() == "name");
            if (nameIndex >= 0) {
                var name = split[nameIndex].Value;
                string unColoredName = ConsoleStringUtils.removeColors(name);
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

        private KeyValuePair<FinishType, ClientEvent>? getCorrectFinishEvent() {
            var correctFinishes = new ListMap<FinishType, ClientEvent>();
            for (int i = clientEvents.Count - 1; i >= 0; i--) {
                FinishType finishType = isFinishCorrect(clientEvents, i);
                if (finishType != FinishType.INCORRECT && clientEvents[i].timeNoError > 0) {
                    correctFinishes.Add(finishType, clientEvents[i]);
                }
            }
            if (correctFinishes.Count > 0) {
                return Ext.MinOf(correctFinishes, x => x.Value.timeNoError);
            } else {
                return null;
            }
        }

        private static FinishType isFinishCorrect(List<ClientEvent> clientEvents, int index) {
            if (!clientEvents[index].eventFinish) {
                return FinishType.INCORRECT; 
            }
            for (int i = index - 1; i >= 0; i--) {
                var prev = clientEvents[i];
                if (prev.eventChangePmType || prev.eventFinish) {
                    return FinishType.INCORRECT;
                }
                clientEvents[index].timeByServerTime = clientEvents[index].serverTime - prev.serverTime;
                if (prev.eventTimeReset) {
                    return FinishType.CORRECT_TR;
                }
                if (prev.eventStartTime) {
                    return FinishType.CORRECT_START;
                }
                //it is possible to start file in one frame with start timer, so check for start file is after
                //if change user and start timer was in one frame, so demo is normal
                if (prev.eventStartFile || prev.eventChangeUser) { 
                    return FinishType.INCORRECT;
                }
            }
            return FinishType.INCORRECT;
        }

        /// <summary> Filling checkpoints data and return list of millseconds between every checkpoint in run</summary>
        private List<long> fillCpData(List<ClientEvent> allEvents, KeyValuePair<FinishType, ClientEvent>? fin) {
            var cps = new List<long>();
            if (fin == null || !fin.HasValue || fin.Value.Key == FinishType.INCORRECT) {
                return cps;
            }
            
            var started = false;
            ClientEvent currentTrigger = null;

            for (int i = allEvents.Count - 1; i >= 0; i--) {
                var ev = allEvents[i];
                if (!started) {
                    if (ev.serverTime == fin.Value.Value.serverTime) {
                        started = true;
                        currentTrigger = ev;
                    }
                } else {
                    if (ev.eventCheckPoint || ev.eventStartTime || ev.eventTimeReset) {
                        long cp = currentTrigger.serverTime - ev.serverTime;
                        cps.Add(cp);
                        currentTrigger = ev;
                    }
                    if (ev.eventStartTime || ev.eventTimeReset) {
                        cps.Reverse();
                        return cps;
                    }
                }
            }
            return new List<long>();
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
                    else if (value.StartsWith("print \"Time performed by")) {
                        allTimes.Add(TimeType.OFFLINE_NORMAL, value);   //defrag 1.9+
                    }

                    //"print \"^3Time Performed: 25:912 (defrag 1.5)\n^7\""
                    else if (value.StartsWith("print \"^3Time Performed:")) {
                        allTimes.Add(TimeType.OFFLINE_OLD2, value);     //defrag 1.5
                    }

                    //print \"Rom^7 reached the finish line in ^23:38:208^7\n\"
                    else if (value.Contains("reached the finish line in")) {
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
                            time = ConsoleStringUtils.getTimeOfflineNormal(demoTimeCmd.Value);
                            oName = ConsoleStringUtils.getNameOffline(demoTimeCmd.Value);
                            break;
                        case TimeType.ONLINE_NORMAL:
                            time = ConsoleStringUtils.getTimeOnline(demoTimeCmd.Value);
                            oName = ConsoleStringUtils.getNameOnline(demoTimeCmd.Value);
                            break;
                        case TimeType.OFFLINE_OLD1:
                            time = ConsoleStringUtils.getTimeOld1(demoTimeCmd.Value);
                            oName = ConsoleStringUtils.getNameOfflineOld1(demoTimeCmd.Value);
                            break;
                        case TimeType.OFFLINE_OLD2:
                            time = ConsoleStringUtils.getTimeOfflineNormal(demoTimeCmd.Value);
                            break;
                        case TimeType.OFFLINE_OLD3:
                            time = ConsoleStringUtils.getTimeOld3(demoTimeCmd.Value);
                            break;
                    }

                    var info = new TimeStringInfo();
                    info.time = time;
                    info.oName = oName;
                    info.timeString = demoTimeCmd.Value;
                    if (i < dateStamps.Count) {
                        info.recordDateString = dateStamps[i];
                        info.recordDate = ConsoleStringUtils.getDateForDemo(dateStamps[i]);
                    }
                    infos.Add(info);
                }
            }
            return infos;
        }
    }
}
