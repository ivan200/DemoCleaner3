using System;
using System.Collections.Generic;
using System.Text;

namespace DemoCleaner3.DemoParser.structures
{
    public class ClientState
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

        public List<ClientEvent> clientEvents = new List<ClientEvent>();
        public ClientEvent lastClientEvent;

        public Dictionary<string, string> clientConfig = null; //this is only client config, not all configs
        public Dictionary<string, string> gameConfig = null;

        public string mapname;
        public int mapNameChecksum = 0;
        public int dfvers = 0;
        public bool isOnline = false;
        public bool isCheatsOn = false;
        public int maxSpeed = 0;
        public bool? isCpmInParams = null;
        public bool? isCpmInSnapshots = null;
    }
}
