using System;
using System.Collections.Generic;
using System.Text;

namespace DemoRenamer.DemoParser
{
    class Q3_SVC
    {
        public const byte BAD = 0;  // not used in demos
        public const byte NOP = 1;  // not used in demos
        public const byte GAMESTATE = 2;
        public const byte CONFIGSTRING = 3; // only inside gamestate
        public const byte BASELINE = 4;     // only inside gamestate
        public const byte SERVERCOMMAND = 5;
        public const byte DOWNLOAD = 6; // not used in demos
        public const byte SNAPSHOT = 7;
        public const byte EOF = 8;
    }
}
