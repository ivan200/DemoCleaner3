using System;
using System.Collections.Generic;
using System.Text;

namespace DemoCleaner3.structures
{
    
    class GameInfo
    {
        Dictionary<string, string> parameters = null;

        public string gameName;            //Defrag          //OSP
        public string gameNameShort;       //defrag          //osp
        public string gameType;            //Online defrag   //Team DM  
        public string gameTypeShort;       //mdf             //tdm
        public string gameplayType;        //CPM             //VQ3
        public string modType;             //4                
        public string modTypeName;         //Weapons Only

        public GameInfo(Dictionary<string, string> parameters, bool hasRawTime) {
            this.parameters = parameters;

            var gn = getGameName(hasRawTime);

            gameNameShort = gn.Key;
            gameName = gn.Value;

            var gt = getGameType();
            gameTypeShort = gt.Key;
            gameType = gt.Value;

            gameplayType = getGameplayType();
             
            var mt = getModType();
            modType = mt.Key;
            modTypeName = mt.Value;
        }

        Pair getModType() {
            var defrag_gametype = Ext.GetOrZero(parameters, "defrag_gametype");
            if (defrag_gametype > 1 && defrag_gametype != 5) {
                var dfMode = Ext.GetOrZero(parameters, "defrag_mode");
                return new Pair(dfMode.ToString(), getDfModText(dfMode));
            }
            return new Pair("", "");
        }

        static string getDfModText(int dfMode) {
            switch (dfMode) {
                case 1: return "Neither";
                case 2: return "Weapons & Map Objects";
                case 3: return "Map Objects Only";
                case 4: return "Weapons Only";
                case 5: return "Swinging Hook";
                case 6: return "VQ3 Hook";
                case 7: return "VQ3";
                case 8: return "Custom";
            }
            return "";
        }

        Pair getGameName(bool hasRawTime) {
            var game = (Ext.GetOrNull(parameters, "fs_game") ?? "").ToLowerInvariant();
            var gName = (Ext.GetOrNull(parameters, "gamename") ?? "").ToLowerInvariant();
            var gameversion = (Ext.GetOrNull(parameters, "gameversion") ?? "").ToLowerInvariant();

            var names = new Pair("", "");

            if (hasRawTime ||
                game.ToLowerInvariant().StartsWith("defrag") 
                || gName.ToLowerInvariant() == "defrag") {
                return new Pair("defrag", "Defrag");
            }
            if (game == "cpma") {
                return new Pair("cpma", "CPMA");
            }
            if (game == "osp" || gameversion.StartsWith("osp")) {
                return new Pair("osp", "OSP");
            }
            if (game == "arena") {
                return new Pair("ra3", "Rocket Arena");
            }
            if (game == "threewave") {
                return new Pair("q3w", "Threewave CTF");
            }
            if (game == "freeze") {
                return new Pair("q3ft", "Freeze Tag");
            }
            if (game == "ufreeze") {
                return new Pair("q3uft", "Ultra Freeze Tag");
            }
            if (game == "q3ut") {
                return new Pair("q3ut", "Urban Terror");
            }

            var xp_version = (Ext.GetOrNull(parameters, "xp_version") ?? "").ToLowerInvariant();
            if (game == "excessiveplus" || xp_version.StartsWith("xp")) {
                return new Pair("q3xp", "Excessive Plus");
            }
            if (game == "excessive") {
                return new Pair("q3ex", "Excessive");
            }
            if (game == "Reactance:IU") {
                return new Pair("q3insta", "InstaUnlagged");
            }
            if (game == "battle") {
                return new Pair(game, "Battle");
            }
            if (game == "beryllium") {
                return new Pair(game, "Beryllium");
            }
            if (game == "bma") {
                return new Pair(game, "BMA");
            }
            if (game.StartsWith("The CorkScrew Mod")) {
                return new Pair("corkscrew", game);
            }
            if (game == "F4A") {
                return new Pair("f4a", game);
            }
            if (game == "freezeplus") {
                return new Pair("fp", "Freeze Plus");
            }
            if (game == "generations") {
                return new Pair("gen", "Generations");
            }
            if (game == "nemesis") {
                return new Pair("nemesis", "Nemesis");
            }
            if (game == "noghost") {
                return new Pair(game, "NoGhost");
            }
            if (game.StartsWith("pkarena")) {
                return new Pair(game, "Painkeep");
            }
            if (game == "q3f" || game == "q3f2") {
                return new Pair("q3f", "Q3F");
            }
            if (game == "truecombat" || game == "q3tc") {
                return new Pair("q3tc", "True Combat");
            }
            if (game.Contains("unlagged")) {
                return new Pair("unlagged", "Unlagged");
            }
            if (game.Contains("westernq3")) {
                return new Pair("westernq3", "Western Quake 3");
            }
            return new Pair("baseq3", "Quake 3 Arena");
        }

        string getGameplayType() {
            int server_promode = 0;
            switch (gameNameShort) {
                case "defrag":
                    var promode = Ext.GetOrZero(parameters, "df_promode");
                    return promode > 0 ? "CPM" : "VQ3";
                case "cpma":
                    string server_gameplay = Ext.GetOrNull(parameters, "server_gameplay") ?? "";
                    switch (server_gameplay) {
                        case "0": case "VQ3": return "VQ3";
                        case "1": case "PMC": return "PMC";
                        case "2": case "CPM": return "CPM";
                        case "CQ3": return "CQ3";
                    }
                    server_promode = Ext.GetOrZero(parameters, "server_promode");
                    return server_promode > 0 ? "CPM" : "VQ3";
                case "osp":
                    server_promode = Ext.GetOrZero(parameters, "server_promode");
                    return server_promode > 0 ? "CPM" : "VQ3";
            }
            return "";
        }


        Pair getGameType() {
            int g_gametype = Ext.GetOrZero(parameters, "g_gametype");
            switch (gameNameShort) {
                case "defrag":
                    int dfGType = Ext.GetOrZero(parameters, "defrag_gametype");
                    switch (dfGType) {
                        case 1: return new Pair("df", "Defrag");
                        case 2: return new Pair("fs", "Freestyle");
                        case 3: return new Pair("fc", "Fast Caps");

                        case 5: return new Pair("mdf", "Multiplayer Defrag");
                        case 6: return new Pair("mfs", "Multiplayer Freestyle");
                        case 7: return new Pair("mfc", "Multiplayer Fast Caps");
                    }
                    break;
                case "cpma":
                    switch (g_gametype) {
                        case 5: return new Pair("ca", "Clan Arena");
                        case 6: return new Pair("fr", "Freeze");
                        case 7: return new Pair("ctfs", "CTFS");
                        case 8: return new Pair("ntf", "NTF");
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
                    string g_serverData = Ext.GetOrNull(parameters, "g_serverData") ?? "";

                    if (g_serverData.Contains("G00")) return new Pair("ffa", "Free for All");
                    if (g_serverData.Contains("G01")) return new Pair("1v1", "Duel");
                    if (g_serverData.Contains("G03")) return new Pair("tdm", "Team Deathmatch");
                    if (g_serverData.Contains("G04")) return new Pair("ctf", "Capture the Flag");
                    if (g_serverData.Contains("G05")) return new Pair("ofc", "One Flag CTF");
                    if (g_serverData.Contains("G09")) return new Pair("cst", "Capturestrike");
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
