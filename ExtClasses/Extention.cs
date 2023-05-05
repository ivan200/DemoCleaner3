using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using DemoCleaner3.ExtClasses;

namespace DemoCleaner3 {
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

        public static int GetOrZero<T> (Dictionary<T, string> dictionary, T key)
        {
            if (dictionary.ContainsKey(key)) {
                var res = dictionary[key];
                int intRes = 0;
                if (!string.IsNullOrEmpty(res)) {
                    int.TryParse(res, out intRes);
                }
                return intRes;
            }
            return 0;
        }

        public static Dictionary<TKey, TValue> Join <TKey, TValue>(params Dictionary<TKey, TValue>[] dicts) {
            var nDict = new Dictionary<TKey, TValue>();
            foreach (var dict in dicts) {
                if (dict != null) {
                    foreach (var item in dict) {
                        nDict[item.Key] = item.Value;
                    }
                }
            }
            return nDict;
        }

        public static bool ContainsAny(String data, params String[] values) {
            foreach (var item in values) {
                if (data.Contains(item)) {
                    return true;
                }
            }
            return false;
        }

        public static bool ContainsAnySplitted(String data, params String[] values) {
            var splitted = Regex.Split(data, "[^a-zA-Z0-9]");
            foreach (var part in splitted) {
                foreach (var item in values) {
                    if (part.ToLowerInvariant() == item.ToLowerInvariant()) {
                        return true;
                    }
                }
            }
            return false;
        }

        public static T MinOf<T>(IEnumerable<T> collection, Func<T, long> minComparizon) {
            long minResult = long.MaxValue;
            T minObject = default(T);
            foreach (var val in collection) {
                var res = minComparizon(val);
                if (minResult > res) {
                    minResult = res;
                    minObject = val;
                }
            }
            return minObject;
        }
        public static int CountOf<T>(IEnumerable<T> collection, T toCompare) {
            int count = 0;
            foreach (var val in collection) {
                if (val.Equals(toCompare)) {
                    count++;
                }
            }
            return count;
        } 

        public static int IndexOf<T>(IEnumerable<T> collection, Func<T, bool> function) {
            int i = 0;
            foreach (var val in collection) {
                if (function(val)) {
                    return i;
                }
                i++;
            }
            return -1;
        }

        public static List<List<T>> Split<T>(IEnumerable<T> collection, int chunkSize) {
            var splitList = new List<List<T>>();
            var chunkCount = (int)Math.Ceiling((double)collection.Count() / (double)chunkSize);

            for (int c = 0; c < chunkCount; c++) {
                var skip = c * chunkSize;
                var take = skip + chunkSize;
                var chunk = new List<T>(chunkSize);

                for (int e = skip; e < take && e < collection.Count(); e++) {
                    chunk.Add(collection.ElementAt(e));
                }

                splitList.Add(chunk);
            }

            return splitList;
        }

        public static Dictionary<string, V> LowerKeys<V>(Dictionary<string, V> dictionary) {
            Dictionary<string, V> rez = new Dictionary<string, V>();
            foreach (var item in dictionary) {
                rez.Add(item.Key.ToLowerInvariant(), item.Value);
            }
            return rez;
        }

        public static void replaceKeys(ListMap<string, string> src, Dictionary<string, string> replaces) {
            for (int i = 0; i < src.Count; i++) {
                var str = src[i];
                if (replaces.ContainsKey(str.Key.ToLowerInvariant())) {
                    src[i] = new KeyValuePair<string, string>(replaces[str.Key], str.Value);
                }
            }
        }

        public static int ToInt(string inputString, int defaultValue) {
            if (!string.IsNullOrEmpty(inputString) && int.TryParse(inputString, out int result)) {
                return result;
            } else {
                return defaultValue;
            }
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

    public struct Pair
    {
        public Pair(string key, string value) {
            Key = key;
            Value = value;
        }
        public string Key { get; }
        public string Value { get; }
    }
}
