using System;
using System.Collections.Generic;
using System.Text;

namespace DemoCleaner3.DemoParser.structures
{
    /**
    * re-worked from q_shared.h, typedef struct entityState_s
    */
    public class EntityState
    {
        public int number;                  // entity index
        public int eType;                   // entityType_t
        public int eFlags;

        public Trajectory pos = new Trajectory();  // for calculating position
        public Trajectory apos = new Trajectory(); // for calculating angles

        public long time;
        public long time2;

        public float[] origin = new float[3];
        public float[] origin2 = new float[3];
        public float[] angles = new float[3];
        public float[] angles2 = new float[3];

        public int otherEntityNum;          // shotgun sources, etc
        public int otherEntityNum2;

        public int groundEntityNum;         // -1 = in air

        public long constantLight;          // r + (g<<8) + (b<<16) + (intensity<<24)
        public int loopSound;               // constantly loop this sound

        public int modelindex;
        public int modelindex2;
        public int clientNum;               // 0 to (MAX_CLIENTS - 1), for players and corpses
        public int frame;

        public int solid;                   // for client side prediction, trap_linkentity sets this properly

        public int events;                  // impulse events -- muzzle flashes, footsteps, etc
        public int eventParm;

        // for players
        public int powerups;                // bit flags
        public int weapon;                  // determines weapon and flash model, etc
        public int legsAnim;                // mask off ANIM_TOGGLEBIT
        public int torsoAnim;               // mask off ANIM_TOGGLEBIT

        public int generic1;

        public void copy(EntityState x)
        {
            this.number = x.number;
            this.eType = x.eType;
            this.eFlags = x.eFlags;
            this.pos = new Trajectory(x.pos);
            this.apos = new Trajectory(x.apos);
            this.time = x.time;
            this.time2 = x.time2;
            Array.Copy(x.origin, origin, 3);
            Array.Copy(x.origin2, origin2, 3);
            Array.Copy(x.angles, angles, 3);
            Array.Copy(x.angles2, angles2, 3);
            this.otherEntityNum = x.otherEntityNum;
            this.otherEntityNum2 = x.otherEntityNum2;
            this.groundEntityNum = x.groundEntityNum;
            this.constantLight = x.constantLight;
            this.loopSound = x.loopSound;
            this.modelindex = x.modelindex;
            this.modelindex2 = x.modelindex2;
            this.clientNum = x.clientNum;
            this.frame = x.frame;
            this.solid = x.solid;
            this.events = x.events;
            this.eventParm = x.eventParm;
            this.powerups = x.powerups;
            this.weapon = x.weapon;
            this.legsAnim = x.legsAnim;
            this.torsoAnim = x.torsoAnim;
            this.generic1 = x.generic1;
        }
    }

}
