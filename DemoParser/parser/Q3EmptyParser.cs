using System;
using System.Collections.Generic;
using System.Text;

namespace DemoCleaner2.DemoParser.parser
{
    class Q3EmptyParser : AbstractDemoMessageParser
    {
        public int count = 0;
        public bool parse(Q3DemoMessage message)
        {
            ++this.count;
            return true;
        }
    }
}
