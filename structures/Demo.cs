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
using DemoCleaner3.DemoParser.utils;

namespace DemoCleaner3 {
    //Class to maintain all information about demo file
    public class Demo {
        public string mapName;
        public string modphysic;
        public string timeString;
        public TimeSpan time;
        public string playerName;
        public DemoNames names = null;
        public string country;
        public FileInfo file;
        public bool isBroken;
        public bool hasError;
        public bool hasCorrectName = false;
        public DateTime? recordTime;
        public bool hasTr = false;
        public bool isNotFinished = false;
        public bool isTas = false;  //tool assisted speedrun (boted, scripted, etc...)
        public static String[] tasTriggers =
            new String[] { "tas", "tasbot", "bot", "boted", "botland", "wiz", "wizland", "script", "scripted", "scriptland" }
            .OrderByDescending(x => x.Length)
            .ToArray();

        public Dictionary<string, string> validDict = new Dictionary<string, string>(); //incorrect validation params dictionary
        public string validity {
            get {
                if (validDict.Count > 0) {
                    var first = validDict.First();
                    return string.Format("{0}={1}", first.Key, first.Value);
                } else return "";
            }
        }

        public bool useValidation = true;
        public bool rawTime = false;                //time is obtained from triggers or console prints
        public bool triggerTime = false;            //time is obtained from trigger touching, not from prefs\
        public bool triggerTimeNoFinish = false;    //time from trigger touching has startTimer but have not stopTimer
        public bool isSpectator = false;

        public RawInfo rawInfo = null;
        private string _demoNewName = "";

        private string _demoNewNameSimple = "";
        public string demoNewNameSimple {           //demo name without {validity} or [spect] at end
            get {
                if (string.IsNullOrEmpty(_demoNewNameSimple)) {
                    fillDemoNewName();
                }
                return _demoNewNameSimple;
            }
        }

        private string _normalizedFileName = "";
        private string normalizedFileName {
            get {
                if (string.IsNullOrEmpty(_normalizedFileName)) {
                    _normalizedFileName = getNormalizedFileName(file);
                }
                return _normalizedFileName;
            }
        }

        public long userId = -1;

        //generating new demo name by all information
        public string demoNewName {
            get {
                if (!string.IsNullOrEmpty(_demoNewName)) {
                    return _demoNewName;
                }

                if (hasError) {
                    return normalizedFileName;
                }
                fillDemoNewName();

                return _demoNewName;
            }
        }

