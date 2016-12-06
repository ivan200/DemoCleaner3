using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;

namespace DemoCleaner2
{
    public class DemoFolder
    {
        public string _folderName { get; set; }
        public string fullFolderName { get; set; }
        public DemoFolder(string folderName)
        {
            this._folderName = folderName;
        }

        //Получение имён первых папок
        static string getFirstDir(string Key)
        {
            var c = Key[0];
            if (char.IsDigit(c))
            {
                return ("0-9");
            }
            if (c >= 'a' && c <= 'z')
            {
                return c.ToString();
            }
            else return "._";
        }

        //по уровню вложенности и ключу группировки нарезаем полный путь к каталогу
        public static string GetFullNameFromIndex(string key, int index)
        {
            if (key.Length == 1)
            {
                return getFirstDir(key);
            }

            var subkeys = new List<string>();
            for (int i = 0; i < key.Length - 1; i++)
            {
                subkeys.Add(key.Substring(0, i + 1));
            }
            subkeys[0] = getFirstDir(key);

            var separator = Path.DirectorySeparatorChar.ToString();

            var minString = string.Join(separator, subkeys.Take(index + 1).ToArray());
            return Path.Combine(minString, key);
        }

        //получение ключа группировки по уровню вложенности и названию папки
        public static string GetKeyFromIndex(string key, int index)
        {
            if (index == 0)
            {
                return getFirstDir(key);
            }
            return key.Substring(index, 1);
        }

    }
}
