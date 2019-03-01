using DemoCleaner2.DemoParser.utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DemoCleaner2.DemoParser.parser
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
        public List<string> dateStamps;
        public string demoPath;

        Dictionary<string, Dictionary<string, string>> friendlyInfo;

        public RawInfo(
            string demoName,
            Dictionary<short, string> rawConfig,
            List<string> dateStamps,
            List<string> performedTimes,
            List<string> onlineTimes)
        {
            this.demoPath = demoName;
            this.rawConfig = rawConfig;
            this.performedTimes = performedTimes;
            this.dateStamps = dateStamps;
            this.onlineTimes = onlineTimes;
        }

        public Dictionary<string, Dictionary<string, string>> getFriendlyInfo()
        {
            if (friendlyInfo == null) {
                friendlyInfo = new Dictionary<string, Dictionary<string, string>>();

                Dictionary<string, string> times = new Dictionary<string, string>();
                times.Add(keyDemoName, new FileInfo(demoPath).Name);

                if (dateStamps.Count > 0 || performedTimes.Count > 0 || onlineTimes.Count > 0) {
                    for (int i = 0; i < dateStamps.Count; i++) {
                        string key = dateStamps.Count > 1 ? keyRecordDate + " " + (i + 1) : keyRecordDate;
                        times.Add(key, dateStamps[i].Replace("print ", ""));
                    }
                    for (int i = 0; i < performedTimes.Count; i++) {
                        string key = performedTimes.Count > 1 ? keyRecordTime + " " + (i + 1) : keyRecordTime;
                        times.Add(key, performedTimes[i].Replace("print ",""));
                    }
                    for (int i = 0; i < onlineTimes.Count; i++) {
                        string key = onlineTimes.Count > 1 ? keyRecordTime + " " + (i + 1) : keyRecordTime;
                        times.Add(key, onlineTimes[i].Replace("print ", ""));
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
            }
            return friendlyInfo;
        }
    }
}
