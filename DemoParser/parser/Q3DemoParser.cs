using DemoCleaner3.DemoParser.utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DemoCleaner3.DemoParser.parser
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

        public RawInfo parseConfig()
        {
            var msgParser = new Q3DemoConfigParser();
            this.doParse(msgParser);
            RawInfo info = new RawInfo(
                file_name,msgParser.getRawConfigs(), 
                msgParser.dateStamp, 
                msgParser.performedTimes, 
                msgParser.onlineTimes.Keys.ToList());
            return info;
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

        public static RawInfo getRawConfigStrings(string file_name)
        {
            Q3DemoParser p = new Q3DemoParser(file_name);
            return p.parseConfig();
        }

        public static int countDemoMessages(string file_name)
        {
            Q3DemoParser p = new Q3DemoParser(file_name);
            return p.countMessages();
        }
    }
}
