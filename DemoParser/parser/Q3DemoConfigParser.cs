using DemoCleaner3.DemoParser.structures;
using System;
using System.Collections.Generic;
using System.Text;

namespace DemoCleaner3.DemoParser.parser
{
    class Q3DemoConfigParser : AbstractDemoMessageParser
    {
        public Dictionary<long, string> console = new Dictionary<long, string>();
        public Dictionary<short, string> configs = new Dictionary<short, string>();

        public EntityState [] entityBaselines = new EntityState[Q3Const.MAX_GENTITIES];

        long clientNum = 0;
        long checksumFeed = 0;

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

        private void parseServerCommand(Q3HuffmanReader reader) {
            var key = reader.readLong();
            var value = reader.readString();

            console[key] = value;
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
                        this.configs[key] = reader.readBigString();
                        break;

                    case Q3_SVC.BASELINE:
                        long newnum = reader.readNumBits(Q3Const.GENTITYNUM_BITS);
                        if (newnum < 0 || newnum >= Q3Const.MAX_GENTITIES) {
                            Console.WriteLine("Baseline number out of range: {}", newnum);
                            return;
                        }

                        EntityState es = entityBaselines[newnum];
                        if (es == null) {
                            es = new EntityState();
                            entityBaselines[newnum] = es;
                        }

                        if (!reader.readDeltaEntity(es, (int)newnum)) {
                            Console.WriteLine("unable to parse delta-entity state");
                            return;
                        }
                        break;

                    default:
                        //  bad command
                        return;
                }
            }

            //clc.clientNum
            clientNum = reader.readLong();

            //clc.checksumFeed
            checksumFeed = reader.readLong();
        }
    }
}
