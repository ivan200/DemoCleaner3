using DemoRenamer.DemoParser;
using System;
using System.Collections.Generic;
using System.Text;

namespace DemoRenamer
{
    interface AbstractDemoMessageParser
    {
        bool parse(Q3DemoMessage message);
    }
}
