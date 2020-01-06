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
    public static class TaskExtensions
    {
        /// <summary>
        /// Run a task with a timeout
        /// </summary>
        public static async Task WithTimeout(this Task task, TimeSpan timeout)
        {
            if (task == await Task.WhenAny(task, Task.Delay(timeout)))
            {
                await task;
                return;
            }

            throw new TimeoutException();
        }

        /// <summary>
        /// Run a task with a timeout
        /// </summary>
        /// <param name="timeout">Timeout in milliseconds</param>
        public static async Task WithTimeout(this Task task, int timeout)
        {
            int seconds = timeout / 1000;
            int milliseconds = timeout % 1000;

            await task.WithTimeout(new TimeSpan(0, 0, 0, seconds, milliseconds));
        }
    }
}
