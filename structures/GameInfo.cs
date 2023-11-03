using System;
using System.Collections.Generic;
using System.Text;

namespace DemoCleaner3.structures
{
    public class GameInfo
    {
        public Dictionary<string, string> parameters = null;

        public bool isDefrag = false;
        public bool isFreeStyle = false;
        public bool isOnline = true;

        public string gameName;            //Defrag             //Orange Smoothie Productions
        public string gameNameShort;       //defrag             //osp
        public string gameType;            //Online defrag      //Team Deathmatch
        public string gameTypeShort;       //mdf                //tdm
        public string gameplayType;        //Challenge ProMode  //Vanilla Quake3
        public string gameplayTypeShort;   //CPM                //VQ3
        public string modType;             //4                
        public string modTypeName;         //Weapons Only

        public GameInfo(Dictionary<string, string> parameters, bool? isCpmInSnapshots) {
            this.parameters = Ext.LowerKeys(parameters);

            var gn = getGameName();

            gameNameShort = gn.Key;
            gameName = gn.Value;

            var gt = getGameType();
            gameTypeShort = gt.Key;
            gameType = gt.Value;

            gameplayTypeShort = getGameplayTypeShort(isCpmInSnapshots);
            gameplayType = getGameplayType();

            var mt = getModType();
            modType = mt.Key;
            modTypeName = mt.Value;
        }

        Pair getModType() {
            var defrag_gametype = Ext.GetOrZero(parameters, "defrag_gametype");
            if (defrag_gametype > 1 && defrag_gametype != 5) {  //df and mdf have no modtext
                var dfMode = Ext.GetOrZero(parameters, "defrag_mode");
                return new Pair(dfMode.ToString(), getDfModText(dfMode));
            }
            if (gameTypeShort == "fc") {
                int dfMode;
                //In defrag 1.80 was no defrag_mode param. Convert "all_weapons" into "defrag_mode" param
                var allWeapMode = Ext.ToInt(Ext.GetOrNull(parameters, "all_weapons"), -1);
                switch (allWeapMode) {
                    case 0: dfMode = 7; break;  //all_weapons 0 = Pickup            = Game Mode 7 (Original Quake 3)
                    case 1: dfMode = 2; break;  //all_weapons 1 = Give All, No BFG  = Game Mode 2 (weapons, map objects)
                    case 2: dfMode = 8; break;  //all_weapons 2 = Give All          = Game Mode 8 (Custom)
                    case 3: dfMode = 3; break;  //all_weapons 3 = No weapons        = Game Mode 3 (no weapon, map objects)
                    default: dfMode = 8; break; //all other modes considered custom
                }
                return new Pair(dfMode.ToString(), getOldDfModText(allWeapMode));
            }
            return new Pair("", "");
        }

        static string getDfModText(int dfMode) {
            switch (dfMode) {
                case 0: return "Custom";
                case 1: return "No weapon / No map objects";
                case 2: return "Weapons & Map Objects";
                case 3: return "Map Objects Only";
                case 4: return "Weapons Only";
                case 5: return "Swinging Hook";
                case 6: return "Quake3 Hook";
                case 7: return "Original quake 3";
                case 8: return "Custom";
            }
            return "";
        }

        static string getOldDfModText(int dfMode) {
            switch (dfMode) {
                case 0: return "Pickup";
                case 1: return "Give All, No BFG";
                case 2: return "Give All";
                case 3: return "No weapons";
                default: return "Custom";
            }
        }

