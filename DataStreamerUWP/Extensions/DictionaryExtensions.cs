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
        public static IDictionary<string, string> AppendKeyValuePairs(this IDictionary<string, string> dict, string[] keyValuePairs)
        {
            if (keyValuePairs != null)
            {
                int len = keyValuePairs.Length;

                for (int i = 0; i < len; i += 2)
                {
                    if (i + 1 < len)
                    {
                        dict.Add(keyValuePairs[i], keyValuePairs[i + 1]);
                    }
                }
            }

            return dict;
        }

        public static IDictionary<string, string> Merge(this IDictionary<string, string> dict1, IDictionary<string, string> dict2, bool allowEmpties = false)
        {
            if(dict1 == null)
                return dict2;

            if(dict2 != null)
            { 
                foreach(var key in dict2.Keys)
                {
                    var newVal = dict2[key];

                    if(allowEmpties || !string.IsNullOrWhiteSpace(newVal))
                        dict1.AddOrUpdate(key, newVal);
                }
            }

            return dict1;
        }

        public static string ToLogString(this IDictionary<string, string> dict, string initString)
        {
            if (dict != null && dict.Count > 1)
            {
                var sb = new StringBuilder(initString.Trim());

                sb.AppendLine();

                foreach(string key in dict.Keys)
                {
                    sb.AppendLine($"  {key}: {dict[key]}");
                }

                return sb.ToString();
            }

            return initString;
        }

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

        public static IDictionary<string, string> ToDictionary(this object obj)
        {
            if (obj == null)
                return new Dictionary<string, string>();

            if (obj is IDictionary<string, string> data)
                return data;

            if (obj is string[] keyValuePairs)
            {
                var dict = new Dictionary<string, string>();

                return dict.AppendKeyValuePairs(keyValuePairs);
            }

            var result2 = new Dictionary<string, string>();
            var members = obj.GetType().GetMembers();

            foreach (var member in members.Where(info => info.MemberType == MemberTypes.Method && info.Name.StartsWith("get_")))
            {
                var field = member as MethodInfo;
                var name  = field?.Name.Replace("get_", "");
                var value = field?.Invoke(obj, null);

                if (name != null)
                { 
                    result2.Add(name, value?.ToString());
                }
            }

            return result2;
        }

        /// <summary>
        /// Match a wildcard to a key at either beginning or end of key. 
        /// Note the "this" dictionary contains the wildcards not the key to match to.
        /// </summary>
        public static T WildcardMatch<T>(this IDictionary<string, T> dict, string matchKey)
        {
            foreach(var key in dict.Keys)
            { 
                if(key.StartsWith("*"))
                { 
                    if(matchKey.EndsWith(key.Substring(1)))
                        return dict[key];

                    continue;
                }

                if(key.EndsWith("*"))
                { 
                    if(matchKey.StartsWith(key.Substring(0, key.Length - 1)))
                        return dict[key];

                    continue;
                }

                if(key == matchKey)
                    return dict[matchKey];
            }

            return default(T);
        }
    }
}
