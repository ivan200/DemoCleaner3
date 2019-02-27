using System;
using System.Collections.Generic;
using System.Text;

namespace DemoCleaner2.DemoParser.parser
{
    class Q3DemoMessage
    {
        public int sequence;
        public int size;
        public byte[] data;

        public Q3DemoMessage(int sequence, int size) {
            this.sequence = sequence;
            this.size = size;
        }
    }
}
