using System;
using System.Collections.Generic;
using System.Text;

namespace DemoCleaner3.DemoParser.structures
{
    class MapperFactory
    {
        public const int EntityStateFieldNum = 51;
        public const int PlayerStateFieldNum = 48;

        public static void updateEntityState(EntityState state, int number, Q3HuffmanReader reader, bool reset) {
            switch (number) {
                case 0:  state.pos.trTime      = reset ? 0 : reader.readLong();           break;
                case 1:  state.pos.trBase[0]   = reset ? 0 : reader.readFloatIntegral();  break;
                case 2:  state.pos.trBase[1]   = reset ? 0 : reader.readFloatIntegral();  break;
                case 3:  state.pos.trDelta[0]  = reset ? 0 : reader.readFloatIntegral();  break;
                case 4:  state.pos.trDelta[1]  = reset ? 0 : reader.readFloatIntegral();  break;
                case 5:  state.pos.trBase[2]   = reset ? 0 : reader.readFloatIntegral();  break;
                case 6:  state.apos.trBase[1]  = reset ? 0 : reader.readFloatIntegral();  break;
                case 7:  state.pos.trDelta[2]  = reset ? 0 : reader.readFloatIntegral();  break;
                case 8:  state.apos.trBase[0]  = reset ? 0 : reader.readFloatIntegral();  break;
                case 9:  state.events          = reset ? 0 : (int)reader.readNumBits(10); break;
                case 10: state.angles2[1]      = reset ? 0 : reader.readFloatIntegral();  break;
                case 11: state.eType           = reset ? 0 : (int)reader.readNumBits(8);  break;
                case 12: state.torsoAnim       = reset ? 0 : (int)reader.readNumBits(8);  break;
                case 13: state.eventParm       = reset ? 0 : (int)reader.readNumBits(8);  break;
                case 14: state.legsAnim        = reset ? 0 : (int)reader.readNumBits(8);  break;
                case 15: state.groundEntityNum = reset ? 0 : (int)reader.readNumBits(10); break;
                case 16: state.pos.trType      = reset ? 0 : (TrType) reader.readByte();  break;
                case 17: state.eFlags          = reset ? 0 : (int)reader.readNumBits(19); break;
                case 18: state.otherEntityNum  = reset ? 0 : (int)reader.readNumBits(10); break;
                case 19: state.weapon          = reset ? 0 : (int)reader.readNumBits(8);  break;
                case 20: state.clientNum       = reset ? 0 : (int)reader.readNumBits(8);  break;
                case 21: state.angles[1]       = reset ? 0 : reader.readFloatIntegral();  break;
                case 22: state.pos.trDuration  = reset ? 0 : reader.readLong();           break;
                case 23: state.apos.trType     = reset ? 0 : (TrType)reader.readByte();   break;
                case 24: state.origin[0]       = reset ? 0 : reader.readFloatIntegral();  break;
                case 25: state.origin[1]       = reset ? 0 : reader.readFloatIntegral();  break;
                case 26: state.origin[2]       = reset ? 0 : reader.readFloatIntegral();  break;
                case 27: state.solid           = reset ? 0 : (int)reader.readNumBits(24); break;
                case 28: state.powerups        = reset ? 0 : (int)reader.readNumBits(16); break;
                case 29: state.modelindex      = reset ? 0 : (int)reader.readNumBits(8);  break;
                case 30: state.otherEntityNum2 = reset ? 0 : (int)reader.readNumBits(10); break;
                case 31: state.loopSound       = reset ? 0 : (int)reader.readNumBits(8);  break;
                case 32: state.generic1        = reset ? 0 : (int)reader.readNumBits(8);  break;
                case 33: state.origin2[2]      = reset ? 0 : reader.readFloatIntegral();  break;
                case 34: state.origin2[0]      = reset ? 0 : reader.readFloatIntegral();  break;
                case 35: state.origin2[1]      = reset ? 0 : reader.readFloatIntegral();  break;
                case 36: state.modelindex2     = reset ? 0 : (int)reader.readNumBits(8);  break;
                case 37: state.angles[0]       = reset ? 0 : reader.readFloatIntegral();  break;
                case 38: state.time            = reset ? 0 : reader.readLong();           break;
                case 39: state.apos.trTime     = reset ? 0 : reader.readLong();           break;
                case 40: state.apos.trDuration = reset ? 0 : reader.readLong();           break;
                case 41: state.apos.trBase[2]  = reset ? 0 : reader.readFloatIntegral();  break;
                case 42: state.apos.trDelta[0] = reset ? 0 : reader.readFloatIntegral();  break;
                case 43: state.apos.trDelta[1] = reset ? 0 : reader.readFloatIntegral();  break;
                case 44: state.apos.trDelta[2] = reset ? 0 : reader.readFloatIntegral();  break;
                case 45: state.time2           = reset ? 0 : reader.readLong();           break;
                case 46: state.angles[2]       = reset ? 0 : reader.readFloatIntegral();  break;
                case 47: state.angles2[0]      = reset ? 0 : reader.readFloatIntegral();  break;
                case 48: state.angles2[2]      = reset ? 0 : reader.readFloatIntegral();  break;
                case 49: state.constantLight   = reset ? 0 : reader.readLong();           break;
                case 50: state.frame           = reset ? 0 : (int)reader.readNumBits(16); break;
            }
        }

