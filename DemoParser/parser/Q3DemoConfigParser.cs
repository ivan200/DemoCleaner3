using DemoCleaner3.DemoParser.structures;
using DemoCleaner3.DemoParser.utils;
using System;
using System.Collections.Generic;
using System.Text;
using static DemoCleaner3.DemoParser.structures.PlayerState.StatIndex;

namespace DemoCleaner3.DemoParser.parser
{
    class Q3DemoConfigParser
    {
        public ClientConnection clc = new ClientConnection();
        public ClientState client = new ClientState();

        public bool parse(Q3DemoMessage message)
        {
            clc.serverMessageSequence = message.sequence;
            Q3HuffmanReader reader = new Q3HuffmanReader(message.data);
            reader.readLong();

            while (!reader.isEOD()) {
                var b = reader.readByte();
                switch (b) {
                    case Q3_SVC.BAD:
                    case Q3_SVC.NOP:
                        return true;
                    case Q3_SVC.EOF:
                        return true;
                    case Q3_SVC.SERVERCOMMAND:
                        this.parseServerCommand(reader);
                        break;
                    case Q3_SVC.GAMESTATE:
                        this.parseGameState(reader);
                        break;
                    case Q3_SVC.SNAPSHOT:
                        this.parseSnapshot(reader);
                        // snapshots couldn't be mixed with game-state command in a single message
                        break;
                    default:
                        // unknown command / corrupted stream
                        return true;
                }
            }
            return true;
        }

        private void parseServerCommand(Q3HuffmanReader reader) {
            var key = reader.readLong();
            var value = reader.readString();

            clc.console[key] = value;
        }

        private void parseGameState(Q3HuffmanReader reader)
        {
            reader.readLong();

            while (true)
            {
                byte cmd = reader.readByte();
                if (cmd == Q3_SVC.EOF)
                    break;

                switch (cmd)
                {
                    case Q3_SVC.CONFIGSTRING:
                        short key = reader.readShort();
                        if (key < 0 || key > Q3Const.MAX_CONFIGSTRINGS)
                        {
                            return;
                        }
                        clc.configs[key] = reader.readBigString();
                        break;

                    case Q3_SVC.BASELINE:
                        long newnum = reader.readNumBits(Q3Const.GENTITYNUM_BITS);
                        if (newnum < 0 || newnum >= Q3Const.MAX_GENTITIES) {
                            Console.WriteLine("Baseline number out of range: {}", newnum);
                            return;
                        }

                        EntityState es = Ext2<long, EntityState>.GetOrCreate(clc.entityBaselines, newnum);
                        if (!reader.readDeltaEntity(es, (int)newnum)) {
                            Console.WriteLine("unable to parse delta-entity state");
                            return;
                        }
                        break;

                    default:
                        Console.WriteLine("bad command in parseGameState");
                        return;
                }
            }

            //clc.clientNum
            clc.clientNum = reader.readLong();

            //clc.checksumFeed
            clc.checksumFeed = reader.readLong();
        }

