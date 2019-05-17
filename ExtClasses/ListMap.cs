using System;
using System.Collections.Generic;
using System.Text;

namespace DemoCleaner3.ExtClasses
{
    public class ListMap<T,V> : List<KeyValuePair<T, V>>
    {
        public void Add(T key, V value) {
            Add(new KeyValuePair<T, V>(key, value));
        }

        public List<V> Get(T key) {
            return FindAll(p => p.Key.Equals(key)).ConvertAll(p=> p.Value);
        }
    }
}
