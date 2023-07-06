﻿using System;
using System.Collections.Generic;
using System.Text;

namespace DemoCleaner3.DemoParser.structures
{
    public class ClientEvent
    {
        public bool eventStartFile = false;
        public bool eventStartTime = false;
        public bool eventTimeReset = false;
        public bool eventFinish = false;
        public bool eventCheckPoint = false;
        public bool eventSomeTrigger = false;
        public bool eventChangePmType = false; //normal, noclip, spectator, death
        public bool eventChangeUser = false;

        public bool hasAnyEvent {
            get {
                return eventStartFile || eventStartTime || eventTimeReset || eventFinish
                    || eventCheckPoint || eventChangePmType || eventChangeUser || eventSomeTrigger;
            }
        }

        public static string[] pmTypesStrings = new string[4] {
            "normal","noclip","spectator","death"
        };

        public enum PlayerMode {
            PM_NORMAL,              // can accelerate and turn
            PM_NOCLIP,              // noclip movement
            PM_SPECTATOR,           // still run into walls
            PM_DEAD                 // no acceleration or turning, but free falling
        }

        public long time = 0;
        public bool timeHasError;
        public long timeByServerTime = 0;
        public long serverTime;
        public int playerNum;
        public int playerMode;
        public int userStat;
        public int speed = 0;

        public long timeNoError {
            get {
                if (timeHasError) {
                    return timeByServerTime;
                } else {
                    return time;
                }
            }
        }

        //stats[12]
        //start - xor 2
        //tr - xor 4
        //finish - xor 8

        public ClientEvent(long time, bool timeHasError, CLSnapshot snapshot) {
            if (!timeHasError) {
                this.time = time;
            }
            this.timeHasError = timeHasError;
            this.serverTime = snapshot.serverTime;
            this.playerNum = snapshot.ps.clientNum;
            this.userStat = snapshot.ps.stats[12];
            this.playerMode = snapshot.ps.pm_type;
        }

    }
}
