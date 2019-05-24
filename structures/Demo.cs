using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DemoCleaner3.DemoParser.parser;
using System.Text.RegularExpressions;
using System.Globalization;
using DemoCleaner3.DemoParser.huffman;
using DemoCleaner3.DemoParser;
using DemoCleaner3.DemoParser.structures;
using DemoCleaner3.ExtClasses;
using DemoCleaner3.structures;

namespace DemoCleaner3
{
    //Class to maintain all information about demo file
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
        public bool hasCorrectName = false;
        public DateTime? recordTime;
        public bool hasTr = false;

        string dfType;
        string physic;
        string modNum;

        string validity = "";
        public bool useValidation = true;
        public bool rawTime = false;

        public RawInfo rawInfo = null;

        private string _demoNewName = "";

        //generating new demo name by all information
        public string demoNewName {
            get {
                if (!string.IsNullOrEmpty(_demoNewName)) {
                    return _demoNewName;
                }

                if (hasError) {
                    return file.Name;
                }

                string demoname = "";
                string playerCountry = country.Length > 0 ? playerName + "." + country : playerName;

                if (time.TotalMilliseconds > 0) {
                    //if we have time, write a normal name for the demo
                    string trString = hasTr ? "_tr" : "";
                    demoname = string.Format("{0}{1}[{2}]{3:D2}.{4:D2}.{5:D3}({6})",
                    mapName, trString, modphysic, (int) time.TotalMinutes, time.Seconds, time.Milliseconds, playerCountry);
                    hasCorrectName = true;
                } else {
                    hasCorrectName = false;
                    //if there is no time, then tormented with the generation of text
                    string oldName = file.Name;
                    oldName = oldName.Substring(0, oldName.Length - file.Extension.Length); //remove the extension
                    oldName = removeSubstr(oldName, mapName);                               //remove the map name
                    if (country.Length > 0) {
                        oldName = removeSubstr(oldName, playerCountry, false);
                    }
                    oldName = removeSubstr(oldName, playerName, false);                     //remove the player name
                    oldName = removeSubstr(oldName, country, false);                        //remove the country
                    oldName = removeSubstr(oldName, modphysic);                             //remove the mod with physics
                    oldName = removeSubstr(oldName, physic);                                //remove physics
                    oldName = removeSubstr(oldName, validity);                              //remove validation lines
                    oldName = removeDouble(oldName);                                        //remove double characters (except brackets)
                    oldName = oldName.Replace("[]", "").Replace("()", "");                  //remove the empty brackets
                    //oldName = Regex.Replace(oldName, "(^[^[a-zA-Z0-9]+|[^[a-zA-Z0-9]+$)", "");
                    oldName = Regex.Replace(oldName, "(^[^[a-zA-Z0-9\\(\\)\\]\\[]|[^[a-zA-Z0-9\\(\\)\\]\\[]$)", "");    //we remove crap at the beginning and at the end of name
                    oldName = oldName.Replace(" ", "_");                                    //убираем пробелы
                    
                    demoname = string.Format("{0}[{1}]{2}({3})", mapName, modphysic, oldName, playerCountry);

                    demoname = demoname.Replace(").)", ")").Replace(".)", ")");
                }

                if (useValidation && validity.Length > 0) {
                    demoname = demoname + "{" + validity + "}"; //add information about validation
                }

                _demoNewName = demoname + file.Extension;
                return _demoNewName;
            }
        }

        //the removing of the double non-alphanumeric characters and replace at first
        //for example: test__abc-_.xy -> test_abc-xy
        string removeDouble(string input)
        {
            var dup = Regex.Match(input, "[^[a-zA-Z0-9\\(\\)\\]\\[]{2,}");
            if (dup.Success && dup.Groups.Count > 0) {
                var symbol = dup.Groups[0].Value[0];
                return removeDouble(input.Substring(0, dup.Groups[0].Index) + symbol + input.Substring(dup.Groups[0].Index + dup.Groups[0].Length));
            } else {
                return input;
            }
        }

