using System;
using System.Collections.Generic;
using System.Text;

namespace DemoCleaner3.DemoParser.structures
{
    public class Trajectory
    {
        public TrType trType = TrType.TR_STATIONARY;
        public long trTime;
        public long trDuration;
        public float[] trBase = new float[3];
        public float[] trDelta = new float[3];

        public Trajectory()
        {
        }

        public Trajectory(Trajectory x)
        {
            this.trType = x.trType;
            this.trTime = x.trTime;
            this.trDuration = x.trDuration;
            Array.Copy(x.trBase, trBase, 3);
            Array.Copy(x.trDelta, trDelta, 3);
        }
    }

}
