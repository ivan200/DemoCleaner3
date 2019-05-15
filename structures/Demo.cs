using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DemoCleaner3.DemoParser.parser;
using System.Text.RegularExpressions;
using System.Globalization;
using DemoCleaner3.DemoParser.huffman;
using DemoCleaner3.DemoParser;

namespace DemoCleaner3
{
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

        string dfType;
        string physic;
        string modNum;

        string validity = "";
        public bool useValidation = true;
        public bool rawTime = false;

        public RawInfo rawInfo = null;

        private string _demoNewName = "";

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
                    //если есть тайм, то пишем нормальный name для демки
                    demoname = string.Format("{0}[{1}]{2:D2}.{3:D2}.{4:D3}({5})",
                    mapName, modphysic, (int) time.TotalMinutes, time.Seconds, time.Milliseconds, playerCountry);
                    hasCorrectName = true;
                } else {
                    hasCorrectName = false;
                    //если нет тайма, то мучаемся с генерацией текста
                    string oldName = file.Name;
                    oldName = oldName.Substring(0, oldName.Length - file.Extension.Length); //убираем расширение
                    oldName = removeSubstr(oldName, mapName);                               //убираем имя карты
                    oldName = removeSubstr(oldName, playerName, false);                     //убираем имя игрока
                    oldName = removeSubstr(oldName, country, false);                        //убираем страну
                    oldName = removeSubstr(oldName, modphysic);                             //убираем мод с физикой
                    oldName = removeSubstr(oldName, physic);                                //убираем физику
                    oldName = removeSubstr(oldName, validity);                              //убираем строки валидации
                    oldName = removeDouble(oldName);                                        //убираем двойные символы (кроме  скобочек)
                    oldName = oldName.Replace("[]", "").Replace("()", "");                  //убираем пустые скобки
                    //oldName = Regex.Replace(oldName, "(^[^[a-zA-Z0-9]+|[^[a-zA-Z0-9]+$)", "");
                    oldName = Regex.Replace(oldName, "(^[^[a-zA-Z0-9\\(\\)\\]\\[]|[^[a-zA-Z0-9\\(\\)\\]\\[]$)", "");    //убираем хрень в начале и в конце
                    oldName = oldName.Replace(" ", "_");                                    //убираем пробелы
                    
                    demoname = string.Format("{0}[{1}]{2}({3})", mapName, modphysic, oldName, playerCountry);

                    demoname = demoname.Replace(").)", ")").Replace(".)", ")");
                }

                if (useValidation && validity.Length > 0) {
                    demoname = demoname + "{" + validity + "}"; //добавляем инфу о валидации
                }

                _demoNewName = demoname + file.Extension;
                return _demoNewName;
            }
        }

        //убирание двойных небуквенных символов и замена на первый
        //например: test__abc-_.xy -> test_abc-xy
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

        //убирание подстроки с граничащими символами например: test_abc.xy -> test.xy
        //берётся последний символ если их 2, и первый если только слева: test_abcxy -> test_xy
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

        //Получаем из имени файла детали демки
        public static Demo GetDemoFromFile(FileInfo file)
        {
            Demo demo = new Demo();
            demo.file = file;
            demo.recordTime = demo.file.CreationTime;

            var sub = file.Name.Split("[]()".ToArray());
            if (sub.Length >= 4) {
                //Карта
                demo.mapName = sub[0];

                //Физика
                demo.modphysic = sub[1];
                if (demo.modphysic.Length < 3) {
                    demo.hasError = true;
                }

                //Время
                demo.timeString = sub[2];
                var times = demo.timeString.Split('-', '.');
                try {
                    demo.time = getTimeSpan(demo.timeString);
                } catch (Exception) {
                    demo.hasError = true;
                }

                //Имя + страна
                var countryName = sub[3];
                demo.country = tryGetCountryFromBrackets(countryName);
                demo.playerName = tryGetNameFromBrackets(countryName);
            } else {
                demo.hasError = true;
            }
            return demo;
        }

        //обработка группировки, если нажата галка с обработкой мдф как дф
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

        //Получаем заполненную демку из детальной инфы выдернутой из демки
        public static Demo GetDemoFromRawInfo(RawInfo raw)
        {
            var file = new FileInfo(raw.demoPath);

            var frConfig = raw.getFriendlyInfo();

            Demo demo = new Demo();

            demo.rawInfo = raw;

            //файл
            demo.file = file;
            if (frConfig.Count == 0  || !frConfig.ContainsKey(RawInfo.keyClient)) {
                demo.hasError = true;
                return demo;
            }

            string countryName = getNameAndCountry(file);

            //Имя
            string demoUserName = tryGetNameFromBrackets(countryName);  
            string dfName = null;
            string uName = null;

            if (frConfig.ContainsKey(RawInfo.keyPlayer)) {
                var kPlayer = frConfig[RawInfo.keyPlayer];

                dfName = Ext.GetOrNull(kPlayer, "dfn");
                uName = Ext.GetOrNull(kPlayer, "n");
                
                if (!string.IsNullOrEmpty(uName)) {
                    uName = Regex.Replace(uName, "\\^.", "");
                    uName = Regex.Replace(uName, "[^a-zA-Z0-9\\!\\#\\$\\%\\&\\'\\(\\)\\+\\,\\-\\.\\;\\=\\[\\]\\^_\\{\\}]", "");
                }

                if (string.IsNullOrEmpty(dfName) || dfName == "UnnamedPlayer") {
                    if (!string.IsNullOrEmpty(uName)) {
                        demo.playerName = uName;
                    } else {
                        demo.playerName = dfName;
                    }
                } else {
                    demo.playerName = dfName;
                }
            } else {
                demo.playerName = demoUserName;
            }

            //оффлайн тайм
            if (raw.performedTimes.Count == 1) {
                demo.time = getTimeSpanForDemo(raw.performedTimes[0]);
                if (raw.dateStamps.Count > 0) {
                    demo.recordTime = getDateForDemo(raw.dateStamps[0]);
                }
            } else if (raw.performedTimes.Count > 1) {
                double maxMillis = double.MaxValue;
                for (int i = 0; i < raw.performedTimes.Count; i++) {
                    var time = getTimeSpanForDemo(raw.performedTimes[i]);
                    if (time.TotalMilliseconds < maxMillis) {
                        demo.time = time;
                        if (i < raw.dateStamps.Count) {
                            demo.recordTime = getDateForDemo(raw.dateStamps[i]);
                        }
                    }
                }
            }

            //онлайн тайм
            if (raw.onlineTimes.Count == 1) {
                var onlineName = getOnlineName(raw.onlineTimes[0]);
                if (onlineName.Length > 0) {
                    demo.playerName = onlineName;
                }
                demo.time = getOnlineTimeSpan(raw.onlineTimes[0]);
            } else if (raw.onlineTimes.Count > 1) {
                double maxMillis = double.MaxValue;
                foreach (var timeString in raw.onlineTimes) {
                    var onlineName = getOnlineName(timeString);
                    if (onlineName == dfName || onlineName == uName || onlineName == demoUserName) {
                        var time = getOnlineTimeSpan(timeString);
                        if (time.TotalMilliseconds < maxMillis) {
                            demo.time = time;
                        }
                    }
                }
            }
            //хоть какой то тайм (из имени демки)
            if (demo.time.TotalMilliseconds > 0) {
                demo.rawTime = true;
            } else {
                var time = tryGetTimeFromFileName(file);
                if (time != null) {
                    demo.time = time.Value;
                }
            }

            //Карта
            var mapInfo = raw.rawConfig.ContainsKey(Q3Const.Q3_DEMO_CFG_FIELD_MAP) ? raw.rawConfig[Q3Const.Q3_DEMO_CFG_FIELD_MAP] : "";
            var mapName = Ext.GetOrNull(frConfig[RawInfo.keyClient], "mapname");

            //Если в mapInfo написано название этой же мапы, но с заглавными, то название берём оттуда
            if (mapName.ToLowerInvariant().Equals(mapInfo.ToLowerInvariant())) {
                demo.mapName = mapInfo;
            } else {
                demo.mapName = mapName.ToLowerInvariant();
            }

            //геймтайп
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

            if (gType == 0) {
                var vers = Ext.GetOrNull(frConfig[RawInfo.keyClient], "gamename");
                if (vers == "defrag"){
                    demo.dfType = "df";
                } else {
                    demo.dfType = "dm";
                }
            }

            //промод
            var promode = Ext.GetOrNull(frConfig[RawInfo.keyClient], "df_promode");
            if (!string.IsNullOrEmpty(promode)) {
                int.TryParse(promode, out int phMode);
                demo.physic = phMode == 1 ? "cpm" : "vq3";                  //vq3, cpm
            } 

            //мод для fastcaps и freestyle
            var dfMode = Ext.GetOrNull(frConfig[RawInfo.keyClient], "defrag_mode");
            if (!string.IsNullOrEmpty(dfMode)) {
                int.TryParse(dfMode, out int defragMode);                                                 //>=0 = mode
                demo.modNum = (gType != 1 && gType != 5) ? string.Format(".{0}", defragMode) : null;      //.0 - .7
            }

            //комбинируем
            if (demo.physic != null) {
                demo.modphysic = string.Format("{0}.{1}{2}", demo.dfType, demo.physic, demo.modNum);
            } else {
                demo.modphysic = demo.dfType;
            }

            //если есть читы, то пишем в демку
            demo.validity = checkValidity(frConfig, demo.time.TotalMilliseconds > 0, demo.rawTime);

            demo.country = tryGetCountryFromBrackets(countryName);
            return demo;
        }

        //Пытаемся получить имя и страну из демки (первое вхождение двух круглых скобочек)
        static string getNameAndCountry(FileInfo file) {
            var brackets = Regex.Match(file.Name, "\\([^)]*\\)");
            if (brackets.Success && brackets.Groups.Count > 0) {
                return brackets.Groups[0].Value.Replace("(", "").Replace(")", "");
            }
            return "";
        }

        static string tryGetNameFromBrackets(string partname)
        {
            int i = partname.LastIndexOf('.');
            if (i > 0) {
                partname = partname.Substring(0, i);
            }
            return partname;
        }

        //Пытаемся получить страну, и только её, из имени и страны
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

        //Пытаемся получить время из демки
        static TimeSpan? tryGetTimeFromFileName(FileInfo file)
        {
            var sub = file.Name.Split("[]()_".ToArray());
            foreach (string part in sub) {
                var time = tryGetTimeFromBrackets(part);
                if (time != null) {
                    return time;
                }
            }
            return null;
        }

        //Пытаемся получить время из куска имени демки
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
            return getTimeSpan(partname);
        }


        //получение времени из онлайн надписи
        static string getOnlineName(string demoTimeCmd)
        {
            //print \"Rom^7 reached the finish line in ^23:38:208^7\n\"
            demoTimeCmd = Regex.Replace(demoTimeCmd, "(\\^[0-9]|\\\"|\\n|\")", "");          //print Rom reached the finish line in 3:38:208
            string name = demoTimeCmd.Substring(6, demoTimeCmd.LastIndexOf(" reached") - 6); //Rom
            name = Regex.Replace(name, "[^a-zA-Z0-9\\!\\#\\$\\%\\&\\'\\(\\)\\+\\,\\-\\.\\;\\=\\[\\]\\^_\\{\\}]", "");
            return name;
        }

        static TimeSpan getOnlineTimeSpan(string demoTimeCmd) {
            //print \"Rom^7 reached the finish line in ^23:38:208^7\n\"
            demoTimeCmd = Regex.Replace(demoTimeCmd, "(\\^[0-9]|\\\"|\\n|\")", "");          //print Rom reached the finish line in 3:38:208
            string demoTime = demoTimeCmd.Substring(demoTimeCmd.LastIndexOf("in") + 3);      //3:38:208
            return getTimeSpan(demoTime);
        }

        //получение времени из оффлайн надписи
        static TimeSpan getTimeSpanForDemo(string demoTimeCmd)
        {
            //print "Time performed by ^2uN-DeaD!Enter^7 : ^331:432^7 (v1.91.23 beta)\n"

            //TODO проверить, что будет если в df_name будет со спецсимволами, например двоеточие, вопрос, кавычки
            demoTimeCmd = Regex.Replace(demoTimeCmd, "(\\^[0-9]|\\\"|\\n|\")", "");     //print Time performed  by uN-DeaD!Enter : 31:432 (v1.91.23 beta)

            demoTimeCmd = demoTimeCmd.Substring(demoTimeCmd.IndexOf(':') + 2);      //31:432 (v1.91.23 beta)
            demoTimeCmd = demoTimeCmd.Substring(0, demoTimeCmd.IndexOf(' ')).Trim();    //31:432

            return getTimeSpan(demoTimeCmd);
        }

        //получение времени из строки
        static TimeSpan getTimeSpan(string timeString)
        {
            var times = timeString.Split('-', '.', ':').Reverse().ToList();
            //так как мы реверснули таймы, милисекунды будут в начале
            return TimeSpan.Zero
                .Add(TimeSpan.FromMilliseconds  (times.Count > 0 ? int.Parse(times[0]) : 0))
                .Add(TimeSpan.FromSeconds       (times.Count > 1 ? int.Parse(times[1]) : 0))
                .Add(TimeSpan.FromMinutes       (times.Count > 2 ? int.Parse(times[2]) : 0));
            //делаем через таймспаны чтобы если минут больше 60, они корректно добавлялись
        }

        //получение даты записи демки если она есть
        static DateTime? getDateForDemo(string s)
        {
            //print "Date: 10-25-14 02:43\n"
            string dateString = s.Substring(13).Replace("\n", "").Replace("\"", "").Trim();
            try {
                return DateTime.ParseExact(dateString, "MM-dd-yy HH:mm", CultureInfo.InvariantCulture);
            } catch (Exception ex) {}
            return null;
        }


        //проверка демки на валидность
        static string checkValidity(Dictionary<string, Dictionary<string, string>> frConfig, bool hasTime, bool hasRawTime) {
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

            var fs = (gametype == 2 || gametype == 6);
            if (!fs) {
                res = checkKey(kGame, "sv_cheats", 0); if (res.Length > 0) return res;
            }

            if(online && !fs) { 
                res = checkKey(kGame, "df_mp_interferenceoff", 3);  if (res.Length > 0) return res;
            }

            res = checkKey(kGame, "timescale", 1);                  if (res.Length > 0) return res;
            res = checkKey(kGame, "g_speed", 320);                  if (res.Length > 0) return res;
            res = checkKey(kGame, "g_gravity", 800);                if (res.Length > 0) return res;
            res = checkKey(kGame, "g_knockback", 1000);             if (res.Length > 0) return res;

            if (hasTime && !hasRawTime) {
                return "client_finish=false";
            }

            res = checkKey(kGame, "pmove_msec", 8);                 if (res.Length > 0) return res;
            res = checkKey(kGame, "defrag_svfps", 125, "sv_fps");   if (res.Length > 0) return res;

            res = checkKey(kGame, "g_synchronousclients", (online ? 0 : 1), "g_sync"); if (res.Length > 0) return res;
            res = checkKey(kGame, "pmove_fixed", (online ? 1 : 0)); if (res.Length > 0) return res;
            return "";
        }

        //проверка ключа на валидность
        static string checkKey(Dictionary<string, string> keysGame, string key, int val, string errorString = "") {
            if (keysGame.ContainsKey(key) && keysGame[key].Length > 0) {
                var keyValue = keysGame[key];
                float value = -1;
                try {
                    value = float.Parse(keyValue, CultureInfo.InvariantCulture);
                } catch (Exception ex) {
                }
                if (value < 0 || value != (float) val) {
                    if (keyValue.StartsWith(".")) {     //правим отображение таймскейла как .3 -> 0.3
                        keyValue = "0" + keyValue;
                    }
                    return (errorString.Length > 0 ? errorString : key) + "=" + keyValue;
                }
            }
            return "";
        }
    }
}