        //removing substring with adjacent characters for example: test_abc.xy -> test.xy
        //the last character is taken if there are symbols to left and to right 
        //and the first if only from left: test_abcxy -> test_xy
        string removeSubstr(string input, string include, bool fromstart = true)
        {
            if (include == null || include.Length == 0 || !input.Contains(include)) {
                return input;
            }
            var symbol = "";

            int cropstart = 0;
            int cropend = 0;
            int pos = fromstart ? input.IndexOf(include) : input.LastIndexOf(include);
            if (pos > 0) {
                symbol = input[pos - 1] + "";
                cropstart = char.IsLetterOrDigit(input[pos - 1]) ? 0 : 1;
            }
            if (pos + include.Length + 1 < input.Length) {
                cropend = char.IsLetterOrDigit(input[pos + include.Length]) ? 0 : 1;
                symbol = input[pos + include.Length] + "";
            }
            if (symbol == ")" || symbol == "]" || symbol == "}") {
                symbol = "_";
            }
            return input.Substring(0, pos - cropstart) + symbol + input.Substring(pos + include.Length + cropend);
        }

        //Get the details of the demo from the file name
        public static Demo GetDemoFromFile(FileInfo file)
        {
            Demo demo = new Demo();
            demo.file = file;
            demo.recordTime = demo.file.CreationTime;

            int index = Math.Max(file.Name.IndexOf(".cpm"), file.Name.IndexOf(".vq3"));
            if (index <= 0) {
                demo.hasError = true;
                return demo;
            }
            int firstSquareIndex = file.Name.Substring(0, index).LastIndexOf('[');
            if (firstSquareIndex <= 0) {
                demo.hasError = true;
                return demo;
            }
            string mapname = file.Name.Substring(0, firstSquareIndex);
            string others = file.Name.Substring(firstSquareIndex);

            var sub = others.Split("[]()".ToArray());
            if (sub.Length >= 4) {
                //Map
                demo.mapName = mapname;

                //Physic
                demo.modphysic = sub[1];
                if (demo.modphysic.Length < 3) {
                    demo.hasError = true;
                }

                //Time
                demo.timeString = sub[2];
                var times = demo.timeString.Split('-', '.');
                try {
                    demo.time = RawInfo.getTimeSpan(demo.timeString);
                } catch (Exception) {
                    demo.hasError = true;
                }

                //Name + country
                var countryName = sub[3];
                demo.country = tryGetCountryFromBrackets(countryName);
                demo.playerName = tryGetNameFromBrackets(countryName);
            } else {
                demo.hasError = true;
            }
            return demo;
        }

        //processing grouping files, if selected with processing mdf as df
        public static string mdfToDf(string mod, bool processIt)
        {
            if (processIt && mod.Length > 0 && mod[0] == 'm') {
                return mod.Substring(1);
            }
            return mod;
        }


        public static Demo GetDemoFromFileRaw(FileInfo file)
        {
            Q3HuffmanMapper.init();
            var raw = Q3DemoParser.getRawConfigStrings(file.FullName);
            return Demo.GetDemoFromRawInfo(raw);
        }

