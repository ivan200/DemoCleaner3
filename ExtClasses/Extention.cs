using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Collections;

namespace DemoCleaner3
{
    public static class Ext
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

        public static TValue GetOrNull<TKey, TValue>(Dictionary<TKey, TValue> dictionary, TKey key)
        {
            if (dictionary.ContainsKey(key)) {
                return dictionary[key];
            }
            return default(TValue);
        }
    }

    public static class Ext2<TKey, TValue> where TValue : new()
    {
        public static TValue GetOrCreate(Dictionary<TKey, TValue> dictionary, TKey key)
        {
            if (!dictionary.ContainsKey(key)) {
                dictionary[key] = new TValue();
            } 
            return dictionary[key];
        }
    }
}
