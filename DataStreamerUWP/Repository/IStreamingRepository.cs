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
    /// Interface for classes that supply data for streaming
    /// <summary>
    public interface IStreamingRepository
    {
        /// <summary>
        /// Gets an individual line of data
        /// <summary>
        Task<string> GetData();

        /// <summary>
        /// Return the manifest for this application
        /// <summary>
        DataSourceManifest  Manifest { get; }
    }
}
