using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DemoCleaner3.DemoParser.parser;
using System.Text.RegularExpressions;
using DemoCleaner3.DemoParser.huffman;
using DemoCleaner3.DemoParser;
using DemoCleaner3.structures;
using System.Globalization;

namespace DemoCleaner3 {
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
        public bool isBroken;
        public bool hasError;
        public bool hasCorrectName = false;
        public DateTime? recordTime;
        public bool hasTr = false;

        public Dictionary<string, string> validDict = new Dictionary<string, string>();
        public string validity {
            get {
                if (validDict.Count > 0) {
                    var first = validDict.First();
                    return string.Format("{0}={1}", first.Key, first.Value);
                } else return "";
            }
        }

        public bool useValidation = true;
        public bool rawTime = false;
        public bool triggerTime = false;

        public RawInfo rawInfo = null;
        private string _demoNewName = "";

        private string _normalizedFileName = "";
        private string normalizedFileName {
            get {
                if (string.IsNullOrEmpty(_normalizedFileName)) {
                    _normalizedFileName = getNormalizedFileName(file);
                }
                return _normalizedFileName;
            }
        }

        private long userId = 0;

        //generating new demo name by all information
        public string demoNewName {
            get {
                if (!string.IsNullOrEmpty(_demoNewName)) {
                    return _demoNewName;
                }

                if (hasError) {
                    return normalizedFileName;
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
                    string oldName = normalizedFileName;
                    oldName = oldName.Substring(0, oldName.Length - file.Extension.Length); //remove the extension
                    oldName = removeSubstr(oldName, mapName);                               //remove the map name
                    if (country.Length > 0) {
                        oldName = removeSubstr(oldName, playerCountry, false);
                    }
                    oldName = oldName.Replace("[dm]", "");  //replace previous wrong mod detection

                    var normalizedName = DemoNames.normalizeName(playerName);
                    oldName = removeSubstr(oldName, normalizedName, false);                 //remove the player name
                    oldName = removeSubstr(oldName, country, false);                        //remove the country
                    oldName = removeSubstr(oldName, modphysic);                             //remove the mod with physics
                    if (rawInfo != null && rawInfo.gameInfo != null) {
                        oldName = removeSubstr(oldName, rawInfo.gameInfo.gameNameShort);    //remove the mod
                    }
                    //oldName = removeSubstr(oldName, physic);                              //remove physics
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
                if (userId > 0) {
                    demoname = demoname + "[" + userId.ToString() + "]"; //add userId (can be added only with triggertime)
                }
                _demoNewName = demoname + file.Extension.ToLowerInvariant();
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
                var s = input[pos - 1];
                cropstart = char.IsLetterOrDigit(s) ? 0 : 1;
                if (cropstart > 0) {
                    symbol = s.ToString();
                }
            }
            if (pos + include.Length + 1 < input.Length) {
                var s = input[pos + include.Length];
                cropend = char.IsLetterOrDigit(s) ? 0 : 1;
                if (cropend > 0) {
                    symbol = s.ToString();
                }
            }
            if ((cropstart > 0 && (symbol == "(" || symbol == "[" || symbol == "{"))
                || (cropend > 0 && (symbol == ")" || symbol == "]" || symbol == "}"))) {
                symbol = "_";
            }
            return input.Substring(0, pos - cropstart) + symbol + input.Substring(pos + include.Length + cropend);
        }

        //Get the details of the demo from the file name
        public static Demo GetDemoFromFile(FileInfo file)
        {
            Demo demo = new Demo();
            demo.file = file;
            var filename = file.Name;

            demo.recordTime = demo.file.CreationTime;

            int index = Math.Max(filename.IndexOf(".cpm"), filename.IndexOf(".vq3"));
            if (index <= 0) {
                demo.hasError = true;
                return demo;
            }
            int firstSquareIndex = filename.Substring(0, index).LastIndexOf('[');
            if (firstSquareIndex <= 0) {
                demo.hasError = true;
                return demo;
            }
            string mapname = filename.Substring(0, firstSquareIndex);
            string others = filename.Substring(firstSquareIndex);

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
                if (demo.time.TotalMilliseconds <= 0) {
                    demo.hasError = true;
                }

                //Name + country
                var countryName = sub[3];
                var countryNameParsed = tryGetNameAndCountry(countryName, null);
                demo.playerName = countryNameParsed.Key;
                demo.country = countryNameParsed.Value;

                var c1 = filename.LastIndexOf(')');
                var b1 = filename.LastIndexOf('{');
                var b2 = filename.LastIndexOf('}');
                if (b2 > b1 && b1 > c1 && c1 > 0) {
                    var vstr = filename.Substring(b1 + 1, b2 - b1 - 1);
                    var v = vstr.Split('=');
                    if (v.Length > 1) {
                        demo.validDict = new Dictionary<string, string>();
                        demo.validDict.Add(v[0], v[1]);
                    }
                }
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
                demo.isBroken = true;
                return demo;
            }

            //config names
            var names = new DemoNames();
            names.setNamesByPlayerInfo(Ext.GetOrNull(frConfig, RawInfo.keyPlayer));

            //time from triggers
            if (raw.fin.HasValue) {
                demo.time = TimeSpan.FromMilliseconds(raw.fin.Value.Value.time);
                demo.hasTr = raw.fin.Value.Key > 1;
                demo.triggerTime = true;
            }

            var timestrings = raw.timeStrings;

            var fastestTimeString = getFastestTimeStringInfo(timestrings, names);

            if (demo.time.TotalMilliseconds > 0) {
                var date = timestrings.LastOrDefault(x => x.recordDate != null);
                demo.recordTime = date?.recordDate;
            } else {
                //time from commands
                if (fastestTimeString != null) {
                    demo.time = fastestTimeString.time;
                    demo.recordTime = fastestTimeString.recordDate;

                    var user = raw.getPlayerInfoByPlayerName(fastestTimeString.oName);
                    if (user != null) {
                        names.setNamesByPlayerInfo(user);
                    }
                }
            }
            if (fastestTimeString != null) {
                names.setOnlineName(fastestTimeString.oName);
            }

            var filename = demo.normalizedFileName;
            var countryAndName = getNameAndCountry(filename);
            var countryNameParsed = tryGetNameAndCountry(countryAndName, names);

            //fle name
            names.setBracketsName(countryNameParsed.Key);   //name from the filename

            demo.playerName = names.chooseNormalName();

            //demo has not info about country, so take it from filename
            demo.country = countryNameParsed.Value;

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
            var gInfo = raw.gameInfo;
            if (gInfo.isDefrag) {
                if (!string.IsNullOrEmpty(gInfo.modType)) {
                    demo.modphysic = string.Format("{0}.{1}.{2}", gInfo.gameTypeShort, gInfo.gameplayTypeShort, gInfo.modType);
                } else {
                    demo.modphysic = string.Format("{0}.{1}", gInfo.gameTypeShort, gInfo.gameplayTypeShort);
                }
            } else {
                demo.modphysic = string.Format("{0}.{1}", gInfo.gameNameShort, gInfo.gameTypeShort);
            }

            //If demo has cheats, write it
            demo.validDict = checkValidity(demo.time.TotalMilliseconds > 0, demo.rawTime, gInfo);

            if (demo.triggerTime) {
                demo.userId = tryGetUserIdFromFileName(file);
            }
            return demo;
        }

