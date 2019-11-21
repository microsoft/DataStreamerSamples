using System;
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

using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.DataStreamer.UWP
{
    public static class IEnumerableExtensions
    {    
        /// <summary>
        ///  Given a list of objects and a param name to identify a child list of those objects that establish a hierarchy, return a list of just the leaf nodes in the hierarchy
        /// </summary>
        public static IList<T> GetLeaves<T>(this IEnumerable<T> obj, string childParam, Func<T, bool> fnCanAdd = null)
        {
            if(obj == null)
                return null;

            var leaves = new List<T>();

            foreach(var child in obj)
                GetChildLeaves(leaves, child, childParam, fnCanAdd);

            return leaves;
        }

        #region Private 
        
        private static void GetChildLeaves<T>(IList<T> list, T child, string childParam, Func<T, bool> fnCanAdd)
        {
            var childList = child.GetProperty(childParam) as IEnumerable<T>;

            if(childList == null || childList.Count() == 0)
            {
                // If no children then add child as leaf
                if(fnCanAdd == null || fnCanAdd(child))
                    list.Add(child);

                return;
            }

            foreach(var gchild in childList)
                GetChildLeaves(list, gchild, childParam, fnCanAdd);
           
            return;
        }

        #endregion
    }
}
