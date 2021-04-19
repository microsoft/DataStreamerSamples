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

using Windows.ApplicationModel.AppService;
using Windows.Foundation.Collections;

using System.Diagnostics;
using System.Xml.XPath;
using System.Text.Json;
using System.Collections;
using System.ComponentModel;

namespace Microsoft.DataStreamer.UWP
{
    /// <summary>
    /// A streaming service for a UWP AppService
    /// </summary>
    public abstract class AppServiceStreamingService : StreamingService
    {
        private int _lastId = 0;

        public AppServiceStreamingService(IStreamingRepository repository) : base(repository)
        {
        }

        public AppServiceConnection Connection { get; set; }

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

                await this.Connection.SendMessageAsync(message);
            }
            catch (Exception ex)
            {
                _ = ex.Message;
            }
        }

        public override async Task StartData()
        {
            await base.StartData();
            await SendRPCCommand("StartData");
        }

        public override async Task StopData(bool disconnecting = false)
        {
            await base.StopData();

            if(!disconnecting)
                await SendRPCCommand("StopData");
        }
        
        public override async Task StartRecording(string fileName, Func<string, Task> fnOnError = null)
        {
            await SendRPCCommand("StartRecording", new { path = fileName }, fnOnError);
        }

        public override async Task StopRecording(Func<string, Task> fnOnError = null)
        {
            await SendRPCCommand("StopRecording", null, fnOnError);
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

        protected async Task SendRPCCommand(string command, object oParams = null, Func<string, Task> fnOnError = null)
        {
            var cmd = new Command
            {
                Id = (++_lastId).ToString(),
                Name = command
            };

            if (oParams != null)
                cmd.Params = oParams;

            var json = JsonSerializer.Serialize(cmd);

            // Create a message to send to Data Streamer
            var message = new ValueSet
            {
                { "Command", command },
                    
                // JSON RPC 2.0 payload
                { "Data", json }
            };

            // Run asynchronously
            Task.Run( async()=>
            {
                try
                { 
                    var result  = await this.Connection.SendMessageAsync(message);
                    var dParams = new Dictionary<string, object>(result.Message);

                    if(dParams != null && result.Message.ContainsKey("Data"))
                    {
                        var jsonResult = result.Message["Data"].ToString();
                        var cmdResult  = JsonSerializer.Deserialize<CommandResult>(jsonResult);

                        if(cmdResult.Error != null)
                        { 
                            Debug.WriteLine(cmdResult.Error.Message);

                            if(fnOnError != null)
                                await fnOnError(cmdResult.Error.Message);
                        }
                    }
                }
                catch(Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }

            }).FireAndForget();

            await Task.CompletedTask;
        }

        /// <summary>
        /// Serialize as a JSON RPC 2.0 command. See https://www.jsonrpc.org/specification
        /// </summary>
        public class Command
        {
            public string Version { get; set; } = "2.0";

            public string Id { get; set; }

            public string Name { get; set; }

            public Object Params { get; set; }
        }

       /// <summary>
        /// Serialize as a JSON RPC 2.0 result set. See https://www.jsonrpc.org/specification
        /// </summary>
        public class CommandResult
        {
            public string Version { get; set; } = "2.0";

            public string Id { get; set; }

            public string Result { get; set; }

            public CommandError Error { get; set; }
        }

        public class CommandError
        {
            public string Code { get; set; }

            public string Message { get; set; }
        }    
    }
}
