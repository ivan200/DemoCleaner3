using System;
using Microsoft.Win32;
using System.Runtime.InteropServices;
using System.IO;
using System.Linq;
using System.Drawing;

namespace DemoCleaner3.ExtClasses {
   
    public class FileAssociation {
        public string Extension { get; set; }
        public string ProgId { get; set; }
        public string FileTypeDescription { get; set; }
        public string ExecutableFilePath { get; set; }
    }

    public class FileAssociations {
        // needed so that Explorer windows get refreshed after the registry is updated
        [System.Runtime.InteropServices.DllImport("Shell32.dll")]
        private static extern int SHChangeNotify(int eventId, int flags, IntPtr item1, IntPtr item2);

        private const int SHCNE_ASSOCCHANGED = 0x8000000;
        private const int SHCNF_FLUSH = 0x1000;

        public static void EnsureAssociationsSet() {
            var filePath = Environment.GetCommandLineArgs()[0];

            var dm_68 = new FileAssociation {
                Extension = ".dm_68",
                ProgId = "DemoCleaner3",
                FileTypeDescription = "Quake 3 demo file",
                ExecutableFilePath = filePath
            };

            var dm_67 = new FileAssociation {
                Extension = ".dm_67",
                ProgId = "DemoCleaner3",
                FileTypeDescription = "Quake 3 demo file",
                ExecutableFilePath = filePath
            };

            var dm_66 = new FileAssociation {
                Extension = ".dm_66",
                ProgId = "DemoCleaner3",
                FileTypeDescription = "Quake 3 demo file",
                ExecutableFilePath = filePath
            };

            EnsureAssociationsSet(dm_68, dm_67, dm_66);
        }

        public static Boolean isAsociated() {
            using (RegistryKey ext = Registry.ClassesRoot.OpenSubKey(".dm_68", false)) {
                if (ext != null) {
                    return true;
                }
            }
            return false;
        }

        public static void EnsureAssociationsSet(params FileAssociation[] associations) {
            bool madeChanges = false;
            foreach (var association in associations) {
                madeChanges |= SetAssociation(
                    association.Extension,
                    association.ProgId,
                    association.FileTypeDescription,
                    association.ExecutableFilePath);
            }

            if (madeChanges) {
                SHChangeNotify(SHCNE_ASSOCCHANGED, SHCNF_FLUSH, IntPtr.Zero, IntPtr.Zero);
            }
        }

        public static bool SetAssociation(string extension, string progId, string fileTypeDescription, string applicationFilePath) {
            bool madeChanges = false;
            madeChanges |= SetKeyDefaultValue($@"Software\Classes\{extension}", progId);
            madeChanges |= SetKeyDefaultValue($@"Software\Classes\{progId}\DefaultIcon", $@"""{applicationFilePath}"",1");
            madeChanges |= SetKeyDefaultValue($@"Software\Classes\{progId}", fileTypeDescription);
            madeChanges |= SetKeyDefaultValue($@"Software\Classes\{progId}\shell\open\command", $@"""{applicationFilePath}"" ""%1""");
            return madeChanges;
        }

        private static bool SetKeyDefaultValue(string keyPath, string value) {
            using (var key = Registry.CurrentUser.CreateSubKey(keyPath)) {
                if (key.GetValue(null) as string != value) {
                    key.SetValue(null, value);
                    return true;
                }
            }
            return false;
        }


        [DllImport("shell32.dll")]
        public static extern IntPtr ExtractIcon(IntPtr hInst, string file, int nIconIndex);

        [DllImport("user32.dll", SetLastError = true)]
        static extern bool DestroyIcon(IntPtr hIcon);

        /// <summary>
        /// Sets icon from .exe or .dll into form object
        /// </summary>
        /// <param name="iIconIndex">Icon index to use.</param>
        /// <param name="form">Form to assign to given icon</param>
        /// <returns>true if ok, false if failed.</returns>
        public static Icon getIcon(int iIconIndex) {
            var filePath = Environment.GetCommandLineArgs()[0];
            IntPtr hIcon = ExtractIcon(IntPtr.Zero, filePath, iIconIndex);
            if (hIcon == IntPtr.Zero)
                return null;

            Icon icon = (Icon)Icon.FromHandle(hIcon).Clone();
            DestroyIcon(hIcon);
            return icon;
        }
    }

}
