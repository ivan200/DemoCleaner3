using System;
using System.Collections.Generic;
using System.Text;

namespace DemoCleaner2.DemoParser.huffman
{
    class Q3HuffmanNode
    {
        public Q3HuffmanNode left;
        public Q3HuffmanNode right;
        public long symbol;
        /**
         * Q3HuffmanNode constructor.
         * @param $symbol
         */
        public Q3HuffmanNode()
        {
            this.symbol = Constants.Q3_HUFFMAN_NYT_SYM;
        }
    }
}
