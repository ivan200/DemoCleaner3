using System;
using System.Collections.Generic;
using System.Text;

namespace DemoCleaner3.DemoParser.parser
{
    class Q3DemoConfigParser : AbstractDemoMessageParser
    {
        public List<string> onlineTimes = new List<string>();
        public List<string> performedTimes = new List<string>();
        public List<string> oldOfflineTimes = new List<string>();
        public List<string> oldOfflineTimes2 = new List<string>();
        public List<string> dateStamp = new List<string>();

        private Dictionary<short, string> configs = null;

        public Dictionary<short, string> getRawConfigs()
        {
            return this.configs;
        }

        public bool parse(Q3DemoMessage message)
        {
            Q3HuffmanReader reader = new Q3HuffmanReader(message.data);
            reader.readLong();

            while (!reader.isEOD())
            {
                var b = reader.readByte();
                switch (b)
                {
                    case Q3_SVC.BAD:
                    case Q3_SVC.NOP:
                        return true;
                    case Q3_SVC.EOF:
                        return true;
                    case Q3_SVC.SERVERCOMMAND:
                        this.parseServerCommand(reader);
                        break;
                    case Q3_SVC.GAMESTATE:
                        this.parseGameState(reader);
                        break;
                    case Q3_SVC.SNAPSHOT:
                        // snapshots couldn't be mixed with game-state command in a single message
                        return true;
                    default:
                        // unknown command / corrupted stream
                        return true;
                }
            }
            return true;
        }

        public List<string> console = new List<string>();

        private void parseServerCommand(Q3HuffmanReader reader) {
            var key = reader.readLong();
            var value = reader.readString();
            console.Add(value);
            if (value.StartsWith("print")) {

                //print "Date: 10-25-14 02:43\n"
                if (value.StartsWith("print \"Date:")) {
                    dateStamp.Add(value);
                }

                //print "Time performed by ^2uN-DeaD!Enter^7 : ^331:432^7 (v1.91.23 beta)\n"
                if (value.StartsWith("print \"Time performed by")) {
                    performedTimes.Add(value);
                }

                //"print \"^3Time Performed: 25:912 (defrag 1.5)\n^7\""
                if (value.StartsWith("print \"^3Time Performed:")) {
                    oldOfflineTimes2.Add(value);
                }

                //print \"Rom^7 reached the finish line in ^23:38:208^7\n\"
                if (value.Contains("reached the finish line in")) {
                    onlineTimes.Add(value);
                }
            } else if (value.StartsWith("NewTime")) {
                //"NewTime -971299442 7:200 \"defrag 1.80\" \"Viper\" route ya->->rg"
                oldOfflineTimes.Add(value);
            }
        }


        private void parseGameState(Q3HuffmanReader reader)
        {
            reader.readLong();

            while (true)
            {
                byte cmd = reader.readByte();
                if (cmd == Q3_SVC.EOF)
                    break;

                switch (cmd)
                {
                    case Q3_SVC.CONFIGSTRING:
                        short key = reader.readShort();
                        if (key < 0 || key > Q3Const.MAX_CONFIGSTRINGS)
                        {
                            return;
                        }
                        if (this.configs == null)
                            this.configs = new Dictionary<short, string>();

                        this.configs[key] = reader.readBigString();
                        break;

                    case Q3_SVC.BASELINE:
                        // assume Baseline command has to follow after config-strings
                        return;

                    default:
                        //  bad command
                        return;
                }
            }

            //clc.clientNum
            reader.readLong();

            //clc.checksumFeed
            reader.readLong();
        }
    }
}
