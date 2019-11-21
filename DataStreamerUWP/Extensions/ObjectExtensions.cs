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
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.DataStreamer.UWP
{
    public static class ObjectExtensions
    {
        /// <summary>
        /// Return the value the object's property with the given name
        /// </summary>
        public static object GetProperty(this object obj, string name)
        {        
            if(string.IsNullOrWhiteSpace(name))
                return null;

            var type = obj.GetType();
            var prop = type.GetProperty(name);

            return prop?.GetValue(obj, null);
        }    
    }
}
