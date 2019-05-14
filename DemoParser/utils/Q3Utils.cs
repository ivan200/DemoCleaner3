using System;
using System.Collections.Generic;
using System.Text;

namespace DemoCleaner3.DemoParser.utils
{
    class Q3Utils
    {
        public static int ANGLE2SHORT(long x)
        {
            return ((int)(x * 65536.0 / 360.0)) & 65535;
        }

        public static float SHORT2ANGLE(long x)
        {
            return (float)((float) x * (360.0 / 65536.0));
        }

        public static float rawBitsToFloat(long bits)
        {
            int sign = (bits & 0x80000000) > 0 ? -1 : 1;
            long e = (bits >> 23) & 0xFF;
            long m = e > 0 ? (bits & 0x7fffff) | 0x800000 : (bits & 0x7fffff) << 1;
            return sign * m * (float)Math.Pow(2, e - 150);
        }

        public static Dictionary<string, string> split_config(string src)
        {
            int begin_ind = src.Substring(0, 1) == "\\" ? 1 : 0;
            string[] src2 = src.Split('\\');
            Dictionary<string, string> rez = new Dictionary<string, string>();
            for (int k = begin_ind; k < src2.Length; k += 2)
            {
                rez.Add(src2[k].ToLower(), src2[k + 1]);
            }
            return rez;
        }
    }
}