        public static void updatePlayerState(PlayerState state, int number, Q3HuffmanReader reader, bool reset)
        {
            switch (number) {
                case 0: state.commandTime        = reset ? 0 : reader.readLong();            break;
                case 1: state.origin[0]          = reset ? 0 : reader.readFloatIntegral();   break;
                case 2: state.origin[1]          = reset ? 0 : reader.readFloatIntegral();   break;
                case 3: state.bobCycle           = reset ? 0 : (int)reader.readNumBits(8);   break;
                case 4: state.velocity[0]        = reset ? 0 : reader.readFloatIntegral();   break;
                case 5: state.velocity[1]        = reset ? 0 : reader.readFloatIntegral();   break;
                case 6: state.viewangles[1]      = reset ? 0 : reader.readFloatIntegral();   break;
                case 7: state.viewangles[0]      = reset ? 0 : reader.readFloatIntegral();   break;
                case 8: state.weaponTime         = reset ? 0 : (int)reader.readNumBits(-16); break;
                case 9: state.origin[2]          = reset ? 0 : reader.readFloatIntegral();   break;
                case 10: state.velocity[2]       = reset ? 0 : reader.readFloatIntegral();   break;
                case 11: state.legsTimer         = reset ? 0 : (int)reader.readNumBits(8);   break;
                case 12: state.pm_time           = reset ? 0 : (int)reader.readNumBits(-16); break;
                case 13: state.eventSequence     = reset ? 0 : (int)reader.readNumBits(16);  break;
                case 14: state.torsoAnim         = reset ? 0 : (int)reader.readNumBits(8);   break;
                case 15: state.movementDir       = reset ? 0 : (int)reader.readNumBits(4);   break;
                case 16: state.events[0]         = reset ? 0 : (int)reader.readNumBits(8);   break;
                case 17: state.legsAnim          = reset ? 0 : (int)reader.readNumBits(8);   break;
                case 18: state.events[1]         = reset ? 0 : (int)reader.readNumBits(8);   break;
                case 19: state.pm_flags          = reset ? 0 : (int)reader.readNumBits(16);  break;
                case 20: state.groundEntityNum   = reset ? 0 : (int)reader.readNumBits(10);  break;
                case 21: state.weaponstate       = reset ? 0 : (int)reader.readNumBits(4);   break;
                case 22: state.eFlags            = reset ? 0 : (int)reader.readNumBits(16);  break;
                case 23: state.externalEvent     = reset ? 0 : (int)reader.readNumBits(10);  break;
                case 24: state.gravity           = reset ? 0 : (int)reader.readNumBits(16);  break;
                case 25: state.speed             = reset ? 0 : (int)reader.readNumBits(16);  break;
                case 26: state.delta_angles[1]   = reset ? 0 : (int)reader.readNumBits(16);  break;
                case 27: state.externalEventParm = reset ? 0 : (int)reader.readNumBits(8);   break;
                case 28: state.viewheight        = reset ? 0 : (int)reader.readNumBits(-8);  break;
                case 29: state.damageEvent       = reset ? 0 : (int)reader.readNumBits(8);   break;
                case 30: state.damageYaw         = reset ? 0 : (int)reader.readNumBits(8);   break;
                case 31: state.damagePitch       = reset ? 0 : (int)reader.readNumBits(8);   break;
                case 32: state.damageCount       = reset ? 0 : (int)reader.readNumBits(8);   break;
                case 33: state.generic1          = reset ? 0 : (int)reader.readNumBits(8);   break;
                case 34: state.pm_type           = reset ? 0 : (int)reader.readNumBits(8);   break;
                case 35: state.delta_angles[0]   = reset ? 0 : (int)reader.readNumBits(16);  break;
                case 36: state.delta_angles[2]   = reset ? 0 : (int)reader.readNumBits(16);  break;
                case 37: state.torsoTimer        = reset ? 0 : (int)reader.readNumBits(12);  break;
                case 38: state.eventParms[0]     = reset ? 0 : (int)reader.readNumBits(8);   break;
                case 39: state.eventParms[1]     = reset ? 0 : (int)reader.readNumBits(8);   break;
                case 40: state.clientNum         = reset ? 0 : (int)reader.readNumBits(8);   break;
                case 41: state.weapon            = reset ? 0 : (int)reader.readNumBits(5);   break;
                case 42: state.viewangles[2]     = reset ? 0 : reader.readFloatIntegral();   break;
                case 43: state.grapplePoint[0]   = reset ? 0 : reader.readFloatIntegral();   break;
                case 44: state.grapplePoint[1]   = reset ? 0 : reader.readFloatIntegral();   break;
                case 45: state.grapplePoint[2]   = reset ? 0 : reader.readFloatIntegral();   break;
                case 46: state.jumppad_ent       = reset ? 0 : (int)reader.readNumBits(10);  break;
                case 47: state.loopSound         = reset ? 0 : (int)reader.readNumBits(16);  break;
            }
        }
    }
}
