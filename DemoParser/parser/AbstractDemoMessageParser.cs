using System;
using System.Collections.Generic;
using System.Text;

namespace DemoCleaner2.DemoParser.parser
{
    interface AbstractDemoMessageParser
    {
        bool parse(Q3DemoMessage message);
    }
}
