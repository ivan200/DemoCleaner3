using System;
using System.Collections.Generic;
using System.Text;

namespace DemoCleaner3.DemoParser.structures
{
    public enum TrType
    {
        TR_STATIONARY,
        TR_INTERPOLATE,     // non-parametric, but interpolate between snapshots
        TR_LINEAR,
        TR_LINEAR_STOP,
        TR_SINE,            // value = base + sin( time / duration ) * delta
        TR_GRAVITY
    }
}