        //We get the filled demo from the full raw information pulled from the demo
        public static Demo GetDemoFromRawInfo(RawInfo raw)
        {
            var file = new FileInfo(raw.demoPath);

            var frConfig = raw.getFriendlyInfo();

            Demo demo = new Demo();

            demo.rawInfo = raw;

            //file
            demo.file = file;
            if (frConfig.Count == 0 || !frConfig.ContainsKey(RawInfo.keyClient)) {
                demo.hasError = true;
                return demo;
            }

            var filename = getNormalizedFileName(file);
            var countryAndName = getNameAndCountry(filename);
            string dfName = null;                                       //name in the game in the demo names
            string uName = null;                                        //name in the game
            string demoUserName = tryGetNameFromBrackets(countryAndName);  //name from the filename

            //names
            var names = new DemoNames(Ext.GetOrNull(frConfig, RawInfo.keyPlayer), demoUserName);

            //time from triggers
            if (raw.fin.HasValue) {
                demo.time = TimeSpan.FromMilliseconds(raw.fin.Value.Value.time);
                demo.hasTr = raw.fin.Value.Key > 1;
            }

            var timestrings = raw.timeStrings;
            if (demo.time.TotalMilliseconds > 0) {
                var date = timestrings.LastOrDefault(x => x.recordDate != null);
                demo.recordTime = date?.recordDate;
            } else {
                //time from commands
                TimeStringInfo fastestTimeString = null;
                if (timestrings.Count == 1) {
                    fastestTimeString = timestrings.First();
                } else if (timestrings.Count >= 1) {
                    var cuStrings = timestrings.Where(x => (!string.IsNullOrEmpty(x.oName) && (x.oName == names.dfName || x.oName == names.uName)));
                    if (cuStrings.Count() > 0) {
                        fastestTimeString = Ext.MinOf(cuStrings, x => (long) x.time.TotalMilliseconds);
                    }
                }
                if (fastestTimeString != null) {
                    demo.time = fastestTimeString.time;
                    demo.recordTime = fastestTimeString.recordDate;
                    if (fastestTimeString.oName.Length > 0) {
                        var user = raw.getPlayerInfoByPlayerName(fastestTimeString.oName);
                        if (user != null) {
                            names = new DemoNames(user, demoUserName);
                        } else {
                            names = new DemoNames(fastestTimeString.oName, demoUserName);
                        }
                    }
                }
            }
            demo.playerName = names.normalName;

            //at least some time (from name of demo)
            if (demo.time.TotalMilliseconds > 0) {
                demo.rawTime = true;
            } else {
                var demoNameTime = tryGetTimeFromFileName(filename);
                if (demoNameTime != null) {
                    demo.time = demoNameTime.Value;
                }
            }

            //Map
            var mapInfo = raw.rawConfig.ContainsKey(Q3Const.Q3_DEMO_CFG_FIELD_MAP) ? raw.rawConfig[Q3Const.Q3_DEMO_CFG_FIELD_MAP] : "";
            var mapName = Ext.GetOrNull(frConfig[RawInfo.keyClient], "mapname");

            //If in mapInfo the name of the same map is written, then we take the name from there
            if (mapName.ToLowerInvariant().Equals(mapInfo.ToLowerInvariant())) {
                demo.mapName = mapInfo;
            } else {
                demo.mapName = mapName.ToLowerInvariant();
            }

            //Gametype
            var gameType = Ext.GetOrNull(frConfig[RawInfo.keyClient], "defrag_gametype");
            int gType = 0;
            if (!string.IsNullOrEmpty(gameType)) {
                int.TryParse(gameType, out gType);
                switch (gType) {
                    case 1: demo.dfType = "df"; break;
                    case 2: demo.dfType = "fs"; break;
                    case 3: demo.dfType = "fc"; break;

                    case 5: demo.dfType = "mdf"; break;
                    case 6: demo.dfType = "mfs"; break;
                    case 7: demo.dfType = "mfc"; break;
                }
            }

            //Promode
            var promode = Ext.GetOrNull(frConfig[RawInfo.keyClient], "df_promode");
            if (!string.IsNullOrEmpty(promode)) {
                int phMode = 0;
                int.TryParse(promode, out phMode);
                demo.physic = phMode == 1 ? "cpm" : "vq3";                  //vq3, cpm
            }

            if (gType == 0) {
                var gName = Ext.GetOrNull(frConfig[RawInfo.keyClient], "gamename");
                var fsGName = frConfig.ContainsKey(RawInfo.keyGame) ? Ext.GetOrNull(frConfig[RawInfo.keyGame], "fs_game") : "";
                var fsdf = fsGName?.ToLowerInvariant()?.StartsWith("defrag");
                if (gName?.ToLowerInvariant() == "defrag" || (fsdf.HasValue && fsdf.Value) || demo.rawTime == true) {
                    demo.dfType = "df";

                    //in older protocols may not be information about physic, then there vq3
                    if (string.IsNullOrEmpty(demo.physic)) {
                        demo.physic = "vq3";
                    }
                } else if (gName?.ToLowerInvariant() == "osp") {
                    demo.dfType = "osp";
                } else { 
                    demo.dfType = "dm";
                }
            }

            //Mode for fastcaps and freestyle
            var dfMode = Ext.GetOrNull(frConfig[RawInfo.keyClient], "defrag_mode");
            if (!string.IsNullOrEmpty(dfMode)) {
                int defragMode = 0;
                int.TryParse(dfMode, out defragMode);                                                     //>=0 = mode
                demo.modNum = (gType != 1 && gType != 5) ? string.Format(".{0}", defragMode) : null;      //.0 - .7
            }

            //Joining
            if (demo.physic != null) {
                demo.modphysic = string.Format("{0}.{1}{2}", demo.dfType, demo.physic, demo.modNum);
            } else {
                demo.modphysic = demo.dfType;
            }

            int protocol = 0;
            var protocolString = Ext.GetOrNull(frConfig[RawInfo.keyClient], "protocol");
            if (!string.IsNullOrEmpty(protocolString)) {
                int.TryParse(protocolString, out protocol);
            }

            //If demo has cheats, write it
            demo.validity = checkValidity(frConfig, demo.time.TotalMilliseconds > 0, demo.rawTime, demo.dfType);

            //demo has not info about country, so take it from filename
            demo.country = tryGetCountryFromBrackets(countryAndName);
            return demo;
        }

