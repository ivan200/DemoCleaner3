using System;
using System.Collections.Generic;
using System.Text;

namespace DemoCleaner3.DemoParser.structures
{
    public class ClientConnection
    {
        public long clientNum;
        public int connectPacketCount;                     // for display on connection dialog

        public long checksumFeed;                           // from the server for checksum calculations

        // these are our reliable messages that go to the server
        public int reliableSequence;
        public int reliableAcknowledge;            // the last one the server has executed
                                                   //    public String reliableCommands[] = new String[Const.MAX_RELIABLE_COMMANDS];

        // server message (unreliable) and command (reliable) sequence
        // numbers are NOT cleared at level changes, but continue to
        // increase as long as the connection is valid

        // message sequence is used by both the network layer and the
        // delta compression layer
        public int serverMessageSequence;

        // reliable messages received from server
        public int serverCommandSequence;
        public int lastExecutedServerCommand;              // last server command grabbed or executed with CL_GetServerCommand

        public Dictionary<long, KeyValuePair<long, string>> console = new Dictionary<long, KeyValuePair<long, string>>();
        public Dictionary<short, string> configs = new Dictionary<short, string>();
        public Dictionary<string, string> errors = new Dictionary<string, string>();

        public Dictionary<long, EntityState> entityBaselines = new Dictionary<long, EntityState>();

        public bool demowaiting;    // don't record until a non-delta message is received
    }
}
