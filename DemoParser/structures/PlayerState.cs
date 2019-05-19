using System;
using System.Collections.Generic;
using System.Text;

namespace DemoCleaner3.DemoParser.structures
{
    public class PlayerState
    {
        public long commandTime;                 // cmd->serverTime of last executed command
        public int pm_type;
        public int bobCycle;                    // for view bobbing and footstep generation
        public int pm_flags;                    // ducked, jump_held, etc
        public int pm_time;

        public float[] origin = new float[3];
        public float[] velocity = new float[3];
        public int weaponTime;
        public int gravity;
        public int speed;
        public int[] delta_angles = new int[3]; // add to command angles to get view direction
                                                // changed by spawns, rotating objects, and teleporters

        public int groundEntityNum;             // ENTITYNUM_NONE = in air

        public int legsTimer;                   // don't change low priority animations until this runs out
        public int legsAnim;                    // mask off ANIM_TOGGLEBIT

        public int torsoTimer;                  // don't change low priority animations until this runs out
        public int torsoAnim;                   // mask off ANIM_TOGGLEBIT

        public int movementDir;                 // a number 0 to 7 that represents the reletive angle
                                                // of movement to the view angle (axial and diagonals)
                                                // when at rest, the value will remain unchanged
                                                // used to twist the legs during strafing

        public float[] grapplePoint = new float[3];             // location of grapple to pull towards if PMF_GRAPPLE_PULL

        public int eFlags;                      // copied to entityState_t->eFlags

        public int eventSequence;               // pmove generated events
        public int[] events = new int[Q3Const.MAX_PS_EVENTS];
        public int[] eventParms = new int[Q3Const.MAX_PS_EVENTS];

        public int externalEvent;               // events set on player from another source
        public int externalEventParm;
        public int externalEventTime;

        public int clientNum;                   // ranges from 0 to MAX_CLIENTS-1
        public int weapon;                      // copied to entityState_t->weapon
        public int weaponstate;

        public float[] viewangles = new float[3];  // for fixed views
        public int viewheight;

        // damage feedback
        public int damageEvent;                 // when it changes, latch the other parms
        public int damageYaw;
        public int damagePitch;
        public int damageCount;

        public int[] stats = new int[Q3Const.MAX_STATS];
        public int[] persistant = new int[Q3Const.MAX_PERSISTANT];  // stats that aren't cleared on death
        public int[] powerups = new int[Q3Const.MAX_POWERUPS];      // level.time that the powerup runs out
        public int[] ammo = new int[Q3Const.MAX_WEAPONS];

        public int generic1;
        public int loopSound;
        public int jumppad_ent;                 // jumppad entity hit this frame

        // not communicated over the net at all
        public int ping;                        // server to game info for scoreboard
        public int pmove_framecount;            // FIXME: don't transmit over the network
        public int jumppad_frame;
        public int entityEventSequence;
    }
}
