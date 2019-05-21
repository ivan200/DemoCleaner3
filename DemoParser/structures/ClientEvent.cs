using System;
using System.Collections.Generic;
using System.Text;

namespace DemoCleaner3.DemoParser.structures
{
    class ClientEvent
    {
        public enum EventType {
            None,
            DemoStart,
            Start,
            TimeReset,
            Finish,
            Something,
            ChangePmType, //normal, noclip, spectator, death
            ChangeUser
        }

        public EventType ev;
        public long time;
        public long serverTime;
        public int playerNum;
        public int playerMode;
        //PM_NORMAL,        // can accelerate and turn
        //PM_NOCLIP,        // noclip movement
        //PM_SPECTATOR,    // still run into walls
        //PM_DEAD,        // no acceleration or turning, but free falling

        public int userStat;
        //stats[12]
        //start - xor 2
        //tr - xor 4
        //finish - xor 8

        public ClientEvent(EventType ev, long time, CLSnapshot snapshot) {
            this.ev = ev;
            this.time = time;
            this.serverTime = snapshot.serverTime;
            this.playerNum = snapshot.ps.clientNum;
            this.userStat = snapshot.ps.stats[12];
            this.playerMode = snapshot.ps.pm_type;
        }
    }
}
