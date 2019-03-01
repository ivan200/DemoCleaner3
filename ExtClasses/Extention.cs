using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace DemoCleaner2
{
    public static class Extention
    {
        public static List<KeyValuePair<TKey, TValue>> MakeListFromGroups<TKey, TValue>(IEnumerable<IGrouping<TKey, TValue>> folders)
        {
            var dict = new List<KeyValuePair<TKey, TValue>>();

            foreach (var group in folders) {
                foreach (var item in group) {
                    dict.Add(new KeyValuePair<TKey, TValue>(group.Key, item));
                }
            }
            return dict;
        }
    }
}
