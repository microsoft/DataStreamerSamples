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

using Windows.ApplicationModel.AppService;

namespace Microsoft.DataStreamer.UWP
{
    public class MessageDeferral : IDisposable
    {
        private AppServiceDeferral _deferral;

        public MessageDeferral(AppServiceRequestReceivedEventArgs msg)
        {
            _deferral = msg.GetDeferral();
        }

        public void Dispose()
        {
            _deferral?.Complete();
            _deferral = null;
        }
    }
}
