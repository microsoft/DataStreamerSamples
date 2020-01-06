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
    /// <summary>
    /// Interface for services that communicate with Data Streamer
    /// <summary>
    public interface IStreamingService
    {
        Task<string> Connect(IDictionary<string, object> mesage);
        Task         Disconnect();

        Task         StartData();
        Task         StopData(bool disconnecting = false);
        Task         StartRecording(string fileName, Func<string, Task> fnOnError = null);
        Task         StopRecording(Func<string, Task> fnOnError = null);
        Task         Reset();
        Task         Ready();
        Task         NotReady();
        Task         UpdateManifest(Func<string, Task> fnOnError = null);
        Task         OnEvent(IDictionary<string, object> message);
        Task         OnCommand(string command, dynamic parms);
    }
}
