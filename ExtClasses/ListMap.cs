using System;
using System.Collections.Generic;
using System.Text;

namespace DemoCleaner3.ExtClasses
{
    public class ListMap<T,V> : List<KeyValuePair<T, V>>
    {
        public ListMap() {
            
        }

        public ListMap(Dictionary<T, V> src) {
            foreach (var item in src) {
                this.Add(item.Key, item.Value);
            }
        }

        public void Add(T key, V value) {
            Add(new KeyValuePair<T, V>(key, value));
        }

        public List<V> Get(T key) {
            return FindAll(p => p.Key.Equals(key)).ConvertAll(p=> p.Value);
        }

        public Dictionary<T, V> ToDictionary() {
            Dictionary<T, V> rez = new Dictionary<T, V>();
            foreach (var item in this) {
                rez[item.Key] = item.Value;
            }
            return rez;
        }
    }
}