        Pair getGameName() {
            var game = (Ext.GetOrNull(parameters, "fs_game") ?? "").ToLowerInvariant();
            var gName = (Ext.GetOrNull(parameters, "gamename") ?? "").ToLowerInvariant();
            var gameversion = (Ext.GetOrNull(parameters, "gameversion") ?? "").ToLowerInvariant();
            var df_vers = (Ext.GetOrNull(parameters, "defrag_vers") ?? "").ToLowerInvariant();
            var df_version = (Ext.GetOrNull(parameters, "defrag_version") ?? "").ToLowerInvariant();    //1.61


            if (game.StartsWith("defrag") || gName == "defrag" || !string.IsNullOrEmpty(df_vers) || !string.IsNullOrEmpty(df_version)) {
                isDefrag = true;
                return new Pair("defrag", "Defrag");
            }

            switch (game) {
                case "cpma":                return new Pair("cpma", "Challenge ProMode Arena");
                case "osp":                 return new Pair("osp", "Orange Smoothie Productions");
                case "arena":               return new Pair("ra3", "Rocket Arena");
                case "threewave":           return new Pair("q3w", "Threewave CTF");
                case "freeze":              return new Pair("q3ft", "Freeze Tag");
                case "ufreeze":             return new Pair("q3uft", "Ultra Freeze Tag");
                case "q3ut":                return new Pair(game, "Urban Terror");
                case "excessiveplus":       return new Pair("q3xp", "Excessive Plus");
                case "excessive":           return new Pair("q3ex", "Excessive");
                case "reactance:iu":        return new Pair("q3insta", "InstaUnlagged");   //Reactance:IU
                case "battle":              return new Pair(game, "Battle");
                case "beryllium":           return new Pair(game, "Beryllium");
                case "bma":                 return new Pair(game, "Black Metal Assault");
                case "the corkscrew mod":   return new Pair("corkscrew", "The CorkScrew Mod");
                case "f4a":                 return new Pair(game, "Freeze For All");
                case "freezeplus":          return new Pair("fp", "Freeze Plus");
                case "generations":         return new Pair("gen", "Generations");
                case "nemesis":             return new Pair(game, "Nemesis");
                case "noghost":             return new Pair(game, "NoGhost");
                case "q3f":
                case "q3f2":                return new Pair("q3f", "Quake 3 Fortress");
                case "truecombat":
                case "q3tc":                return new Pair(game, "Quake 3 True Combat");
            }

            if (gameversion.StartsWith("osp")) {
                return new Pair("osp", "Orange Smoothie Productions");
            }
            var xp_version = (Ext.GetOrNull(parameters, "xp_version") ?? "").ToLowerInvariant();
            if (xp_version.StartsWith("xp")) {
                return new Pair("q3xp", "Excessive Plus");
            }
            if (game.StartsWith("pkarena")) {
                return new Pair(game, "Painkeep");
            }
            if (game.Contains("unlagged")) {
                return new Pair("unlagged", "Unlagged");
            }
            if (game.Contains("westernq3")) {
                return new Pair("westernq3", "Western Quake 3");
            }
            return new Pair("q3a", "Quake 3 Arena");
        }

        string getGameplayTypeShort(bool? isCpmInSnapshots) {
            int server_promode = 0;
            switch (gameNameShort) {
                case "defrag":
                    if (isCpmInSnapshots != null) {
                        return isCpmInSnapshots == true ? "cpm" : "vq3";
                    } else {
                        var promode = Ext.GetOrZero(parameters, "df_promode");
                        return promode > 0 ? "cpm" : "vq3";
                    }
                case "cpma":
                    string server_gameplay = Ext.GetOrNull(parameters, "server_gameplay") ?? "";
                    switch (server_gameplay) {
                        case "0": case "vq3": return "vq3";
                        case "1": case "pmc": return "pmc";     //ProMode Classic
                        case "2": case "cpm": return "cpm";
                        case "cq3": return "cq3";               //Challenge Quake3
                    }
                    server_promode = Ext.GetOrZero(parameters, "server_promode");
                    return server_promode > 0 ? "cpm" : "vq3";
                case "osp":
                    server_promode = Ext.GetOrZero(parameters, "server_promode");
                    return server_promode > 0 ? "cpm" : "vq3";
            }
            return "";
        }

        string getGameplayType() {
            switch (gameplayTypeShort) {
                case "vq3": return "Vanilla Quake3";
                case "cpm": return "Challenge ProMode";
                case "pmc": return "ProMode Classic";
                case "cq3": return "Challenge Quake3";
            }
            return "";
        }

