using System;
using System.Collections.Generic;
using System.Text;

namespace DemoRenamer.DemoParser
{
    class Q3DemoConfigParser : AbstractDemoMessageParser
    {

        private Dictionary<short, string> configs = null;

        public bool hasConfigs()
        {
            return this.configs != null;
        }

        public Dictionary<short, string> getRawConfigs()
        {
            return this.configs;
        }

        public bool parse(Q3DemoMessage message)
        {
            Q3HuffmanReader reader = new Q3HuffmanReader(message.data);
            var lo = reader.readLong();

            while (!reader.isEOD())
            {
                var b = reader.readByte();
                switch (b)
                {
                    case Q3_SVC.BAD:
                    case Q3_SVC.NOP:
                        return false;

                    case Q3_SVC.EOF:
                        return this.configs != null;

                    case Q3_SVC.SERVERCOMMAND:
                        reader.readServerCommand();
                        break;

                    case Q3_SVC.GAMESTATE:
                        this.parseGameState(reader);
                        return this.configs != null;

                    case Q3_SVC.SNAPSHOT:
                        // snapshots couldn't be mixed with game-state command in a single message
                        return false;

                    default:
                        // unknown command / corrupted stream
                        return false;
                }
            }
            return false;
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
