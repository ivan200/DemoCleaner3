using DemoCleaner3.DemoParser.parser;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DemoCleaner3.structures
{
    public class DemoNames
    {
        public string dfName = null;        //name in the game in the demo names
        public string uName = null;         //name in the game
        public string demoUserName = null;  //name from the filename

        Dictionary<string, string> playerInfo;

        public DemoNames(Dictionary<string, string> playerInfo, string bracketsName) {
            //names
            if (playerInfo!= null) {
                dfName = Ext.GetOrNull(playerInfo, "df_name");
                uName = Ext.GetOrNull(playerInfo, "name");
                uName = RawInfo.removeColors(uName);
                uName = RawInfo.normalizeName(uName);
            }
            demoUserName = bracketsName;
        }

        public DemoNames(string onlineName, string bracketsName) {
            //names
            uName = onlineName;
            uName = RawInfo.removeColors(uName);
            uName = RawInfo.normalizeName(uName);
            demoUserName = bracketsName;
        }

        public string normalName {
            get {
                var name = chooseName(dfName, uName, demoUserName);
                if(name == "UnnamedPlayer") {
                    name = chooseName(uName, dfName, demoUserName);
                }
                return name;
            }
        }

        //selection of the first non-empty string from parameters
        private static string chooseName(params string[] names) {
            for (int i = 0; i < names.Length - 1; i++) {
                if (!string.IsNullOrEmpty(names[i])) {
                    return names[i];
                }
            }
            return names[names.Length - 1];
        }

    }
}
