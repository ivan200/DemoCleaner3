using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using DemoCleaner3.DemoParser.utils;

namespace DemoCleaner3.DemoParser.parser
{
    class Q3MessageStream
    {
        private BinaryReader binaryReader = null;
        private Stream fileHandle = null;
        private int readMessages = 0;

        /**
         * Q3DemoParser constructor.
         * @param string file_name - name of demo-file
         * @throws Exception in case file is failed to open
         */
        public Q3MessageStream(string file_name)
        {
            this.readMessages = 0;
            this.fileHandle = File.OpenRead(file_name);

            if (!fileHandle.CanRead)
            {
                throw new ErrorCantOpenFile();
            }
            else
            {
                binaryReader = new BinaryReader(fileHandle);
            }
        }

        /**
        * @return Q3DemoMessage return a next message buffer or null if EOD is reached
        * @throws Exception in case stream is corrupted
        */
        public Q3DemoMessage nextMessage()
        {
            byte[] headerBuffer = binaryReader.ReadBytes(8);
            if (headerBuffer.Length != 8) {
                return null;
            }

            //if (BitConverter.IsLittleEndian) {
            //    Array.Reverse(headerBuffer);
            //}

            int sequence = BitConverter.ToInt32(headerBuffer, 0);
            int msgLength = BitConverter.ToInt32(headerBuffer, 4);

            if (sequence == -1 && msgLength == -1) {
                // a normal case, end of message-sequence
                return null;
            }

            if (msgLength < 0 || msgLength > Constants.Q3_MESSAGE_MAX_SIZE) {
                throw new ErrorWrongLength();
            }

            var msg = new Q3DemoMessage(sequence, msgLength);

            msg.data = binaryReader.ReadBytes(msgLength);

            this.readMessages++;
            return msg;
        }

        public void close()
        {
            if (binaryReader != null) {
                binaryReader.Close();
                binaryReader = null;
            }
            if (fileHandle != null) {
                fileHandle.Close();
                fileHandle = null;
            }
        }
    }
}
