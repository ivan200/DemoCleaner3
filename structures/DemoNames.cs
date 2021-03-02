using DemoCleaner3.DemoParser.parser;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace DemoCleaner3.structures
{
    public class DemoNames
    {
        public static string defaultName = "UnnamedPlayer";

        public string dfName = null;        //name in params - df_name
        public string uName = null;         //name in the game
        public string oName = null;         //name from console line - online name
        public string cName = null;         //name from console line - offline name, was used before df_name was added
        public string fName = null;         //name from the filename

        public void setNamesByPlayerInfo(Dictionary<string, string> playerInfo) {
            if (playerInfo != null) {
                dfName = Ext.GetOrNull(playerInfo, "df_name");
                uName = normalizeName(RawInfo.removeColors(Ext.GetOrNull(playerInfo, "name")));
            }
        }

        public void setConsoleName(string onlineName, bool isOnline) {
            if (isOnline) {
                oName = normalizeName(RawInfo.removeColors(onlineName));
            } else {
                cName = normalizeName(RawInfo.removeColors(onlineName));
            }
        }

        public void setBracketsName(string bracketsName) {
            fName = bracketsName;
        }

        public string chooseNormalName() {
            return chooseName(dfName, cName, uName, oName, fName); 
        }

        //selection of the first non-empty string from parameters
        public static string chooseName(params string[] names) {
            var validNames = names.ToList().Where(x => !string.IsNullOrEmpty(x) && x != defaultName);
            return validNames.FirstOrDefault() ?? defaultName;
        }

        //name that can be used in the file name
        public static string normalizeName(string name) {
            return string.IsNullOrEmpty(name)
                ? name
                : Regex.Replace(name, "[^a-zA-Z0-9\\!\\#\\$\\%\\&\\'\\(\\)\\+\\,\\-\\.\\;\\=\\[\\]\\^_\\{\\}]", "");
        }
    }
}