        private void parseSnapshot(Q3HuffmanReader decoder)
        {
            if (client.clientConfig == null) {
                client.clientConfig = new Dictionary<string, string>();
                if (clc.configs.ContainsKey(Q3Const.Q3_DEMO_CFG_FIELD_CLIENT)) {
                    client.clientConfig = Q3Utils.split_config(clc.configs[Q3Const.Q3_DEMO_CFG_FIELD_CLIENT]);
                    client.dfvers = Ext.GetOrZero(client.clientConfig, "defrag_vers");
                    client.mapname = Ext.GetOrNull(client.clientConfig, "mapname");
                    client.mapNameChecksum = getMapNameChecksum(client.mapname);
                    client.isOnline = Ext.GetOrZero(client.clientConfig, "defrag_gametype") > 4;
                }
            }

            CLSnapshot newSnap = new CLSnapshot();
            CLSnapshot old = null;

            newSnap.serverCommandNum = clc.serverCommandSequence;
            newSnap.serverTime = decoder.readLong();
            newSnap.messageNum = clc.serverMessageSequence;

            int deltaNum = decoder.readByte();
            if (deltaNum == 0) {
                newSnap.deltaNum = -1;
            } else {
                newSnap.deltaNum = newSnap.messageNum - deltaNum;
            }
            newSnap.snapFlags = decoder.readByte();
            // If the frame is delta compressed from data that we
            // no longer have available, we must suck up the rest of
            // the frame, but not use it, then ask for a non-compressed
            // message
            if (newSnap.deltaNum <= 0) {
                newSnap.valid = true;          // uncompressed frame
                old = null;
                clc.demowaiting = false;       // we can start recording now
            } else {
                old = Ext2<int, CLSnapshot>.GetOrCreate(client.snapshots, newSnap.deltaNum & Q3Const.PACKET_MASK);
                if (old == null || !old.valid) {
                    // should never happen
                    Console.WriteLine("Delta from invalid frame (not supposed to happen!)");
                } else if (old.messageNum != newSnap.deltaNum) {
                    // The frame that the server did the delta from
                    // is too old, so we can't reconstruct it properly.
                    Console.WriteLine("Delta frame too old.");
                } else if ((client.parseEntitiesNum - old.parseEntitiesNum) > (Q3Const.MAX_PARSE_ENTITIES - 128)) {
                    Console.WriteLine("Delta parseEntitiesNum too old");
                } else {
                    newSnap.valid = true;  // valid delta parse
                }
            }

            int len = decoder.readByte();
            if (len > newSnap.areamask.Length) {
                Console.WriteLine("CL_ParseSnapshot: Invalid size {} for areamask", len);
                return;
            }
            decoder.readData(newSnap.areamask, len);
            if(old != null)
            {
                newSnap.ps.copy(old.ps);
            }
            decoder.readDeltaPlayerState(newSnap.ps);
            parsePacketEntities(decoder, old, newSnap);

            // if not valid, dump the entire thing now that it has
            // been properly read
            if (!newSnap.valid) {
                return;
            }

            // clear the valid flags of any snapshots between the last
            // received and this one, so if there was a dropped packet
            // it won't look like something valid to delta from next
            // time we wrap around in the buffer
            int oldMessageNum = client.snap.messageNum + 1;

            if (newSnap.messageNum - oldMessageNum >= Q3Const.PACKET_BACKUP) {
                oldMessageNum = newSnap.messageNum - (Q3Const.PACKET_BACKUP - 1);
            }
            for (; oldMessageNum < newSnap.messageNum; oldMessageNum++) {
                CLSnapshot s;
                if (client.snapshots.TryGetValue(oldMessageNum & Q3Const.PACKET_MASK, out s)) {
                    if (s != null) {
                        s.valid = false;
                    }
                }
            }

            // copy to the current good spot
            client.snap = newSnap;

            // skip ping calculations
            client.snap.ping = 0;

            // save the frame off in the backup array for later delta comparisons
            client.snapshots[client.snap.messageNum & Q3Const.PACKET_MASK] = client.snap;

            client.newSnapshots = true;

            updateClientEvents(newSnap);
        }

