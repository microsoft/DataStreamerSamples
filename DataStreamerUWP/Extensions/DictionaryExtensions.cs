using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.DataStreamer.UWP
{
    public static class DictionaryExtensions
    {
        public static IDictionary<K, V> AddOrUpdate<K, V>(this IDictionary<K, V> dict, K key, V val)
        {
            if (dict.ContainsKey(key))
                dict[key] = val;
            else
                dict.Add(key, val);

            return dict;
        }

        public static V ValueOrDefault<K, V>(this IDictionary<K, V> dict, K key, V defaultVal)
        {
            if (dict.ContainsKey(key))
                return dict[key];

            return defaultVal;
        }
    }
}
