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

using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Windows.ApplicationModel.AppService;
using Windows.ApplicationModel.Background;
using Windows.Foundation.Collections;

namespace Microsoft.DataStreamer.UWP
{
    public sealed class DataStreamerAppService : IBackgroundTask
    {
        private BackgroundTaskDeferral              _backgroundTaskDeferral;
        private AppServiceConnection                _connection;
        private readonly AppServiceStreamingService _service;

        public DataStreamerAppService(AppServiceStreamingService service)
        {
            _service = service;
        }

        public void Run(IBackgroundTaskInstance taskInstance)
        {
            // Get a deferral so that the service isn't terminated.
            _backgroundTaskDeferral = taskInstance.GetDeferral();

            // Associate a cancellation handler with the background task.
            taskInstance.Canceled += OnTaskCanceled;

            // Retrieve the app service connection and set up a listener for incoming app service requests.
            var details = taskInstance.TriggerDetails as AppServiceTriggerDetails;

            _service.Connection = _connection = details.AppServiceConnection;
            _connection.RequestReceived += OnRequestReceived;
            _connection.ServiceClosed += OnServiceClosed;
        }

        private async void OnRequestReceived(AppServiceConnection sender, AppServiceRequestReceivedEventArgs args)
        {
            // Get a deferral because we use an awaitable API below to respond to the message
            // and we don't want this call to get cancelled while we are waiting.
            using(new MessageDeferral(args))
            { 
                ValueSet message = args.Request.Message;
                ValueSet returnData = new ValueSet();

                string command = message["Command"] as string;

                try
                {
                    switch (command)
                    {
                        case "Connect":
                        { 
                            var result = await _service.Connect(message);

                            returnData.Add("Result", result); 
                            await args.Request.SendResponseAsync(returnData);

                            break;
                        }

                        // Handle events/notifications from Data Streamer  
                        case "Event":
                            // Run asynchronously
                            _service.OnEvent(message).FireAndForget();
                            break;

                        case "Close":                       
                            // Run asynchronously
                            _service.Disconnect().FireAndForget();
                            break;

                        default:
                        {
                            var jsonParams = message.ValueOrDefault("Params", "").ToString();
                            var parms = jsonParams == null ? null : JObject.Parse(jsonParams);

                            await _service.OnCommand(command, parms);

                            await args.Request.SendResponseAsync(returnData);
                            break;
                        }
                    }

                }
                catch (Exception e)
                {
                    Debug.WriteLine($"Exception while sending the response : {e.Message}");
                }            
            }        
        }

        private async void OnServiceClosed(AppServiceConnection sender, AppServiceClosedEventArgs args)
        {
            await ResetConnection();
        }

        private async void OnTaskCanceled(IBackgroundTaskInstance sender, BackgroundTaskCancellationReason reason)
        {
            await ResetConnection();
        }    

        private async Task ResetConnection()
        {
            if(_backgroundTaskDeferral != null)
            {
                // Complete the service deferral.
                _backgroundTaskDeferral.Complete();
                _backgroundTaskDeferral = null;
            }

            await _service.Disconnect();

            if(_connection != null)
            {
                _connection.Dispose();
                _connection = null;
            }
        }
    }
}
