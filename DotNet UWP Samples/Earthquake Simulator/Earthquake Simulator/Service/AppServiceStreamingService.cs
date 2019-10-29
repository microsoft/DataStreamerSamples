using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Windows.ApplicationModel.AppService;
using Windows.Foundation.Collections;

using DataStreamer.App.Core;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

using Microsoft.DataStreamer.UWP;

namespace Microsoft.DataStreamer.Samples.EarthquakeSimulator
{
    public class AppServiceStreamingService : StreamingService
    {
        private int _lastId = 0;

        public AppServiceStreamingService(IStreamingRepository repository) : base(repository)
        {
        }

        public override Task UpdateManifest()
        {
            throw new NotImplementedException();
        }

        protected override async Task SendData(string data)
        {
            try
            {
                // Create a message to send to Data Streamer
                var message = new ValueSet
                {
                    // When sending data to Data Streamer we always use a "Write" command 
                    { "Command", "Write" },
                    
                    // This is the data we will send to Data Streamer. The data should always be a string.
                    { "Data", data }
                };

                await App.DataStreamerConnection.SendMessageAsync(message);
            }
            catch (Exception)
            {
                App.IsConnectedToDataStreamerAppService = false;
            }
        }

        public override async Task StartData()
        {
            await base.StartData();
            await SendRPCCommand("StartData");
        }

        public override async Task StopData()
        {
            await base.StopData();
            await SendRPCCommand("StopData");
        }
        
        public override async Task StartRecording(string fileName)
        {
            await SendRPCCommand("StartRecording", new { path = fileName });
        }

        public override async Task StopRecording()
        {
            await SendRPCCommand("StopRecording");
        }

        public override async Task Reset()
        {
            await SendRPCCommand("ResetDataIn");
        }

        public override async Task Ready()
        {
            await SendRPCCommand("SetStatus", new {status = "appready"} );
        }

        public override async Task NotReady()
        {
            await SendRPCCommand("SetStatus", new {status = "appnotready"} );
        }

        private async Task SendRPCCommand(string command, object oParams = null)
        {
            var cmd = new Command
            {
                Id = (++_lastId).ToString(),
                Name = command
            };

            if(oParams != null)
                cmd.Params = JObject.FromObject(oParams);

            // Create a message to send to Data Streamer
            var message = new ValueSet
            {
                { "Command", command },
                    
                // JSON RPC 2.0 payload
                { "Data", JsonConvert.SerializeObject(cmd) }
            };

            await App.DataStreamerConnection.SendMessageAsync(message);
        }

        public class Command
        {
            [JsonProperty("jsonrpc")]
            public string Version { get; set; } = "2.0";

            [JsonProperty("id")]
            public string Id { get; set; }

            [JsonProperty("method")]
            public string Name { get; set; }

            [JsonProperty("params")]
            public JObject Params { get; set; }
        }

    }
}
