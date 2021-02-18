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
            for (int k = begin_ind; k < src2.Length-1; k += 2)
            {
                var key = src2[k];
                var value = src2[k + 1];
                if (!string.IsNullOrEmpty(value)) {
                    rez[key] = value;
                }
            }
            return rez;
        }

        static bool debug = false;
        public static void PrintDebug(string message, params object[] arg) {
            string errorstring = string.Format(message, arg);
            PrintDebug(errorstring);
        }
        public static void PrintDebug(string message) {
            if (debug) {
                Console.WriteLine(message);
            }
        }

        public static void PrintDebug(Dictionary<string, string> errorsdict, string message, params object[] arg) {
            string errorstring = string.Format(message, arg);
            errorsdict[errorstring] = null;
            PrintDebug(errorstring);
        }
        public static void PrintDebug(Dictionary<string, string> errorsdict, ErrorType message) {
            var str = getMessageForError(message);
            errorsdict[str] = null;
            PrintDebug(str);
        }

        public enum ErrorType {
            UnableToParseDeltaEntityState,
            BadCommandInParseGameState,
            DeltaFrameTooOld,
            DeltaParseEntitiesNumTooOld,
            ParsePacketEntitiesEndOfMessage,
            BaselineNumberOutOfRange,
            ParseSnapshotInvalidsize,
            DeltaFromInvalidFrame,
            BadChecksum,
            Unknown1,
            Unknown2
        }

        public static string getMessageForError(ErrorType error) {
            switch (error) {
                case ErrorType.UnableToParseDeltaEntityState: return "Unable to parse delta-entity state";
                case ErrorType.BadCommandInParseGameState: return "Bad command in parseGameState";
                case ErrorType.DeltaFrameTooOld: return "Delta frame too old.";
                case ErrorType.DeltaParseEntitiesNumTooOld: return "Delta parseEntitiesNum too old";
                case ErrorType.ParsePacketEntitiesEndOfMessage: return "CL_ParsePacketEntities: end of message";
                case ErrorType.BaselineNumberOutOfRange: return "Baseline number out of range";
                case ErrorType.ParseSnapshotInvalidsize: return "CL_ParseSnapshot: Invalid size for areamask";
                case ErrorType.DeltaFromInvalidFrame: return "Delta from invalid frame (not supposed to happen!)";
                case ErrorType.BadChecksum: return "Bad checksum at decoding demo time";
                case ErrorType.Unknown1: return "Unknown Error 1";
                case ErrorType.Unknown2: return "Unknown Error 2";
                default: return error.ToString();
            }
        }


    }
}
