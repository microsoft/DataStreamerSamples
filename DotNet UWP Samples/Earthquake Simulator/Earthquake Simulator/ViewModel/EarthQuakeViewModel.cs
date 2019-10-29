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
            private string _outputLines;

            public EarthQuakeViewModel()
            {
            }                           
            
            public double PGA         { get; set; }
            public double Duration    { get; set; }
            public string OutputLines => _outputLines; 
               
            public async Task SetOutputLines(string lines)
            { 
                _outputLines = lines; 

                await OnPropertyChanged("OutputLines"); 
            }            
            
            public async Task AppendOutputLine(string val)
            { 
                var outputLines = _outputLines + val + "\r\n"; 
                var dispatcher  = this.Dispatcher;

                await dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                {
                    await this.SetOutputLines(outputLines);

                });
            }
    }
}
