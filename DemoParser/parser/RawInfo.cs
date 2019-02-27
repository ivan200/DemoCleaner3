using DemoCleaner2.DemoParser.utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace DemoCleaner2.DemoParser.parser
{
    public class RawInfo
    {
        public static string keyPlayer = "player";
        public static string keyClient = "client";
        public static string keyGame = "game";
        public static string keyRecord = "record";
        public static string keyRecordTime = "time";
        public static string keyRecordDate = "date";
        public static string keyRaw = "raw";

        public Dictionary<short, string> rawConfig;
        public List<string> performedTimes;
        public List<string> dateStamps;

        Dictionary<string, Dictionary<string, string>> friendlyInfo;

        public RawInfo(Dictionary<short, string> rawConfig, List<string> dateStamps, List<string> performedTimes)
        {
            this.rawConfig = rawConfig;
            this.performedTimes = performedTimes;
            this.dateStamps = dateStamps;
        }

        public Dictionary<string, Dictionary<string, string>> getFriendlyInfo() {
            if (friendlyInfo == null) {
                friendlyInfo = new Dictionary<string, Dictionary<string, string>>();

                Dictionary<string, string> times = new Dictionary<string, string>();

                if (dateStamps.Count > 0 || performedTimes.Count > 0)
                {
                    for (int i = 0; i < dateStamps.Count; i++)
                    {
                        string key = dateStamps.Count > 1 ? keyRecordDate + " " + i + 1 : keyRecordDate;
                        times.Add(key, dateStamps[i]);
                    }
                    for (int i = 0; i < performedTimes.Count; i++)
                    {
                        string key = performedTimes.Count > 1 ? keyRecordTime + " " + i + 1 : keyRecordTime;
                        times.Add(key, performedTimes[i]);
                    }
                    friendlyInfo.Add(keyRecord, times);
                }


                if (rawConfig.ContainsKey(Q3Const.Q3_DEMO_CFG_FIELD_PLAYER))
                {
                    friendlyInfo.Add(keyPlayer, Q3Utils.split_config(rawConfig[Q3Const.Q3_DEMO_CFG_FIELD_PLAYER]));
                }
                if (rawConfig.ContainsKey(Q3Const.Q3_DEMO_CFG_FIELD_CLIENT))
                {
                    friendlyInfo.Add(keyClient, Q3Utils.split_config(rawConfig[Q3Const.Q3_DEMO_CFG_FIELD_CLIENT]));
                }
                if (rawConfig.ContainsKey(Q3Const.Q3_DEMO_CFG_FIELD_GAME))
                {
                    friendlyInfo.Add(keyGame, Q3Utils.split_config(rawConfig[Q3Const.Q3_DEMO_CFG_FIELD_GAME]));
                }

                Dictionary<string, string> raw = new Dictionary<string, string>();
                foreach (var r in rawConfig)
                {
                    raw.Add(r.Key.ToString(), r.Value);
                }
                friendlyInfo.Add(keyRaw, raw);
            }
            return friendlyInfo;
        }
    }
}