        public void fillDemoNewName() {
            string demoname = "";
            string playerCountry = country.Length > 0 ? playerName + "." + country : playerName;

            if (time.TotalMilliseconds > 0) {
                //if we have time, write a normal name for the demo
                demoname = string.Format("{0}[{1}]{2:D2}.{3:D2}.{4:D3}({5})",
                mapName, modphysic, (int)time.TotalMinutes, time.Seconds, time.Milliseconds, playerCountry);
                hasCorrectName = true;
            } else {
                hasCorrectName = false;
                //if there is no time, then tormented with the generation of text
                string oldName = normalizedFileName;
                oldName = oldName.Substring(0, oldName.Length - file.Extension.Length); //remove the extension
                oldName = removeSubstr(oldName, mapName);                               //remove the map name
                if (country.Length > 0) {
                    if (names != null && !string.IsNullOrEmpty(names.fName)) {
                        playerCountry = names.fName + "." + country;
                    }
                    oldName = removeSubstr(oldName, playerCountry, false);
                }
                oldName = oldName.Replace("[dm]", "");                                  //replace previous wrong mod detection
                oldName = oldName.Replace("[spect]", "");                               //replace spectate info

                var normalizedName = DemoNames.normalizeName(playerName);

                //remove normal name + country
                oldName = oldName.Replace(string.Format("({0}.{1})", normalizedName, country), "");
                oldName = oldName.Replace(string.Format("({0})", normalizedName), "");
                if (names != null && !string.IsNullOrEmpty(names.fName)) {
                    oldName = oldName.Replace(string.Format("({0}.{1})", names.fName, country), "");
                    oldName = oldName.Replace(string.Format("({0})", names.fName), "");
                }

                //remove name and country with custom brackets
                oldName = removeSubstr(oldName, normalizedName, false);                 //remove the player name
                if (names != null && !string.IsNullOrEmpty(names.fName)) {
                    oldName = removeSubstr(oldName, names.fName, false);
                }
                oldName = removeSubstr(oldName, country, false);                        //remove the country

                oldName = oldName.Replace(string.Format("[{0}]", modphysic), "");       //remove the mod with physics with default brackets
                oldName = removeSubstr(oldName, modphysic);                             //remove the mod with physics with custom brackets
                if (rawInfo != null && rawInfo.gameInfo != null) {
                    oldName = removeSubstr(oldName, rawInfo.gameInfo.gameNameShort);    //remove the mod
                }
                //oldName = removeSubstr(oldName, physic);                              //remove physics
                oldName = removeSubstr(oldName, validity);                              //remove validation lines
                oldName = removeDouble(oldName);                                        //remove double characters (except brackets)
                oldName = oldName.Replace("[]", "").Replace("()", "");                  //remove the empty brackets
                //oldName = Regex.Replace(oldName, "(^[^[a-zA-Z0-9]+|[^[a-zA-Z0-9]+$)", "");
                oldName = Regex.Replace(oldName, "(^[^[a-zA-Z0-9\\(\\)\\]\\[]|[^[a-zA-Z0-9\\(\\)\\]\\[]$)", "");    //we remove crap at the beginning and at the end of name
                oldName = oldName.Replace(" ", "_");                                    //remove_spaces

                demoname = string.Format("{0}[{1}]({2}){3}", mapName, modphysic, playerCountry, oldName);

                demoname = demoname.Replace(").)", ")").Replace(".)", ")");
            }

            _demoNewNameSimple = demoname + file.Extension.ToLowerInvariant();

            if (useValidation && validity.Length > 0) {
                demoname = demoname + "{" + validity + "}"; //add information about validation
            }
            if (userId >= 0) {
                demoname = demoname + "[" + userId.ToString() + "]"; //add userId (can be added only with triggertime)
            } else {
                if (isSpectator || (rawInfo != null && rawInfo.isSpectator)) {
                    demoname = demoname + "[spect]";                //add spectator tag
                }
            }

            _demoNewName = demoname + file.Extension.ToLowerInvariant();
        }

