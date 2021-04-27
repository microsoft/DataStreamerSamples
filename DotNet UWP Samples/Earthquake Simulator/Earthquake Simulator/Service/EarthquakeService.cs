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
using System.Threading;
using System.Threading.Tasks;

using Windows.Foundation.Collections;
using Windows.UI.Core;

using Microsoft.DataStreamer.UWP;

namespace Microsoft.DataStreamer.Samples.EarthquakeSimulator
{
    public class EarthquakeService : AppServiceStreamingService
    {
        private readonly EarthquakeRepository _repo;
        private readonly EarthQuakeViewModel _viewModel;

        public EarthquakeService(EarthquakeRepository repository, EarthQuakeViewModel viewModel) : base(repository)
        {
            _repo = repository;
            _viewModel = viewModel;
        }

        public EarthQuakeViewModel ViewModel  => _viewModel;
        public CoreDispatcher      Dispatcher { set { _viewModel.Dispatcher = value; }}

        public void StartEarthquake()
        {
            _repo.StartEarthquake(_viewModel.PGA, _viewModel.Duration);
        }

        public void StopEarthquake()
        {
            _repo.StopEarthquake();
        }

        public async Task ClearOutput()
        {
            await _viewModel.ClearOutput();
        }

        public async override Task<string> Connect(IDictionary<string, object> message)
        {
            var result = await base.Connect(message);

            await _viewModel.ClearOutput();
            _viewModel.SetStatus("NotReady");
            _viewModel.SetApiVersion(message.ValueOrDefault("Version", "0.0").ToString());
            _viewModel.SetAppVersion(message.ValueOrDefault("AppVersion", "0.0").ToString());

            return result;
        }

        public override async Task OnEvent(IDictionary<string, object> message)
        {
            var eventName = message["EventName"]?.ToString() ?? "";

            if(eventName == "OnStatusUpdate")
            {
                if(_viewModel.Status != "Not Connected")
                { 
                    var status = message["Status"]?.ToString() ?? "";

                    _viewModel.SetStatus(status);
                    await _viewModel.AppendOutputLine("Status: " + status);
                }
            }
            else
                await _viewModel.AppendOutputLine(" Event: " + eventName);
        }

        public async override Task Disconnect()
        {
            try
            { 
                await base.Disconnect();
            }
            catch(Exception ex)
            {
                _ = ex.Message;
            }
            finally
            { 
                this.Connection = null;

                _viewModel.SetStatus("Not Connected");
                _viewModel.SetAppVersion("");
                _viewModel.SetApiVersion("");
            }
        }

		public override Task UpdateManifest(Func<string, Task> fnOnError = null)
		{
			throw new NotImplementedException();
		}

		public override Task OnCommand(string command, dynamic parms)
		{
			throw new NotImplementedException();
		}
	}
}
