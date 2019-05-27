using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace DemoCleaner3.ExtClasses
{
    class FileHelper
    {
        Action<int> _onProgressPercentChanged;
        Action<int> _onProgressChanged;
        LogWriter logger;

        public FileHelper(Action<int> onProgressChanged = null, Action<int> onProgressPercentChanged = null) {
            _onProgressChanged = onProgressChanged;
            _onProgressPercentChanged = onProgressPercentChanged;
            logger = new LogWriter();
        }

        public decimal _countMoveFiles = 0;
        public decimal _countDeleteFiles = 0;
        public decimal _countCreateDir = 0;
        public decimal _countDeleteDir = 0;
        public decimal _countRenameFiles = 0;

        public decimal _countProgressDemos = 0;
        public decimal _countDemosAmount = 0;

        decimal _CountProgressDemos {
            get { return _countProgressDemos; }
            set {
                _countProgressDemos = value;
                float percent = ((float)_countProgressDemos / (float)_countDemosAmount) * 100;
                int dPercent = (int)percent;

                if (dPercent < 0) dPercent = 0;
                if (_onProgressChanged != null) {
                    _onProgressChanged.Invoke((int)_countProgressDemos);
                }
                if (_onProgressPercentChanged != null) {
                    _onProgressPercentChanged.Invoke(dPercent);
                }
            }
        }

        public void increaseProgressCount(int byValue) {
            if (byValue > 0) {
                _CountProgressDemos += byValue;
            }
        }

        //Reset the counter
        public void resetValues(int demosAmount, bool clearCounter = true)
        {
            if (clearCounter) {
                _countMoveFiles = 0;
                _countDeleteFiles = 0;
                _countCreateDir = 0;
                _countDeleteDir = 0;
            }

            _CountProgressDemos = 0;
            _countDemosAmount = demosAmount;
        }

        public void StartLogger(Form1.JobType jobType) {
            logger.openStream(jobType);
        }

        public void stopLogger() {
            logger.closeStream();
        }

        //File transfer function
        public void moveFile(FileInfo file, DirectoryInfo dir, bool deleteIdentical)
        {
            if (!dir.Exists) {
                logger.LogWrite(LogWriter.Operation.CreateDir, dir.FullName);
                dir.Create();
                _countCreateDir++;
                dir.Refresh();
            }

            var path = Path.Combine(dir.FullName, file.Name);

            if (file.FullName == path) {
                _CountProgressDemos++;
                return;
            }

            if (File.Exists(path)) {
                if (deleteIdentical) {
                    deleteCheckRules(file);
                } else {
                    _CountProgressDemos++;
                }
            } else {
                moveCheckRules(file, path, false);
            }
        }

        //File rename function
        public string renameFile(FileInfo file, string newName, bool deleteIdentical)
        {
            string newPath = Path.Combine(file.Directory.FullName, newName);
            if (!newPath.ToLowerInvariant().Equals(file.FullName.ToLowerInvariant())) {
                if (File.Exists(newPath)) {
                    if (deleteIdentical) {
                        deleteCheckRules(file);
                    } else {
                        _CountProgressDemos++;
                    }
                } else {
                    moveCheckRules(file, newPath, true);
                }
            } else {
                if (!newPath.Equals(file.FullName)) {
                    moveCheckRules(file, newPath, true);
                } else {
                    _CountProgressDemos++;
                }
            }
            return newPath;
        }

        public void deleteCheckRules(FileInfo file)
        {
            _countDeleteFiles++;
            _CountProgressDemos++;
            logger.LogWrite(LogWriter.Operation.DeleteFile, file.FullName);
            tryOperateFile(file, f => {
                file.Delete();
            });
        }

        public void moveCheckRules(FileInfo file, string path, bool isRenaming) {
            _countMoveFiles++;
            _CountProgressDemos++;
            logger.LogWrite(isRenaming ? LogWriter.Operation.RenameFile : LogWriter.Operation.MoveFile, file.FullName, path);
            tryOperateFile(file, f => {
                file.MoveTo(path);
            });
        }

        private void tryGetAccess(FileInfo file)
        {
            var attr = File.GetAttributes(file.FullName);
            attr = attr & ~FileAttributes.ReadOnly;
            File.SetAttributes(file.FullName, attr);

            //var fSecurity = file.GetAccessControl();
            //fSecurity.AddAccessRule(
            //    new FileSystemAccessRule(
            //        new SecurityIdentifier(
            //            WellKnownSidType.WorldSid, null),
            //        FileSystemRights.FullControl,
            //        InheritanceFlags.ObjectInherit | InheritanceFlags.ContainerInherit,
            //        PropagationFlags.NoPropagateInherit,
            //        AccessControlType.Allow));
            //file.SetAccessControl(fSecurity);
        }


        //Deleting empty folders
        public void deleteEmpty(DirectoryInfo dir)
        {
            var allDirs = dir.GetDirectories("*", SearchOption.AllDirectories);
            //sort by the number of subdirectories
            var orderedDirs = allDirs.OrderByDescending(x => x.FullName.Split(Path.DirectorySeparatorChar).Count());

            //and delete in the loop
            foreach (var item in orderedDirs) {
                if (item.Exists) {
                    var files = item.GetFiles();
                    var dirs = item.GetDirectories();
                    if ((files == null || files.Count() == 0) && (dirs == null || dirs.Count() == 0)) {
                        try {
                            item.Delete();
                        } catch (Exception ex) {}

                        logger.LogWrite(LogWriter.Operation.DeleteDir, item.FullName);
                        _countDeleteDir++;
                    }
                }
            }
        }

        public void tryOperateFile(FileInfo file, Action<FileInfo> function) {
            if (file.Exists) {
                try {
                    function(file);
                } catch (Exception ex) {
                    if (ex is UnauthorizedAccessException) {
                        tryGetAccess(file);
                        function(file);
                    }
                }
            }
        }

        public void fixCreationTime(FileInfo file, DateTime? date) {
            var now = DateTime.Now;
            if (!date.HasValue && (file.CreationTime > now || file.LastWriteTime > now)) {
                date = now;
            }

            var fc = file.CreationTime;

            if (date.HasValue && (fc.Date != date.Value.Date || fc.Hour != date.Value.Hour || fc.Minute != date.Value.Minute)) {
                logger.LogWrite(LogWriter.Operation.ChangeCreationTime, file.FullName,
                    file.CreationTime.ToString("yyyy-MM-dd HH:mm"), date.Value.ToString("yyyy-MM-dd HH:mm"));
                tryOperateFile(file, f => {
                    f.CreationTime = date.Value;
                    f.LastWriteTime = date.Value;
                });
            }
        }
    }
}