        //We are trying to get the name and country from the demo (the first occurrence of two round brackets)
        static string getNameAndCountry(string filename) {
            var brackets = Regex.Matches(filename, "\\([^)]*\\)");
            if (brackets.Count > 0) {
                for (int i = 0;i< brackets.Count;i++) {
                    var value = brackets[i].Value;
                    if (value.Contains('.')){
                        return value.Replace("(", "").Replace(")", "");
                    }
                }
                return brackets[0].Value.Replace("(", "").Replace(")", "");
            }
            return "";
        }

        //We are trying to get a name from the name and country
        static string tryGetNameFromBrackets(string partname)
        {
            int i = partname.LastIndexOf('.');
            if (i > 0) {
                partname = partname.Substring(0, i);
            }
            return partname;
        }

        //We are trying to get the country, and only it, from the name and country
        static string tryGetCountryFromBrackets(string partname)
        {
            int i = partname.LastIndexOf('.');
            if (i > 0 && i +1 < partname.Length) {
                var country = partname.Substring(i+1, partname.Length - i - 1);
                if (country.Where(c => char.IsNumber(c)).Count() == 0) {
                    return country;
                }
            }
            return "";
        }

        //Trying to get time from the demo filename
        static TimeSpan? tryGetTimeFromFileName(string filename)
        {
            var sub = filename.Split("[]()_".ToArray());
            foreach (string part in sub) {
                var time = tryGetTimeFromBrackets(part);
                if (time != null) {
                    return time;
                }
            }
            return null;
        }

        //Trying to get time from part of demo filename.
        static TimeSpan? tryGetTimeFromBrackets(string partname)
        {
            var parts = partname.Split("-".ToArray());
            if (parts.Length < 2 || parts.Length > 3) {
                parts = partname.Split(".".ToArray());
                if (parts.Length < 2 || parts.Length > 3) {
                    return null;
                }
            }
            foreach (string part in parts) {
                if (part.Length == 0) {
                    return null;
                }
                foreach (char c in part) {
                    if (!char.IsDigit(c)) {
                        return null;
                    }
                }
            }
            return RawInfo.getTimeSpan(partname);
        }