        private void updateClientEvents(CLSnapshot snapshot) {
            if (client.dfvers <= 0 || client.mapNameChecksum <= 0) {
                return;
            }
            var time = getTime(snapshot.ps, (int)snapshot.serverTime, client.dfvers, client.mapNameChecksum);
            var events = client.clientEvents;

            ClientEvent clientEvent = new ClientEvent(time, snapshot);
            if (events.Count == 0) {
                clientEvent.eventStartFile = true;
            } else {
                var prevEvent = events[events.Count - 1];
                if (prevEvent.playerNum != snapshot.ps.clientNum) {
                    clientEvent.eventChangeUser = true;
                }
                if (prevEvent.playerMode != snapshot.ps.pm_type) {
                    clientEvent.eventChangePmType = true;
                }
                if (prevEvent.userStat != snapshot.ps.stats[12]) {
                    if ((prevEvent.userStat & 4) != (snapshot.ps.stats[12] & 4)) {
                        if (snapshot.ps.pm_type == (int)ClientEvent.PlayerMode.PM_NORMAL) {
                            if ((prevEvent.userStat & 2) == 0) {
                                clientEvent.eventStartTime = true;
                            } else {
                                clientEvent.eventTimeReset = true;
                            }
                        }
                    } else if ((prevEvent.userStat & 8) == 0 && (snapshot.ps.stats[12] & 8) != 0) {
                        if (!clientEvent.eventChangeUser) {
                            clientEvent.eventFinish = true;
                        }
                    } else {
                        clientEvent.eventCheckPoint = true;
                    }
                }
            }

            if (clientEvent.hasAnyEvent) {
                events.Add(clientEvent);
            }
        }

        private void parsePacketEntities(Q3HuffmanReader decoder, CLSnapshot oldframe, CLSnapshot newframe)
        {
            newframe.parseEntitiesNum = client.parseEntitiesNum;
            newframe.numEntities = 0;
            int newnum = 0;
            int oldindex = 0;
            int oldnum = 0;
            EntityState oldstate = null;

            if (oldframe == null) {
                oldnum = 99999;
            } else {
                if (oldindex >= oldframe.numEntities) {
                    oldnum = 99999;
                } else {
                    oldstate = Ext2<int, EntityState>.GetOrCreate(client.parseEntities, (oldframe.parseEntitiesNum + oldindex) & (Q3Const.MAX_PARSE_ENTITIES - 1));
                    oldnum = oldstate.number;
                }
            }

            while (true) {
                newnum = (int) decoder.readNumBits(Q3Const.GENTITYNUM_BITS);

                if (newnum == (Q3Const.MAX_GENTITIES - 1)) {
                    break;
                }

                if (decoder.isEOD()) {
                    Console.WriteLine("ERR_DROP, CL_ParsePacketEntities: end of message");
                    return;
                }

                while (oldnum < newnum) {
                    // one or more entities from the old packet are unchanged
                    CL_DeltaEntity(decoder, newframe, oldnum, oldstate, true);

                    oldindex++;

                    if (oldindex >= oldframe.numEntities) {
                        oldnum = 99999;
                    } else {
                        oldstate = Ext2<int, EntityState>.GetOrCreate(client.parseEntities, 
                            (oldframe.parseEntitiesNum + oldindex) & (Q3Const.MAX_PARSE_ENTITIES - 1));
                        oldnum = oldstate.number;
                    }
                }

                if (oldnum == newnum) {
                    // delta from previous state
                    CL_DeltaEntity(decoder, newframe, newnum, oldstate, false);

                    oldindex++;

                    if (oldindex >= oldframe.numEntities) {
                        oldnum = 99999;
                    } else {
                        oldstate = Ext2<int, EntityState>.GetOrCreate(client.parseEntities,
                            (oldframe.parseEntitiesNum + oldindex) & (Q3Const.MAX_PARSE_ENTITIES - 1));
                        oldnum = oldstate.number;
                    }
                    continue;
                }

                if (oldnum > newnum) {
                    // delta from baseline
                    EntityState es = Ext2<int, EntityState>.GetOrCreate(client.entityBaselines, newnum);
                    CL_DeltaEntity(decoder, newframe, newnum, es, false);
                    continue;
                }
            }

            // any remaining entities in the old frame are copied over
            while (oldnum != 99999) {
                // one or more entities from the old packet are unchanged
                CL_DeltaEntity(decoder, newframe, oldnum, oldstate, true);

                oldindex++;

                if (oldindex >= oldframe.numEntities) {
                    oldnum = 99999;
                } else {
                    oldstate = Ext2<int, EntityState>.GetOrCreate(client.parseEntities,
                           (oldframe.parseEntitiesNum + oldindex) & (Q3Const.MAX_PARSE_ENTITIES - 1));
                    oldnum = oldstate.number;
                }
            }
        }

