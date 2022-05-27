using System;
using System.Collections.Generic;
using System.Text;

namespace DemoCleaner3.DemoParser.parser
{
    class Q3DemoMessage
    {
        public int sequence;
        public int size;
        public byte[] data;

        public static int sequenceStart = 0;
        public static int sequenceEnd = 0;

        public Q3DemoMessage(int sequence, int size) {
            this.sequence = sequence;
            this.size = size;

            if (sequenceStart == 0)
            {
                sequenceStart = sequence;
            }
            if (sequence > sequenceEnd)
            {
                sequenceEnd = sequence;
            }
        }
    }
}
