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

using Windows.UI.Core;

using Microsoft.DataStreamer.UWP;

namespace Microsoft.DataStreamer.Samples.EarthquakeSimulator
{
    public class EarthQuakeViewModel : DataStreamerViewModel
    {
            private StringBuilder _outputLines = new StringBuilder();

            public EarthQuakeViewModel()
            {
            }                           
            
            public double        PGA         { get; set; } = 5d;
            public double        Duration    { get; set; } = 10d;
            public StringBuilder OutputLines => _outputLines; 
                           
            public async Task ClearOutput()
            { 
               _outputLines.Clear();

                await OnPropertyChanged("OutputLines"); 
            }            
            
            public async Task AppendLine(string line)
            { 
               _outputLines.AppendLine(line);

                await OnPropertyChanged("OutputLines"); 
            }            
            
            public async Task AppendOutputLine(string val)
            { 
                var dispatcher  = this.Dispatcher;

                await dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                {
                    await this.AppendLine(val);

                });
            }
    }
}