        static TimeStringInfo getFastestTimeStringInfo(List<TimeStringInfo> timestrings, DemoNames names) {
            TimeStringInfo fastestTimeString = null;
            if (timestrings.Count == 1) {
                fastestTimeString = timestrings.First();
            } else if (timestrings.Count >= 1) {
                var cuStrings = timestrings.Where(x => (!string.IsNullOrEmpty(x.oName) && (x.oName == names.dfName || x.oName == names.uName)));
                if (cuStrings.Count() == 0) {
                    var groups = timestrings.GroupBy(x => x.oName);
                    if (groups.Count() == 1) {
                        cuStrings = timestrings;
                    }
                }
                if (cuStrings.Count() > 0) {
                    fastestTimeString = Ext.MinOf(cuStrings, x => (long)x.time.TotalMilliseconds);
                }
            }
            return fastestTimeString;
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

        //We are trying to split name and country
        static Pair tryGetNameAndCountry(string partname, DemoNames names) {
            if (names != null && (partname == names.dfName || partname == names.uName || partname == names.oName)) {
                //name can contains dots so if username from parameters equals part in brackets, no country here
                return new Pair(partname, "");
            }
            int i = partname.LastIndexOf('.');
            if (i > 0 && i + 1 < partname.Length) {
                var country = partname.Substring(i + 1, partname.Length - i - 1);
                if (country.Where(c => char.IsNumber(c)).Count() == 0) {
                    return new Pair(partname.Substring(0, i), country);
                }
            }
            return new Pair(partname, "");
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
            string filenameNoExt = filename.Substring(0, filename.Length - file.Extension.Length);

            //rm_n2%5Bmdf.vq3%5D00.33.984%28h%40des.CountryHere%29.dm_68
            if (filename.Contains("%")) {
                filename = Uri.UnescapeDataString(filename);
            }
            Match match;

            if (!Ext.ContainsAny(filenameNoExt, "(", ")")) {
                if (Ext.CountOf(filenameNoExt, '_') >= 2) {

                    //tamb10_df.vq3_00.11.304_nL_HaZarD.Russia_
                    if (Ext.CountOf(filenameNoExt, '_') >= 4) {
                        match = Regex.Match(filenameNoExt, "([.][vV][qQ][3]|[.][cC][pP][mM]).*[_](\\d+[.]\\d{2}[.]\\d{3}|\\d{1,2}[.]\\d{3})");
                        if (match.Success && match.Groups.Count == 3) {
                            try {
                                //group 0 = .vq3_00.11.304
                                //group 1 = .vq3
                                //group 2 = 00.11.304
                                var indexBracket1 = filenameNoExt.Substring(0, match.Index).LastIndexOf('_');
                                var indexBracket2 = match.Groups[2].Index - 1;
                                var indexBracket3 = match.Groups[2].Index + match.Groups[2].Length;
                                var indexBracket4 = filenameNoExt.Length - 1;

                                var chars = filenameNoExt.ToCharArray();
                                chars[indexBracket1] = '[';
                                chars[indexBracket2] = ']';
                                chars[indexBracket3] = '(';
                                chars[indexBracket4] = ')';

                                return new string(chars) + file.Extension.ToLowerInvariant();
                            } catch (Exception ex) {
                            }
                        }
                    }

                    //runmikrob-4[df.vq3]00.14.488_JL.Ua_.dm_68
                    match = Regex.Match(filenameNoExt, "^.+[\\[].+[\\]](\\d+[.]\\d{2}[.]\\d{3}|\\d{1,2}[.]\\d{3})[_].+[_]$");
                    if (match.Success) {
                        //group 0 = runmikrob-4[df.vq3]00.14.488_JL.Ua_
                        //group 1 = 00.14.488

                        var indexBracket1 = match.Groups[1].Index + match.Groups[1].Length;
                        var indexBracket2 = filenameNoExt.Length - 1;

                        var chars = filenameNoExt.ToCharArray();
                        chars[indexBracket1] = '(';
                        chars[indexBracket2] = ')';
                        return new string(chars) + file.Extension.ToLowerInvariant();
                    }
                }

                //dfcomp009_3.792_VipeR_Russia.dm_68
                //dmp02a_jinx_13.880_t0t3r_germany.dm_68
                //fdcj2_3.408_[kzii]f_china.dm_66
                if (Ext.CountOf(filenameNoExt, '_') >= 3) {
                    match = Regex.Match(filenameNoExt, ".*[_](\\d+[.]\\d{2}[.]\\d{3}|\\d{1,2}[.]\\d{3})[_]");
                    if (match.Success && match.Groups.Count == 2) {
                        try {
                            //group 0 = dfcomp009_3.792_
                            //group 1 = 3.792
                            var mapnametime = match.Groups[0].Value.Substring(0, match.Groups[0].Value.Length - 1);
                            var playerCountry = filenameNoExt.Substring(match.Groups[0].Length);
                            var split = playerCountry.LastIndexOf('_');
                            if (split > 0) {
                                var pa = playerCountry.ToCharArray();
                                pa[split] = '.';
                                playerCountry = new string(pa);
                            }
                            return mapnametime + "(" + playerCountry + ")" + file.Extension.ToLowerInvariant();
                        } catch (Exception ex) {
                        }
                    }
                }

                //r7-falkydf.cpm00.09.960xas.China.dm_68
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

            //DraeliPowa1-[VQ3]-{InT33!Stormer}-{01.09.560}-[Russia].dm_68
            match = Regex.Match(filenameNoExt, "^.*-[[].{3,}]-{(.*)}-{(\\d+[.]\\d{2}[.]\\d{3}|\\d{1,2}[.]\\d{3})}-[[](.*)[]]$");
            if (match.Success && match.Groups.Count == 4) {
                //group 0 = DraeliPowa1-[VQ3]-{InT33!Stormer}-{01.09.560}-[Russia]
                //group 1 = InT33!Stormer
                //group 2 = 01.09.560
                //group 3 = Russia
                var p1 = filenameNoExt.Substring(0, match.Groups[1].Index - 2);
                return p1 + match.Groups[2].Value + "(" + match.Groups[1].Value + "." + match.Groups[3].Value + ")" + file.Extension.ToLowerInvariant();
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

        //this is userId what can be parsed from server recorded files like:
        //lick-dead[49576][1037].dm_68
        //lick-dead[mdf.vq3]00.49.576(pVquBit)[1037].dm_68
        //last square=userid, prelast=time
        static long tryGetUserIdFromFileName(FileInfo file)
        {
            if (file.Name.Count(x=>x == '[') < 2) {
                return 0;
            }
            var nameNoExt = file.Name.Substring(0, file.Name.Length - file.Extension.Length);
            var brackets = Regex.Matches(nameNoExt, "([\\[]\\d+[\\]]){2}$");
            if (brackets.Count == 1) {
                var value = brackets[0].Value;
                var sub = value.Split("[]".ToArray());
                if (sub.Length != 5) return 0;
                var stringId = sub[3];
                long id = 0;
                long.TryParse(stringId, out id);
                return id;
            }
            brackets = Regex.Matches(nameNoExt, ".+[\\[].+[\\]].+[(].+[)]([\\[]\\d+[\\]])$");
            if (brackets.Count != 1) {
                brackets = Regex.Matches(nameNoExt, ".+[\\[].+[\\]].+[(].+[)][{].+[}]([\\[]\\d+[\\]])$");
            }
            if (brackets.Count == 1) {
                var value = brackets[0].Value;
                var sub = value.Split("[]".ToArray());
                var stringId = sub[sub.Length - 2];
                long id = 0;
                long.TryParse(stringId, out id);
                return id;
            }
            return 0;
        }

        //check demo for validity, commmands ordered by relevance. first is more important
        static Dictionary<string, string> checkValidity(bool hasTime, bool hasRawTime, GameInfo gameInfo)
        {
            Dictionary<string, string> invalidParams = new Dictionary<string, string>();
            var kGame = Ext.LowerKeys(gameInfo.parameters);
            if (!gameInfo.isFreeStyle) {
                checkKey(invalidParams, kGame, "sv_cheats", 0);
            }

            checkKey(invalidParams, kGame, "timescale", 1);
            checkKey(invalidParams, kGame, "g_speed", 320);
            checkKey(invalidParams, kGame, "g_gravity", 800);
            checkKey(invalidParams, kGame, "handicap", 100);
            checkKey(invalidParams, kGame, "g_knockback", 1000);
            

            if (hasTime && !hasRawTime && gameInfo.isDefrag) {
                //If the demo was not found messages about the finish map
                invalidParams.Add("client_finish", "false");
            }

            if (hasTime && gameInfo.isOnline && !gameInfo.isFreeStyle) {
                //if the demo was recorded with group support and have time
                checkKey(invalidParams, kGame, "df_mp_interferenceoff", 3);
            }

            checkKey(invalidParams, kGame, "sv_fps", 125);
            checkKey(invalidParams, kGame, "com_maxfps", 125);

            //fixed by dfwc2019 rules:
            //a) "g_synchronousClients 1", and any values in "pmove_fixed", "pmove_msec".
            //b) "g_synchronousClients 0", "pmove_fixed 1", "pmove_msec 8".
            var gSync = getKey(kGame, "g_synchronousclients");
            if (gSync != 1) {
                checkKey(invalidParams, kGame, "pmove_msec", 8);
                checkKey(invalidParams, kGame, "pmove_fixed", 1);
            }

            checkKey(invalidParams, kGame, "g_killWallbug", 1);
            return invalidParams;
        }

        //checking the key for validity
        static void checkKey(Dictionary<string, string> invalidParams, Dictionary<string, string> keysGame, string key, int val) {
            if (keysGame.ContainsKey(key) && keysGame[key].Length > 0) {
                float value = getKey(keysGame, key);
                if (value < 0 || value != val) {
                    var keyValue = keysGame[key];
                    if (keyValue.StartsWith(".")) {     //edit the timescale display as .3 -> 0.3
                        keyValue = "0" + keyValue;
                    }
                    invalidParams.Add(key, keyValue);
                }
            }
        }

        static float getKey(Dictionary<string, string> keysGame, string key)
        {
            if (keysGame.ContainsKey(key)) {
                string strValue = keysGame[key];
                float value;
                if (keysGame.ContainsKey(key) && strValue.Length > 0) {
                    bool success = float.TryParse(strValue, NumberStyles.Number, CultureInfo.InvariantCulture, out value);
                    if (success) {
                        return value;
                    }
                }
            }
            return -1;
        }
    }
}
