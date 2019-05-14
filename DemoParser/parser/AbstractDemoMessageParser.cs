using System;
using System.Collections.Generic;
using System.Text;

namespace DemoCleaner3.DemoParser.parser
{
    interface AbstractDemoMessageParser
    {
        bool parse(Q3DemoMessage message);
    }
}