        private void CL_DeltaEntity(Q3HuffmanReader decoder, CLSnapshot frame, int newnum, EntityState old, bool unchanged)
        {
            EntityState state;

            // save the parsed entity state into the big circular buffer so
            // it can be used as the source for a later delta

            state = Ext2<int, EntityState>.GetOrCreate(client.parseEntities,
                           client.parseEntitiesNum & (Q3Const.MAX_PARSE_ENTITIES - 1));

            if (unchanged) {
                state.copy(old);
            } else {
                decoder.readDeltaEntity(state, newnum);
            }

            if (state.number == (Q3Const.MAX_GENTITIES - 1)) {
                return;         // entity was delta removed
            }
            client.parseEntitiesNum++;
            frame.numEntities++;
        }


        private int getMapNameChecksum(string mapName)
        {
            if (string.IsNullOrEmpty(mapName)) {
                return 0;
            }
            mapName = mapName.ToLowerInvariant();
            int sum = 0;
            foreach (byte c in Encoding.ASCII.GetBytes(mapName)) {
                sum += c;
            }
            return sum & 0xff;
        }

        private int shr32(int x, int n)
        {
            return (int)(((uint)x & 0xffffffff) >> n);
        }
        private int shl32(int x, int n)
        {
            return (int)(((uint)x << n) & 0xffffffff);
        }
        private long getTime(PlayerState ps, int snap_serverTime, int df_ver, int mapNameChecksum)
        {
            int time = shl32(ps.stats[7], 0x10) | (ps.stats[8] & 0xffff);
            if (time == 0)
            {
                return 0;
            }
            if (client.isOnline) {
                return time;
            }

            time ^= Math.Abs((int)(Math.Floor(ps.origin[0]))) & 0xffff;
            time ^= shl32(Math.Abs((int)Math.Floor(ps.velocity[0])), 0x10);
            time ^= ps.stats[0] > 0 ? ps.stats[0] & 0xff : 150;
            time ^= shl32(ps.movementDir & 0xf, 0x1c);

            //if time was byte array(least significant at time[0]):
            //time[3] ^= time[2]
            //time[2] ^= time[1]
            //time[1] ^= time[0]
            //time[0] unchanged

            for (int i = 0x18; i > 0; i -= 8)
            {
                var temp = (shr32(time, i) ^ shr32(time, i - 8)) & 0xff;
                time = (time & ~shl32(0xff, i)) | shl32(temp, i);
            }

            var local1c = shl32(snap_serverTime, 2);
            //df_ver = 19124;
            //map_type = 24; // global_11cdc8, not sure why i called this map_type
            local1c += shl32(df_ver + mapNameChecksum, 8);
            local1c ^= shl32(snap_serverTime, 0x18);
            time ^= local1c;
            local1c = shr32(time, 0x1c); // time[28:32]
            local1c |= shl32(~local1c, 4) & 0xff;
            local1c |= shl32(local1c, 8);
            local1c |= shl32(local1c, 0x10);
            time ^= local1c;
            local1c = shr32(time, 0x16) & 0x3f; // time[22:28]
            time &= 0x3fffff;

            // local20 = time[0:6] + time[6:12] + time[12:18] ...
            var local20 = 0;
            for (int l = 0; l < 3; l++)
            {
                local20 += shr32(time, 6 * l) & 0x3f;
            }

            // ... + time[18:22]
            local20 += shr32(time, 0x12) & 0xf;

            if (local1c != (local20 & 0x3f))
            {
                Console.WriteLine("bad checksum");
            }
            return time;
        }
    }
}
