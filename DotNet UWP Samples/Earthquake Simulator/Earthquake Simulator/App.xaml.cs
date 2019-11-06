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
using System.Diagnostics;
using System.Threading.Tasks;

using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.AppService;
using Windows.ApplicationModel.Background;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

using Microsoft.DataStreamer.UWP;
using System.ComponentModel;
using Windows.UI.Core;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Microsoft.DataStreamer.Samples.EarthquakeSimulator
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : Application
    {
        public static AppServiceConnection DataStreamerConnection;
        private BackgroundTaskDeferral _appServiceDeferral;
        private static App _instance;

        private EarthquakeService _service;

        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
            this.Suspending += OnSuspending;
            _instance = this;

            _service = new EarthquakeService(new EarthquakeRepository());

            this.ViewModel = new EarthQuakeViewModel();
        }

        public void StartEarthquake()
        {
            _service.StartEarthquake(this.ViewModel.PGA, this.ViewModel.Duration);
        }

        public void StopEarthquake()
        {
            _service.StopEarthquake();
        }

        public CoreDispatcher Dispatcher 
        { 
            get {return null;} 
            set {this.ViewModel.Dispatcher = value;}
        }

        public static EarthquakeService StreamerService  => _instance._service;
        public static App               Instance         => _instance;
        public EarthQuakeViewModel      ViewModel        { get; private set; }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
            Frame rootFrame = Window.Current.Content as Frame;

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (rootFrame == null)
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame();

                rootFrame.NavigationFailed += OnNavigationFailed;

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    //TODO: Load state from previously suspended application
                }

                // Place the frame in the current Window
                Window.Current.Content = rootFrame;
            }

            if (e.PrelaunchActivated == false)
            {
                if (rootFrame.Content == null)
                {
                    // When the navigation stack isn't restored navigate to the first page,
                    // configuring the new page by passing required information as a navigation
                    // parameter
                    rootFrame.Navigate(typeof(MainPage), e.Arguments);
                }
                // Ensure the current window is active
                Window.Current.Activate();
            }
        }

        /// <summary>
        /// Invoked when Navigation to a certain page fails
        /// </summary>
        /// <param name="sender">The Frame which failed navigation</param>
        /// <param name="e">Details about the navigation failure</param>
        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();

            deferral.Complete();
        }

        protected override void OnBackgroundActivated(BackgroundActivatedEventArgs args)
        {
            base.OnBackgroundActivated(args);
            IBackgroundTaskInstance taskInstance = args.TaskInstance;
            AppServiceTriggerDetails appService = taskInstance.TriggerDetails as AppServiceTriggerDetails;
            _appServiceDeferral = taskInstance.GetDeferral();
            taskInstance.Canceled += OnAppServicesCanceled;
             _service.Connection = DataStreamerConnection = appService.AppServiceConnection;
            DataStreamerConnection.RequestReceived += OnAppServiceRequestReceived;
            DataStreamerConnection.ServiceClosed += AppServiceConnection_ServiceClosed;
        }

        private void OnAppServicesCanceled(IBackgroundTaskInstance sender, BackgroundTaskCancellationReason reason)
        {
            _appServiceDeferral.Complete();
        }

        /// <summary>
        /// Connection with DataStreamer has been severed
        /// </summary>
        private async void AppServiceConnection_ServiceClosed(AppServiceConnection sender, AppServiceClosedEventArgs args)
        {
            await _service.Disconnect();

            try
            { 
                await this.ViewModel.SetStatus("Not Connected");
                await this.ViewModel.SetAppVersion("");
                await this.ViewModel.SetApiVersion("");
            }
            catch(Exception ex)
            {
            }

            _appServiceDeferral.Complete();
       }

        ///
        private async void OnAppServiceRequestReceived(AppServiceConnection sender, AppServiceRequestReceivedEventArgs args)
        {
            // Get a deferral because we use an awaitable API below to respond to the message
            // and we don't want this call to get cancelled while we are waiting.
            var messageDeferral = args.GetDeferral();
            ValueSet message = args.Request.Message;
            ValueSet returnData = new ValueSet();

            string command = message["Command"] as string;

            try
            {
                switch (command)
                {
                    case "Connect":
                        var result = await _service.Connect();

                        DataStreamerConnection = sender;
                        returnData.Add("Result", result); 
                        await args.Request.SendResponseAsync(returnData);

                        await this.ViewModel.ClearOutput();
                        await this.ViewModel.SetStatus("NotReady");
                        await this.ViewModel.SetApiVersion( message.ValueOrDefault("Version", "0.0").ToString());
                        await this.ViewModel.SetAppVersion( message.ValueOrDefault("AppVersion", "0.0").ToString());

                        break;

                    case "Read":
                        //Receive data from Data Streamer and show it in UWP Application
                        returnData = await ReadDataAsync();
                        break;
                    case "Write":
                        //Send data to Data Streamer
                        await WriteDataAsync(message["Data"] as string);
                        break;

                    // Handle events/notifications from Data Streamer  
                    case "Event":
                    { 
                        var eventName = message["EventName"]?.ToString() ?? "";

                        if(eventName == "OnStatusUpdate")
                        {
                            if(this.ViewModel.Status != "Not Connected")
                            { 
                                var status = message["Status"]?.ToString() ?? "";

                                await this.ViewModel.SetStatus(status);
                                await this.ViewModel.AppendOutputLine("Status: " + status);
                            }
                        }
                        else
                           await this.ViewModel.AppendOutputLine(" Event: " + eventName);

                        break;
                    }

                    case "Close":
                        
                        await this.ViewModel.SetStatus("Not Connected");
                        await this.ViewModel.SetApiVersion("");
                        await this.ViewModel.SetAppVersion("");
 
                        break;

                    default:
                        {
                            returnData.Add("Status", "Fail: unknown command");
                            await args.Request.SendResponseAsync(returnData);
                            break;
                        }
                }

            }
            catch (Exception e)
            {
                Debug.WriteLine($"Exception while sending the response : {e.Message}");
            }
            finally
            {
                // Complete the deferral so that the platform knows that we're done responding to the app service call.
                // Note for error handling: this must be called even if SendResponseAsync() throws an exception.
                messageDeferral.Complete();
            }
        }

        //Send data to Data Streamer
        private async Task WriteDataAsync(string data)
        {
            if (DataStreamerConnection == null)
            {
                throw new ObjectDisposedException(nameof(DataStreamerConnection));
            }

            ValueSet message = new ValueSet();
            AppServiceResponse response = null;

            message.Add("Command", "Write");
            message.Add("Data", data);

            response = await DataStreamerConnection.SendMessageAsync(message);

            if (response.Status != AppServiceResponseStatus.Success)
            {
                Debug.WriteLine($"Failed to send data: {response.Status.ToString()}");
            }
        }

        //Receive data from Data Streamer and return it to UWP Application
        private async Task<ValueSet> ReadDataAsync()
        {
            if (DataStreamerConnection == null)
            {
                throw new ObjectDisposedException(nameof(DataStreamerConnection));
            }

            // Let's ask the DataStreamer for data
            ValueSet message = new ValueSet();
            AppServiceResponse response = null;

            message.Add("Command", "Read");
            response = await DataStreamerConnection.SendMessageAsync(message);

            if (response?.Status != AppServiceResponseStatus.Success)
            {
                throw new Exception($"Failed to send data: {response.Status.ToString()}");
            }

            return response.Message;
        }
    }}
