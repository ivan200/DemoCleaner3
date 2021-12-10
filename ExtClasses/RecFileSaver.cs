using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DemoCleaner3.ExtClasses {
    class RecFileSaver {
        private byte[] demoRecBytes;
        private Demo demo;

        public RecFileSaver(Demo demo) {
            this.demo = demo;
            this.demoRecBytes = getRecBytes(demo);
        }

        public bool canSave {
            get {
                return demoRecBytes != null;
            }
        }
        public void Save() {
            var name = demo.mapName + "_" + demo.modphysic.Replace('.', '_') + ".rec";
            var recNewFilePath = Path.Combine(demo.file.Directory.FullName, name);
            Save(recNewFilePath);
        }

        public void Save(string recFilePath) {
            var fileStream = new FileStream(recFilePath, FileMode.CreateNew);
            Save(fileStream);
        }

        public void Save(Stream saveRecFileStream) {
            saveRecFileStream.Write(demoRecBytes, 0, demoRecBytes.Length);
            saveRecFileStream.Close();
        }


        /// <summary>
        /// Generating rec bytes by demo file
        /// thx frog (aka H@des) for code
        /// </summary>
        private byte[] getRecBytes(Demo demo) {
            var info = demo.rawInfo;
            if (info == null) {
                return null;
            }
            var MAX_CHECKPOINTS = 32;
            var cps = info.cpData;
            if (cps.Count == 0) {
                return null;
            }

            if (cps.Count > MAX_CHECKPOINTS) {
                cps = cps.GetRange(0, MAX_CHECKPOINTS);

                var endRange = cps.GetRange(MAX_CHECKPOINTS, cps.Count - MAX_CHECKPOINTS);
                long sum = 0;
                foreach (var cp in endRange) {
                    sum += cp;
                }
                cps[MAX_CHECKPOINTS - 1] = sum;
            }

            //checkpoint data -> 128 bytes
            int cp_sum = 0;
            var cp_data = new List<byte>();

            foreach (var cp in cps) {
                cp_sum += (int)cp;  //total time at current checkpoint
                var cp_byte = toBytes(cp_sum, 4, true);
                cp_data.AddRange(cp_byte);
            }
            var filledData = toBytes(0, MAX_CHECKPOINTS * 4 - cp_data.Count, true);
            cp_data.AddRange(filledData);

            //last four bytes -> keeps track of how many checkpoints there are (excluding final) -> num of cps - 1
            var cpDataFooter = toBytes(cps.Count - 1, 4, true);
            cp_data.AddRange(cpDataFooter);

            //byte header -> 4 bytes. [version number][checksum byte][0][0]
            byte versionNum = 3;  //.rec structure version number (3)

            //checksum = sum all bytes in cp data -> 1st byte of result
            var cpDataSum = 0;
            foreach (var b in cp_data) {
                cpDataSum += b;
            }
            var checksum = toBytes(cpDataSum, 0, true)[0];
            var header = new byte[4] { versionNum, checksum, 0, 0 };

            //add header
            var rec_data = new List<byte>();
            rec_data.AddRange(header);
            rec_data.AddRange(cp_data);

            return rec_data.ToArray();
        }

        private byte[] toBytes(int value, int size, bool isLittle) {
            byte[] intBytes = BitConverter.GetBytes(value);
            if (BitConverter.IsLittleEndian) {
                Array.Reverse(intBytes);
            }
            if (size <= 0) {
                if (isLittle) Array.Reverse(intBytes);
                return intBytes;
            }
            var sizedBytes = new byte[size];
            Array.Copy(intBytes, sizedBytes, intBytes.Length);
            if (isLittle) Array.Reverse(sizedBytes);
            return sizedBytes;
        }
    }
}
