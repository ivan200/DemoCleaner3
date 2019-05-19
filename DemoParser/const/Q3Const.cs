using System;
using System.Collections.Generic;
using System.Text;

namespace DemoCleaner3.DemoParser
{
    class Q3Const
    {
        public const int MAX_CONFIGSTRINGS = 1024;
        public const int Q3_DEMO_CFG_FIELD_CLIENT = 0;
        public const int Q3_DEMO_CFG_FIELD_GAME = 1;
        public const int Q3_DEMO_CFG_FIELD_MAP = 3;
        public const int Q3_DEMO_CFG_FIELD_PLAYER = 544;


        public const int GENTITYNUM_BITS = 10;
        public const int MAX_GENTITIES = 1 << GENTITYNUM_BITS;

        public const int FLOAT_INT_BITS = 13;
        public const int FLOAT_INT_BIAS = (1 << (FLOAT_INT_BITS - 1));

        public const int PACKET_BACKUP = 32;
        public const int PACKET_MASK = PACKET_BACKUP - 1;

        public const int MAX_RELIABLE_COMMANDS = 64;

        // q_shared.h
        public const int MAX_POWERUPS = 16;
        public const int MAX_WEAPONS = 16;
        public const int MAX_STATS = 16;
        public const int MAX_PERSISTANT = 16;
        public const int PS_PMOVEFRAMECOUNTBITS = 6;
        public const int MAX_PS_EVENTS = 2;
        public const int MAX_MAP_AREA_BYTES = 16;

        // cg_public.h
        public const int CMD_BACKUP = 64;
        public const int CMD_MASK = CMD_BACKUP - 1;


        // client.h
        public const int MAX_PARSE_ENTITIES = 2048;
        // '%'
        public const byte PERCENT_CHAR_BYTE = 37;
        public const byte DOT_CHAR_BYTE = 46;
    }
}
