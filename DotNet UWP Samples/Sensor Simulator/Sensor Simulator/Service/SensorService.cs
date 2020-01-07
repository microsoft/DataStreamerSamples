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

using Microsoft.DataStreamer.UWP;
using Windows.UI.Core;

namespace Microsoft.DataStreamer.Samples.SensorSimulator
{
    public class SensorService : AppServiceStreamingService
    {
        private readonly SensorRepository _repo;
        private readonly SensorViewModel _viewModel;

        public SensorService(SensorRepository repository, SensorViewModel viewModel) : base(repository)
        {
            _repo = repository;
            _viewModel = viewModel;
        }

        public SensorViewModel ViewModel  => _viewModel;
        public CoreDispatcher  Dispatcher { set { _viewModel.Dispatcher = value; }}

        public void ClearOutput()
        {
            _viewModel.ClearOutput();
        }

        public async override Task<string> Connect(IDictionary<string, object> message)
        {
            var result = await base.Connect(message);

            _viewModel.ClearOutput();
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

        public override Task OnCommand(string command, dynamic parms)
        {
            if(command == "Measurement System")
            {
                try
                { 
                    var val = parms.MeasurementSystem;

                    this.ViewModel.IsMetric = val.ToString() == "Metric";
                }
                catch
                {

                }
            }

            return Task.CompletedTask;
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

        public override async Task UpdateManifest(Func<string, Task> fnOnError = null)
        {
            _repo.UpdatePending();

            await SendRPCCommand("SetManifest", new { Manifest = _repo.ActiveManifest }, fnOnError);
        }
    }

}
