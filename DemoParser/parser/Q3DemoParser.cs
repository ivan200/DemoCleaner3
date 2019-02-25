using DemoRenamer.DemoParser;
using DemoRenamer.DemoParser.parser;
using DemoRenamer.DemoParser.utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace DemoRenamer
{
    class Q3DemoParser
    {
        private string file_name;

        /**
        * Q3DemoParser constructor.
        * @param string file_name - name of demo-file
        */
        public Q3DemoParser(string file_name) {
            this.file_name = file_name;
        }

        public Dictionary<short, string> parseConfig()
        {
            var msgParser = new Q3DemoConfigParser();
            this.doParse(msgParser);
            return msgParser.hasConfigs() ? msgParser.getRawConfigs() : null;
        }

        /**
        *
        * @throws Exception
        * @return int messages count in this demo-file
        */
        public int countMessages()
        {
            return ((Q3EmptyParser)this.doParse(new Q3EmptyParser())).count;
        }

        private AbstractDemoMessageParser doParse(AbstractDemoMessageParser msgParser)
        {
            Q3MessageStream messageStream = new Q3MessageStream(this.file_name);
            try
            {
                Q3DemoMessage msg = null;
                while ((msg = messageStream.nextMessage()) != null)
                {
                    if (!msgParser.parse(msg))
                    {
                        break;
                    }
                }
            }
            catch (Exception r) {
                String s = r.Message;
            }
            messageStream.close();

            return msgParser;
        }

        public static Dictionary<short, string> getRawConfigStrings(string file_name)
        {
            Q3DemoParser p = new Q3DemoParser(file_name);
            return p.parseConfig();
        }

        public static Dictionary<string, Dictionary<string, string>> getFriendlyConfig(string file_name)
        {
            Dictionary<short, string> conf = getRawConfigStrings(file_name);

            if (conf == null)
                return null;

            Dictionary<string, Dictionary<string, string>> result = new Dictionary<string, Dictionary<string, string>>();


            foreach (var item in conf)
            {
                if (item.Value.IndexOf('\\') >= 0)
                {
                    result[item.Key.ToString()] = Q3Utils.split_config(item.Value);
                }
            }


            //if (conf.ContainsKey(Q3Const.Q3_DEMO_CFG_FIELD_CLIENT))
            //{
            //    result["client"] = Q3Utils.split_config(conf[Q3Const.Q3_DEMO_CFG_FIELD_CLIENT]);

            //    //result["client_version"] = result["client"]["version"];
            //    //result["physic"] = result['client']['df_promode'] == 0 ? 'vq3' : 'cpm';
            //}

            //if (conf.ContainsKey(Q3Const.Q3_DEMO_CFG_FIELD_GAME))
            //{
            //    result["game"] = Q3Utils.split_config(conf[Q3Const.Q3_DEMO_CFG_FIELD_GAME]);
            //}


            //if (conf.ContainsKey(Q3Const.Q3_DEMO_CFG_FIELD_PLAYER))
            //{
            //    result["player"] = Q3Utils.split_config(conf[Q3Const.Q3_DEMO_CFG_FIELD_PLAYER]);
            //}
            //else {
            //    if (conf.ContainsKey(Q3Const.Q3_DEMO_CFG_FIELD_PLAYER2))
            //    {
            //        result["player"] = Q3Utils.split_config(conf[Q3Const.Q3_DEMO_CFG_FIELD_PLAYER2]);
            //    }
            //}

            Dictionary<string, string> raw = new Dictionary<string, string>();
            foreach (var r in conf) {
                raw.Add(r.Key.ToString(), r.Value);
            }

            result["raw"] = raw;

            return result;
        }

        public static int countDemoMessages(string file_name)
        {
            Q3DemoParser p = new Q3DemoParser(file_name);
            return p.countMessages();
        }
    }
}
