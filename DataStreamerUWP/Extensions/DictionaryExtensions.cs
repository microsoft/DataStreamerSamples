//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// This code is licensed under the MIT License (MIT).
// THIS CODE IS PROVIDED AS IS WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************

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
        /// <summary>
        /// Adds a new value or updates an existing if that key already exists
        /// </summary>
        /// <typeparam name="K">Key type</typeparam>
        /// <typeparam name="V">Value type</typeparam>
        /// <param name="key">Key of value to add or update</param>
        /// <param name="val"></param>
        public static IDictionary<K, V> AddOrUpdate<K, V>(this IDictionary<K, V> dict, K key, V val)
        {
            if (dict.ContainsKey(key))
                dict[key] = val;
            else
                dict.Add(key, val);

            return dict;
        }

        /// <summary>
        /// Returns a value with the given key or the default value if it does not exists
        /// </summary>
        /// <typeparam name="K">Key type</typeparam>
        /// <typeparam name="V">Value type</typeparam>
        /// <param name="key">Key of value to retrieve</param>
        /// <param name="defaultVal">The default value to return if a value with the given key does not exist</param>
        public static V ValueOrDefault<K, V>(this IDictionary<K, V> dict, K key, V defaultVal)
        {
            if (dict.ContainsKey(key))
                return dict[key];

            return defaultVal;
        }
    }
}
