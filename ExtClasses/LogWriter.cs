using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace DemoCleaner3.ExtClasses
{
    class LogWriter
    {
        public enum Operation {
            MoveFile,
            RenameFile,
            DeleteFile,
            CreateDir,
            DeleteDir,
            ChangeCreationTime
        }

        private string m_exePath = string.Empty;

        string filePath = null;
        FileStream fileStream = null;
        StreamWriter logWriter = null;
        bool isOpened = false;
        int writeCount = 0;
        Properties.Settings prop;

        public LogWriter() {
            prop = Properties.Settings.Default;
        }

        public void openStream(Form1.JobType jobType) {
            string operation = "";
            switch (jobType) {
                case Form1.JobType.CLEAN: operation = "clean"; break;
                case Form1.JobType.MOVE: operation = "move"; break;
                case Form1.JobType.RENAME: operation = "rename"; break;
            }
            var name = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") + "_" + operation + ".txt";
            m_exePath = Path.GetDirectoryName(Application.ExecutablePath);
            filePath = Path.Combine(m_exePath, name);

            fileStream = new FileStream(filePath, FileMode.Append);
            logWriter = new StreamWriter(fileStream, Encoding.UTF8);
            isOpened = true;
            writeCount = 0;
        }

        public void closeStream() {
            if (isOpened) {
                isOpened = false;
                logWriter.Close();
                fileStream.Close();
                if (writeCount == 0) {
                    var file = new FileInfo(filePath);
                    file.Delete();
                }
            }
        }

        public void LogWrite(Operation operation, params string[] messages) {
            if (!isOpened) {
                return;
            }

            switch (operation) {
                case Operation.CreateDir:           if (!prop.logCreateDir)             return; break;
                case Operation.DeleteDir:           if (!prop.logDelDir)                return; break;
                case Operation.RenameFile:          if (!prop.logRenameFile)            return; break;
                case Operation.MoveFile:            if (!prop.logMoveFile)              return; break;
                case Operation.DeleteFile:          if (!prop.logDelFile)               return; break;
                case Operation.ChangeCreationTime:  if (!prop.logChangeCreationDate)    return; break;
            }

            writeCount++;
            switch (operation) {
                case Operation.CreateDir:
                    logWriter.WriteLine("CreateDir");
                    logWriter.WriteLine(" path: {0}", messages[0]);
                    logWriter.WriteLine("-------------------------------");
                    break;
                case Operation.DeleteDir:
                    logWriter.WriteLine("DeleteDir");
                    logWriter.WriteLine(" path: {0}", messages[0]);
                    logWriter.WriteLine("-------------------------------");
                    break;
                case Operation.RenameFile:
                    logWriter.WriteLine("RenameFile");
                    logWriter.WriteLine(" from: {0}", messages[0]);
                    logWriter.WriteLine(" to:   {0}", messages[1]);
                    logWriter.WriteLine("-------------------------------");
                    break;
                case Operation.MoveFile:
                    logWriter.WriteLine("MoveFile");
                    logWriter.WriteLine(" from: {0}", messages[0]);
                    logWriter.WriteLine(" to:   {0}", messages[1]);
                    logWriter.WriteLine("-------------------------------");
                    break;
                case Operation.DeleteFile:
                    logWriter.WriteLine("DeleteFile");
                    logWriter.WriteLine(" file: {0}", messages[0]);
                    logWriter.WriteLine("-------------------------------");
                    break;
                case Operation.ChangeCreationTime:
                    logWriter.WriteLine("ChangeCreationTime");
                    logWriter.WriteLine(" file: {0}", messages[0]);
                    logWriter.WriteLine(" from: {0}, to: {1}", messages[1], messages[2]);
                    logWriter.WriteLine("-------------------------------");
                    break;
            }
        }
    }
}