        //Normalization of demo filenames in case they broken by discord or net
        static string getNormalizedFileName(FileInfo file) {
            string filename = file.Name;
            //rm_n2%5Bmdf.vq3%5D00.33.984%28h%40des.CountryHere%29.dm_68
            if (filename.Contains("%")) {
                filename = Uri.UnescapeDataString(filename);
            }
            //r7-falkydf.cpm00.09.960xas.China.dm_68
            if (!filename.Contains("[") && !filename.Contains("]") && !filename.Contains("(") && !filename.Contains(")")) {
                int index = Math.Max(filename.IndexOf(".cpm"), filename.IndexOf(".vq3"));
                if (index > 0) {
                    try {
                        int i1 = filename[index - 3] == 'm' ? index - 3 : index - 2;
                        int i2 = filename[index + 4] == '.' ? index + 6 : index + 4;
                        int i3 = i2 + 9;
                        int i4 = file.Name.Length - file.Extension.Length;
                        int i5 = file.Name.Length;
                        string mapname = filename.Substring(0, i1);
                        string physic = filename.Substring(i1, i2 - i1);
                        string time = filename.Substring(i2, i3 - i2);
                        string name = filename.Substring(i3, i4 - i3);
                        string ext = filename.Substring(i4, i5 - i4);
                        if (isDigits(time[0], time[1], time[3], time[4], time[7], time[8])) {
                            filename = string.Format("{0}[{1}]{2}({3}){4}", mapname, physic, time, name, ext);
                        }
                    } catch (Exception ex) {
                    }
                }
            }
            //dfcomp009_3.792_VipeR_Russia.dm_68
            //dmp02a_jinx_13.880_t0t3r_germany.dm_68
            if (Ext.CountOf(filename, '_') >= 4 && !filename.Contains("(") && !filename.Contains(")")) {
                var array = filename.Substring(0, filename.Length - file.Extension.Length).Split('_');
                TimeSpan? time = null;
                for (int i = 1; i < 4; i++) {
                    if (time == null) {
                        time = tryGetTimeFromBrackets(array[i]);
                    }
                }
                if (time != null && time.Value.TotalMilliseconds > 0) {
                    var fName = string.Join("_", array.Take(array.Length - 2).ToArray());
                    filename = string.Format("{0}({1}.{2}){3}", fName, array[array.Length - 2], array[array.Length - 1], file.Extension);
                }
            }

            return filename;
        }

        static bool isDigits(params char[] keys) {
            foreach (char c in keys) {
                if (!char.IsDigit(c)) {
                    return false;
                }
            }
            return true;
        }

        //check demo for validity
        static string checkValidity(Dictionary<string, Dictionary<string, string>> frConfig, bool hasTime, bool hasRawTime, string dfType) {
            if (!frConfig.ContainsKey(RawInfo.keyGame)) {
                return "";
            }

            var kGame = frConfig[RawInfo.keyGame];

            var defrag_gametype = Ext.GetOrNull(frConfig[RawInfo.keyClient], "defrag_gametype");
            int gametype = 0;
            if (!string.IsNullOrEmpty(defrag_gametype)) {
                int.TryParse(defrag_gametype, out gametype);
            }

            var online = gametype > 3;
            string res;

            var fs = (gametype == 2 || gametype == 6 || dfType == "dm");
            if (!fs) {
                res = checkKey(kGame, "sv_cheats", 0); if (res.Length > 0) return res;
            }

            res = checkKey(kGame, "timescale", 1);                  if (res.Length > 0) return res;
            res = checkKey(kGame, "g_speed", 320);                  if (res.Length > 0) return res;
            res = checkKey(kGame, "g_gravity", 800);                if (res.Length > 0) return res;
            res = checkKey(kGame, "g_knockback", 1000);             if (res.Length > 0) return res;

            if (hasTime && !hasRawTime) {
                //If the demo was not found messages about the finish map
                return "client_finish=false";
            }

            if (online && !fs) {
                res = checkKey(kGame, "df_mp_interferenceoff", 3); if (res.Length > 0) return res;
            }

            res = checkKey(kGame, "pmove_msec", 8);                 if (res.Length > 0) return res;
            res = checkKey(kGame, "defrag_svfps", 125, "sv_fps");   if (res.Length > 0) return res;

            res = checkKey(kGame, "pmove_fixed", (online ? 1 : 0)); if (res.Length > 0) return res;
            res = checkKey(kGame, "g_synchronousclients", (online ? 0 : 1), "g_sync"); if (res.Length > 0) return res;
            return "";
        }

        //checking the key for validity
        static string checkKey(Dictionary<string, string> keysGame, string key, int val, string errorString = "") {
            if (keysGame.ContainsKey(key) && keysGame[key].Length > 0) {
                var keyValue = keysGame[key];
                float value = -1;
                try {
                    value = float.Parse(keyValue, CultureInfo.InvariantCulture);
                } catch (Exception ex) {
                }
                if (value < 0 || value != (float) val) {
                    if (keyValue.StartsWith(".")) {     //edit the timescale display as .3 -> 0.3
                        keyValue = "0" + keyValue;
                    }
                    return (errorString.Length > 0 ? errorString : key) + "=" + keyValue;
                }
            }
            return "";
        }
    }
}