        Pair getGameType() {
            int g_gametype = Ext.GetOrZero(parameters, "g_gametype");
            switch (gameNameShort) {
                case "defrag":
                    int dfGType = Ext.GetOrZero(parameters, "defrag_gametype");
                    isFreeStyle = dfGType == 2 || dfGType == 6;
                    isOnline = dfGType > 4;
                    switch (dfGType) {
                        case 1: return new Pair("df", "Offline Defrag");
                        case 2: return new Pair("fs", "Offline Freestyle");
                        case 3: return new Pair("fc", "Offline Fast Caps");

                        case 5: return new Pair("mdf", "Multiplayer Defrag");
                        case 6: return new Pair("mfs", "Multiplayer Freestyle");
                        case 7: return new Pair("mfc", "Multiplayer Fast Caps");
                    }
                    if (g_gametype == 4) {
                        //In defrag 1.80 was no defrag_gametype param, but was fastcaps. Check fc like in baseq3
                        //kineterra1[fc.vq3]00.28.312(-AIR-).dm_68
                        //kiakrctf[fc.vq3]00.08.728(genosh).dm_68
                        return new Pair("fc", "Offline Fast Caps");
                    }
                    return new Pair("df", "Offline Defrag");

                case "cpma":
                    switch (g_gametype) {
                        case 5: return new Pair("ca", "Clan Arena");
                        case 6: return new Pair("ft", "Freeze Tag");
                        case 7: return new Pair("ctfs", "Capturestrike");
                        case 8: return new Pair("ntf", "Not Team Fortress");
                        case -1: return new Pair("hm", "Hoonymode");
                    }
                    break;
                case "osp":
                    if (g_gametype >= 5) {
                        switch (g_gametype) {
                            case 5: return new Pair("ca", "Clan Arena");
                            default:
                                int server_freezetag = Ext.GetOrZero(parameters, "server_freezetag");
                                if (server_freezetag == 1) return new Pair("fto", "Freeze Tag (OSP)");
                                if (server_freezetag == 2) return new Pair("ftv", "Freeze Tag (Vanilla)");
                                break;
                        }
                    }
                    break;
                case "q3w":
                    string g_serverData = (Ext.GetOrNull(parameters, "g_serverdata") ?? "").ToUpperInvariant();

                    if (g_serverData.Contains("G00")) return new Pair("ffa", "Free for All");
                    if (g_serverData.Contains("G01")) return new Pair("1v1", "Duel");
                    if (g_serverData.Contains("G03")) return new Pair("tdm", "Team Deathmatch");
                    if (g_serverData.Contains("G04")) return new Pair("ctf", "Capture the Flag");
                    if (g_serverData.Contains("G05")) return new Pair("ofc", "One Flag CTF");
                    if (g_serverData.Contains("G09")) return new Pair("ctfs", "Capturestrike");
                    if (g_serverData.Contains("G10") || g_serverData.Contains("G010")) return new Pair("cctf", "Classic CTF");
                    if (g_serverData.Contains("G11") || g_serverData.Contains("G011")) return new Pair("ar", "Arena");
                    break;
                case "q3ut":
                    switch (g_gametype) {
                        case 0: return new Pair("ffa", "Free for All");
                        case 1: return new Pair("ffa", "Free for All");
                        case 3: return new Pair("tdm", "Team Deathmatch");
                        case 4: return new Pair("tsv", "Team Survivor");
                        case 5: return new Pair("ftl", "Follow the Leader");
                        case 6: return new Pair("ch", "Capture & Hold");
                        case 7: return new Pair("ctf", "Capture the Flag");
                        case 8: return new Pair("bd", "Bomb & Defuse");
                    }
                    break;
                case "q3xp":
                    switch (g_gametype) {
                        case 5: return new Pair("rtf", "Return The Flag");
                        case 6: return new Pair("ofc", "One Flag CTF");
                        case 7: return new Pair("ca", "Clan Arena");
                        case 8: return new Pair("ft", "Freeze Tag");
                        case 9: return new Pair("ptl", "Protect The Leader");
                    }
                    break;
            }

            //"baseq3"
            switch (g_gametype) {
                case 0: return new Pair("ffa", "Free for All");
                case 1: return new Pair("1v1", "Duel");
                case 2: return new Pair("ffa", "Free for All");
                case 3: return new Pair("tdm", "Team Deathmatch");
                case 4: return new Pair("ctf", "Capture the Flag");
            }
            return new Pair("ffa", "Free for All");
        }
    }
}
