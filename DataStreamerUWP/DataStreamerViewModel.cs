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
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Windows.UI.Core;

namespace Microsoft.DataStreamer.UWP
{
    public abstract class DataStreamerViewModel : INotifyPropertyChanged
    {
        private string _dataStreamerApiVersion = "";
        private string _dataStreamerAppVersion = "";
        private string _dataStreamerStatus     = "Not Connected";

        public DataStreamerViewModel()
        {

        }

        public string DataStreamerApiVersion => _dataStreamerApiVersion;
        public string DataStreamerAppVersion => _dataStreamerAppVersion;
        public string DataStreamerStatus     => _dataStreamerStatus;
        public bool   IsReady                => _dataStreamerStatus == "Ready";
        public bool   IsStreaming            => _dataStreamerStatus == "Reading";
        public bool   IsRecording            => _dataStreamerStatus == "Recording";

        public async Task SetDataStreamerApiVersion(string val)
        { 
            _dataStreamerApiVersion = val;
            await OnPropertyChanged("DataStreamerApiVersion"); 
        }            
            
        public async Task SetDataStreamerAppVersion(string val)
        { 
            _dataStreamerAppVersion = val;
            await OnPropertyChanged("DataStreamerAppVersion"); 
        } 
                    
        public async Task SetDataStreamerStatus(string val)
        { 
            _dataStreamerStatus = val;
            await OnPropertyChanged("DataStreamerStatus"); 
            await OnPropertyChanged("IsReady"); 
            await OnPropertyChanged("IsStreaming"); 
            await OnPropertyChanged("IsRecording"); 
        } 
        
        public CoreDispatcher Dispatcher { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        protected async Task OnPropertyChanged(string name)
        { 
            var dispatcher = this.Dispatcher;

            if(dispatcher != null)
            { 
                await dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name)); 
                });
            }
        } 
    }
}
