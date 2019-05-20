using System;
using System.Collections.Generic;
using System.Text;

namespace DemoCleaner3.DemoParser.structures
{
    class ClientState
    {
        // latest received from server
        public CLSnapshot snap = new CLSnapshot();
        public bool newSnapshots;           // set on parse of any valid packet

        public Dictionary<int, string> gameState = new Dictionary<int, string>(); // configstrings

        public int parseEntitiesNum;       // index (not anded off) into cl_parse_entities[]

        // can tell if it is for a prior map_restart
        // big stuff at end of structure so most offsets are 15 bits or less
        public Dictionary<int, CLSnapshot> snapshots = new Dictionary<int, CLSnapshot>();
        public Dictionary<int, EntityState> entityBaselines = new Dictionary<int, EntityState>(); // for delta compression when not in previous frame
        public Dictionary<int, EntityState> parseEntities = new Dictionary<int, EntityState>();


        public List<long> times1 = new List<long>();
        public List<TimeSpan> times2 = new List<TimeSpan>();
        public Dictionary<string, string> clientConfig = null;
    }
}