        //the removing of the double non-alphanumeric characters and replace at first
        //for example: test__abc-_.xy -> test_abc-xy
        static string removeDouble(string input) {
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
        static string removeSubstr(string input, string include, bool fromstart = true) {
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
            if (pos + include.Length < input.Length) {
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
        public static Demo GetDemoFromFile(FileInfo file) {
            Demo demo = new Demo();
            demo.file = file;
            var filename = file.Name;

            demo.recordTime = demo.file.CreationTime;

            string fileNameNoExt = filename.Substring(0, filename.Length - file.Extension.Length);
            var match = Regex.Match(fileNameNoExt, "(.+)\\[(.+)\\](\\d+\\.\\d{2}\\.\\d{3})\\((.+)\\)(\\{(.+)\\})?(\\[(.+)\\])?");
            if (match.Success && match.Groups.Count >= 5) {
                //Map
                demo.mapName = match.Groups[1].Value;

                //Physic
                demo.modphysic = match.Groups[2].Value;
                var physic = demo.modphysic.ToLowerInvariant();
                int index = Math.Max(physic.IndexOf(".cpm"), physic.IndexOf(".vq3"));
                if (index <= 0) demo.hasError = true;
                if (physic.Length < 3) demo.hasError = true;
                if (physic.Contains(".tr")) demo.hasTr = true;

                //Time
                demo.timeString = match.Groups[3].Value;
                try {
                    demo.time = ConsoleStringUtils.getTimeSpan(demo.timeString);
                } catch (Exception) {
                    demo.hasError = true;
                }
                if (demo.time.TotalMilliseconds <= 0) {
                    demo.hasError = true;
                }

                //Name + country
                var countryName = match.Groups[4].Value;
                var countryNameParsed = tryGetNameAndCountry(countryName, null);
                demo.playerName = countryNameParsed.Key;
                demo.country = countryNameParsed.Value;

                //Validity
                if (match.Groups.Count >= 7) {
                    addValidDict(demo, match.Groups[6].Value);
                }

                //tas check
                if (filename.ToLowerInvariant().Contains("tool_assisted=true")) {
                    demo.isTas = true;
                }

                //finish demo check
                if (filename.ToLowerInvariant().Contains("client_finish=false")) {
                    demo.isNotFinished = true;
                }

                //userId
                if (match.Groups.Count >= 9) {
                    var idString = match.Groups[8].Value;
                    if (!string.IsNullOrEmpty(idString)) {
                        if (isDigits(idString.ToCharArray())) {
                            long id = -1;
                            long.TryParse(idString, out id);
                            if (id >= 0) {
                                demo.userId = id;
                            }
                        } else {
                            if (idString == "spect") {
                                demo.isSpectator = true;
                            }
                        }
                    }
                }
            } else {
                //ospdm1[df.vq3](bsh.ThrasheR)t_school_3xgj{sv_cheats=1}
                match = Regex.Match(fileNameNoExt, "^(.+)\\[(.+)\\]\\((.+)\\)(.+)$");
                if (match.Success) {
                    demo.mapName = match.Groups[1].Value;
                    demo.modphysic = match.Groups[2].Value;
                    var countryName = match.Groups[3].Value;
                    var countryNameParsed = tryGetNameAndCountry(countryName, null);
                    demo.playerName = countryNameParsed.Key;
                    demo.country = countryNameParsed.Value;

                    var title = match.Groups[4].Value;
                    match = Regex.Match(title, "^.+\\{(.+)\\}$");
                    if (match.Success) {
                        addValidDict(demo, match.Groups[1].Value);
                    }
                }
                //ospdm1[df.vq3]t_school_3xgj(bsh.ThrasheR){sv_cheats=1}
                match = Regex.Match(fileNameNoExt, "^(.+)\\[(.+)\\](.+)\\((.+)\\)(\\{(.+)\\})?$");
                if (match.Success) {
                    demo.mapName = match.Groups[1].Value;
                    demo.modphysic = match.Groups[2].Value;
                    var countryName = match.Groups[4].Value;
                    var countryNameParsed = tryGetNameAndCountry(countryName, null);
                    demo.playerName = countryNameParsed.Key;
                    demo.country = countryNameParsed.Value;
                    //Validity
                    if (match.Groups.Count == 7) {
                        addValidDict(demo, match.Groups[6].Value);
                    }
                }
                demo.hasError = true;
            }
            return demo;
        }

        private static void addValidDict(Demo demo, string validString) {
            var v = validString.Split('=');
            if (v.Length > 1) {
                demo.validDict = new Dictionary<string, string>();
                demo.validDict.Add(v[0], v[1]);
            }
        }

        public static string getModPhysic(string filename) {
            int index = Math.Max(filename.IndexOf(".cpm"), filename.IndexOf(".vq3"));
            if (index <= 0) return null;

            int firstSquareIndex = filename.Substring(0, index).LastIndexOf('[');
            int secondSquareIndex = filename.Substring(index + 1).IndexOf(']');

            if (firstSquareIndex < 0 || secondSquareIndex < 0) return null;

            var modphysic = filename.Substring(firstSquareIndex + 1, secondSquareIndex);
            if (modphysic.Length < 3) return null;

            return modphysic;
        }


        //processing grouping files, if selected with processing mdf as df
        public static string mdfToDf(string mod, bool processIt) {
            if (processIt && mod.Length > 0 && mod[0] == 'm') {
                return mod.Substring(1);
            }
            return mod;
        }


        public static Demo GetDemoFromFileRaw(FileInfo file) {
            Q3HuffmanMapper.init();
            var raw = Q3DemoParser.getRawConfigStrings(file.FullName);
            return Demo.GetDemoFromRawInfo(raw);
        }

        //We get the filled demo from the full raw information pulled from the demo
        public static Demo GetDemoFromRawInfo(RawInfo raw) {
            var file = new FileInfo(raw.demoPath);

            var frConfig = raw.getFriendlyInfo();

            Demo demo = new Demo();
            demo.rawInfo = raw;

            //file
            demo.file = file;
            if (frConfig.Count == 0
                || !frConfig.ContainsKey(RawInfo.keyClient)
                || frConfig[RawInfo.keyClient].Count == 0
                //|| (frConfig.ContainsKey(RawInfo.keyErrors) && frConfig[RawInfo.keyErrors].ContainsValue(new ErrorWrongLength().Message))
                || (frConfig.ContainsKey(RawInfo.keyErrors) && frConfig[RawInfo.keyErrors].ContainsValue(new ErrorBadCommandInParseGameState().Message))
                ) {
                demo.hasError = true;
                demo.isBroken = true;
                return demo;
            }

            //config names
            var names = new DemoNames();
            names.setNamesByPlayerInfo(Ext.GetOrNull(frConfig, RawInfo.keyPlayer));

            var fastestTimeString = raw.consoleComandsParser.getFastestTimeStringInfo(names);

            //time from triggers
            if (raw.fin.HasValue) {
                if (!raw.fin.Value.Value.timeHasError) {
                    //if we can decode time from triggers, then use it
                    demo.time = TimeSpan.FromMilliseconds(raw.fin.Value.Value.time);
                }
                demo.hasTr = raw.fin.Value.Key == RawInfo.FinishType.CORRECT_TR;
                demo.triggerTime = true;
            } else {
                demo.hasTr = isTr(raw, fastestTimeString);
            }
            if (raw.clientEvents.Any(x => x.eventStartTime || x.eventTimeReset) && !raw.clientEvents.Any(x => x.eventFinish)) {
                demo.triggerTimeNoFinish = true;
            }

            if (demo.time.TotalMilliseconds <= 0) {
                //time from commands
                if (fastestTimeString != null) {
                    demo.time = fastestTimeString.time;
                    if (raw.consoleComandsParser.dateStrings.Count > 0) {
                        demo.recordTime = raw.consoleComandsParser.dateStrings.Last().recordDate;
                    }

                    var user = raw.getPlayerInfoByPlayerName(fastestTimeString.oName);
                    if (user != null) {
                        names.setNamesByPlayerInfo(user);
                    }
                } else {
                    //if we can't find the time in the commands, and the trigger decoding time has an error, 
                    //we will get the result from the servertime diff
                    if (raw.fin.HasValue) {
                        demo.time = TimeSpan.FromMilliseconds(raw.fin.Value.Value.timeByServerTime);
                    }
                }
            }
            if (raw.consoleComandsParser.dateStrings.Count > 0) {
                demo.recordTime = raw.consoleComandsParser.dateStrings.LastOrDefault(x => x.recordDate != null)?.recordDate;
            }

            if (fastestTimeString != null) {
                names.setConsoleName(fastestTimeString.oName, raw.gameInfo.isOnline);
            }

            var filename = demo.normalizedFileName;
            var countryAndName = getNameAndCountry(filename);

            var countryNameParsed = tryGetNameAndCountry(countryAndName, names);
            var normalName = names.chooseNormalName();
            if (normalName == null || normalName == DemoNames.defaultName) {
                names.setBracketsName(countryNameParsed.Key);   //name from the filename
            }

            //player name
            demo.playerName = names.chooseNormalName();
            demo.names = names;

            //demo has not info about country, so take it from filename
            demo.country = countryNameParsed.Value;

            //tas check
            if (filename.ToLowerInvariant().Contains("tool_assisted=true")
                || Ext.ContainsAnySplitted(countryAndName, tasTriggers)
                || Ext.ContainsAnySplitted(demo.playerName, tasTriggers)
                ) {
                demo.isTas = true;
            }

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
            var mapName = Ext.GetOrNull(frConfig[RawInfo.keyClient], "mapname") ?? "";

            //If in mapInfo the name of the same map is written, then we take the name from there
            if (mapName.ToLowerInvariant().Equals(mapInfo.ToLowerInvariant())) {
                demo.mapName = mapInfo;
            } else {
                demo.mapName = mapName.ToLowerInvariant();
            }

            if (mapName.Length == 0) {
                demo.isBroken = true;
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

            if (demo.hasTr) {
                demo.modphysic = string.Format("{0}.{1}", demo.modphysic, "tr");
            }

            //parameters from additionalInfo (defrag <= 1.81)
            Dictionary<string, string> info = null;
            if (raw.consoleComandsParser.additionalInfos.Count > 0) {
                info = raw.consoleComandsParser.additionalInfos.Last().toDictionary();  //any info is enough because it used only for validity check
            }

            //If demo has cheats, write it
            demo.validDict = checkValidity(demo.time.TotalMilliseconds > 0, demo.rawTime, gInfo, demo.isTas, demo.triggerTimeNoFinish, info);

            if (demo.validDict.Count == 0) {
                var filenameValidity = getValidities(filename);
                if (filenameValidity.HasValue) {
                    demo.validDict.Add(filenameValidity.Value.Key, filenameValidity.Value.Value);
                }
            }

            if (demo.triggerTime) {
                demo.userId = tryGetUserIdFromFileName(file);
            }
            if (Ext.GetOrNull(demo.validDict, "client_finish") == "false") {
                demo.isNotFinished = true;
            }

            return demo;
        }

        //check for tr without having correct triggers, by console commands and additionalInfos
        static bool isTr(RawInfo raw, TimeStringInfo fastestTimeString) {
            if (raw.clientEvents.Any(x => x.eventTimeReset)) return true;
            var additional = raw.consoleComandsParser.additionalInfos;
            if (fastestTimeString != null && additional.Count > 0) {
                var same = raw.consoleComandsParser.additionalInfos.Where(x => x.time == fastestTimeString.time);
                if (same.Count() > 0) {
                    return same.First().isTr;
                }
            }
            return false;
        }

        //We are trying to get the name and country from the demo (the first occurrence of two round brackets)
        static string getNameAndCountry(string filename) {
            var match = Regex.Match(filename, "[^(]*\\((.*)\\).*");
            if (match.Success && match.Groups.Count > 1) {
                return match.Groups[1].Value;
            }
            return "";
        }

        //trying to keep custom validities like
        //dfwc2014-5[df.vq3]00.36.904(uN-DeaD!Enter.Russia){old_map_version=true}.dm_68
        static Pair? getValidities(string filename) {
            var match = Regex.Match(filename, "^[^\\[]+\\[[^\\.\\]]+.[^\\]]+]\\d{2,3}\\.\\d{2}\\.\\d{3}\\(.+\\){(\\w+)=(\\w+)}(?:\\[\\d+\\])?\\.\\w+$");
            if (match.Success && match.Groups.Count > 1) {
                return new Pair(match.Groups[1].Value, match.Groups[2].Value);
            }
            return null;
        }

        //We are trying to split name and country
        static Pair tryGetNameAndCountry(string partname, DemoNames names) {
            var country = "";
            if (names != null && (partname == names.dfName || partname == names.uName || partname == names.oName || partname == names.cName)) {
                //name can contains dots so if username from parameters equals part in brackets, no country here
                return new Pair(partname, country);
            }
            int i = partname.LastIndexOf('.');
            if (i < 0) {
                i = partname.LastIndexOf(',');
            }
            if (i > 0 && i + 1 < partname.Length) {
                country = partname.Substring(i + 1, partname.Length - i - 1).Trim();
                country = ConsoleStringUtils.removeColors(country);
                if (country.Where(c => char.IsNumber(c)).Count() == 0) {
                    return new Pair(partname.Substring(0, i), country);
                }
            }
            return new Pair(partname, "");
        }

        //Trying to get time from the demo filename
        static TimeSpan? tryGetTimeFromFileName(string filename) {
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
        static TimeSpan? tryGetTimeFromBrackets(string partname) {
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
            return ConsoleStringUtils.getTimeSpan(partname);
        }

        //Normalization of demo filenames in case they broken by discord or net
        static string getNormalizedFileName(FileInfo file) {
            string filename = file.Name;

            //rm_n2%5Bmdf.vq3%5D00.33.984%28h%40des.CountryHere%29.dm_68
            if (filename.Contains("%")) {
                filename = Uri.UnescapeDataString(filename);
            }

            string filenameNoExt = filename.Substring(0, filename.Length - file.Extension.Length);

            if (filenameNoExt.ToLowerInvariant().Contains(" — копия")) {
                filenameNoExt = Regex.Replace(filenameNoExt, "( — [к|К]опия( \\(\\d+\\))?)+", "");
            }
            if (filenameNoExt.ToLowerInvariant().Contains(" — copy")) {
                filenameNoExt = Regex.Replace(filenameNoExt, "( — [c|C]opy( \\(\\d+\\))?)+", "");
            }
            if (filenameNoExt.Contains("^")) {
                filenameNoExt = ConsoleStringUtils.removeColors(filenameNoExt);
            }

            Match match;
            if (!Ext.ContainsAny(filenameNoExt, "(", ")")) {
                if (Ext.CountOf(filenameNoExt, '_') >= 2) {

                    //tamb10_df.vq3_00.11.304_nL_HaZarD.Russia_
                    if (Ext.CountOf(filenameNoExt, '_') >= 4) {
                        match = Regex.Match(filenameNoExt, "(?i)(.vq3|.cpm).*_(\\d+[.]\\d{2}[.]\\d{3}|\\d{1,2}[.]\\d{3})");
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

                    //dfcomp009_3.792_VipeR_Russia.dm_68
                    //dmp02a_jinx_13.880_t0t3r_germany.dm_68
                    //fdcj2_3.408_[kzii]f_china.dm_66
                    if (Ext.CountOf(filenameNoExt, '_') >= 3) {
                        match = Regex.Match(filenameNoExt, ".*[_](\\d+[.]\\d{2}[.]\\d{3}|\\d{1,2}[.]\\d{3}|\\d{5}|\\d{4})[_]");
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

                    //dfcomp012_00.07.128_eS-HosT.russia.dm_68
                    //dfcomp012_00.07.128_eS-HosT.russia_.dm_68
                    //runmikrob-4[df.vq3]00.14.488_JL.Ua_.dm_68
                    match = Regex.Match(filenameNoExt, "^.+(?>_|\\[.+\\])(?>\\d{2}[.]\\d{2}[.]\\d{3}|\\d{1,2}[.]\\d{3})(_).+$");
                    if (match.Success) {
                        var chars = filenameNoExt.ToCharArray();
                        chars[match.Groups[1].Index] = '(';
                        if (chars[chars.Length - 1] == '_') {
                            chars[chars.Length - 1] = ')';
                            return new string(chars) + file.Extension.ToLowerInvariant();
                        } else {
                            return new string(chars) + ')' + file.Extension.ToLowerInvariant();
                        }
                    }
                }

                //r7-falkydf.cpm00.09.960xas.China.dm_68
                int index = Math.Max(filenameNoExt.IndexOf(".cpm"), filenameNoExt.IndexOf(".vq3"));
                if (index > 0) {
                    try {
                        int i1 = filenameNoExt[index - 3] == 'm' ? index - 3 : index - 2;
                        int i2 = filenameNoExt[index + 4] == '.' ? index + 6 : index + 4;
                        int i3 = i2 + 9;
                        string mapname = filenameNoExt.Substring(0, i1);
                        string physic = filenameNoExt.Substring(i1, i2 - i1);
                        string time = filenameNoExt.Substring(i2, i3 - i2);
                        string name = filenameNoExt.Substring(i3);
                        if (isDigits(time[0], time[1], time[3], time[4], time[7], time[8])) {
                            return string.Format("{0}[{1}]{2}({3}){4}", mapname, physic, time, name, file.Extension);
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
        static long tryGetUserIdFromFileName(FileInfo file) {
            if (file.Name.Count(x => x == '[') < 2) {
                return -1;
            }
            long id = -1;

            var nameNoExt = file.Name.Substring(0, file.Name.Length - file.Extension.Length);

            var v1 = Regex.Match(nameNoExt, "^.+\\[\\d+\\]\\[(\\d+)\\]$");
            //lick-dead[49576][1037].dm_68
            if (v1.Groups.Count == 2) {
                long.TryParse(v1.Groups[1].Value, out id);
                return id;
            }

            var v2 = Regex.Match(nameNoExt, $"^.+\\[.+\\].+\\(.+\\)(?:{{.+}})*\\[(\\d+)\\]$");
            //fried-rust[df.cpm]00.26.304(uN-DeaD!Enter.Russia){dsads}[100]
            //fried-rust[df.cpm]00.26.304(uN-DeaD!Enter.Russia)[100]
            if (v2.Groups.Count == 2) {
                long.TryParse(v2.Groups[1].Value, out id);
                return id;
            }
            return -1;
        }

        //check demo for validity, commmands ordered by relevance. first is more important
        static Dictionary<string, string> checkValidity(
            bool hasTime,
            bool hasRawTime,
            GameInfo gameInfo,
            bool isTas,
            bool triggerTimeNoFinish,
            Dictionary<string, string> additionalInfo
            ) {
            Dictionary<string, string> invalidParams = new Dictionary<string, string>();
            var kGame = Ext.LowerKeys(gameInfo.parameters);
            if (additionalInfo != null) {
                kGame = Ext.Join(additionalInfo, kGame);
            }
            if (!gameInfo.isFreeStyle) {
                checkKey(invalidParams, kGame, "sv_cheats", 0);
            }

            if (gameInfo.isDefrag && ((hasTime && !hasRawTime) || triggerTimeNoFinish)) {
                //If the demo was not found messages about the finish map
                invalidParams.Add("client_finish", "false");
            }

            checkKey(invalidParams, kGame, "timescale", 1);
            checkKey(invalidParams, kGame, "g_speed", 320);
            checkKey(invalidParams, kGame, "g_gravity", 800);
            checkKey(invalidParams, kGame, "handicap", 100);
            checkKey(invalidParams, kGame, "g_knockback", 1000);

            if (hasTime && gameInfo.isOnline && !gameInfo.isFreeStyle) {
                //if the demo was recorded with group support and have time
                checkKey(invalidParams, kGame, "df_mp_interferenceoff", 3);
            }

            if (isTas) {
                //if demo was tool-assisted (scripted, boted...)
                invalidParams.Add("tool_assisted", "true");
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
                if (value < 0) {
                    invalidParams.Add(key, keysGame[key]);
                } else if (value != val) {
                    invalidParams.Add(key, Convert.ToString(value, CultureInfo.InvariantCulture));
                }
            }
        }

        static float getKey(Dictionary<string, string> keysGame, string key) {
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
