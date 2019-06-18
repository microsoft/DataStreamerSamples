//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// This code is licensed under the MIT License (MIT).
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************
using System;
using System.Diagnostics;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.AppService;
using Windows.ApplicationModel.Background;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace AppServiceHost
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : Application
    {
        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
            this.Suspending += OnSuspending;
        }

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
            //TODO: Save application state and stop any background activity
            deferral.Complete();
        }

        private BackgroundTaskDeferral _dataConnectionDeferral;
        private BackgroundTaskDeferral _excelAppServiceDeferral;
        private AppServiceConnection _dataConnection;
        private static AppServiceConnection _excelConnection;

        protected override void OnBackgroundActivated(BackgroundActivatedEventArgs args)
        {
            base.OnBackgroundActivated(args);

            IBackgroundTaskInstance taskInstance = args.TaskInstance;
            var deferral = taskInstance.GetDeferral();

            AppServiceTriggerDetails appService = taskInstance.TriggerDetails as AppServiceTriggerDetails;
            Debug.WriteLine($"appService.CallerPackageFamilyName: {appService.CallerPackageFamilyName}");  // blank if Excel
            if ((string.IsNullOrEmpty(appService.CallerPackageFamilyName)) && (_excelConnection==null))
            {
                if (_excelConnection != null)
                {
                    ExcelResetConnection();
                }

                _excelConnection = appService.AppServiceConnection;
                _excelConnection.RequestReceived += ExcelOnAppServiceRequestReceived;
                _excelConnection.ServiceClosed += ExcelConnectionServiceConnection_ServiceClosed;
                _excelAppServiceDeferral = deferral;
                taskInstance.Canceled += ExcelOnAppServicesCanceled;

                Debug.WriteLine($"Connecting excel Service {_excelConnection.GetHashCode()} {"Excel"}");
            }
            else
            {
                if (_dataConnection != null)
                {
                    DataConnectionResetConnection();
                }
                _dataConnection = appService.AppServiceConnection;
                _dataConnection.RequestReceived += DataServiceRequestReceived;
                _dataConnection.ServiceClosed += DataServiceConnection_ServiceClosed;
                _dataConnectionDeferral = deferral;
                taskInstance.Canceled += DataOnAppServicesCanceled;

                Debug.WriteLine($"Connecting Data Service {_dataConnection.GetHashCode()} {appService.CallerPackageFamilyName}");
            }
        }
        private async void ExcelOnAppServiceRequestReceived(AppServiceConnection sender, AppServiceRequestReceivedEventArgs args)
        {
            var messageDeferral = args.GetDeferral();
            var request = args.Request;
            var m = request.Message;
            object command;
            m.TryGetValue("Command", out command);

            try
            {
                if (command as string == "Connect")
                {
                    object role;
                    m.TryGetValue("Role", out role);

                    if ((role as string) == "DataStreamer") 
                    {
                        _excelConnection = sender;
                        Debug.WriteLine($"Connecting Excel: {sender.GetHashCode()}");
                    }
                    var response = new ValueSet();
                    response.Add("Response", "OK");
                    await request.SendResponseAsync(response);

                    SendStatusAsync();
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

        private void ExcelOnAppServicesCanceled(IBackgroundTaskInstance sender, BackgroundTaskCancellationReason reason)
        {
            Debug.WriteLine($"ExcelOnAppServicesCanceled {reason}");
            if (_excelConnection != null)
            {
                ExcelResetConnection();
            }
        }
        private void DataOnAppServicesCanceled(IBackgroundTaskInstance sender, BackgroundTaskCancellationReason reason)
        {
            Debug.WriteLine($"DataConnectionOnAppServicesCanceled {reason}");
            if (_dataConnection != null)
            {
                DataConnectionResetConnection();
            }
        }

        private void ExcelResetConnection()
        {
            if (_excelConnection != null)
            {
                if (_excelAppServiceDeferral != null)
                {
                    _excelAppServiceDeferral.Complete();
                    _excelAppServiceDeferral = null;
                }
                _excelConnection.Dispose();
                _excelConnection = null;
            }
            SendStatusAsync();
        }

        private void DataConnectionResetConnection()
        {
            if (_dataConnection != null)
            {
                if (_dataConnectionDeferral != null)
                {
                    _dataConnectionDeferral.Complete();
                    _dataConnectionDeferral = null;
                }
                _dataConnection.Dispose();
                _dataConnection = null;
            }
            SendStatusAsync();
        }
        private void DataServiceConnection_ServiceClosed(AppServiceConnection sender, AppServiceClosedEventArgs args)
        {
            Debug.WriteLine($"DataConnectionServiceConnection_ServiceClosed {args.ToString()}");
            if (_dataConnection != null)
            {
                DataConnectionResetConnection();
            }
        }
        private void ExcelConnectionServiceConnection_ServiceClosed(AppServiceConnection sender, AppServiceClosedEventArgs args)
        {
            Debug.WriteLine($"ExcelConnectionServiceConnection_ServiceClosed {args.ToString()}");
            if (_excelConnection != null)
            {
                ExcelResetConnection();
            }
        }

        private async void DataServiceRequestReceived(AppServiceConnection sender, AppServiceRequestReceivedEventArgs args)
        {
            Debug.WriteLine($"DataServiceRequestReceived");

            AppServiceResponse result;
            AppServiceResponseStatus resultStatus;
            var messageDeferral = args.GetDeferral();
            var request = args.Request;
            var m = request.Message;
            object command;
            try
            {
                m.TryGetValue("Command", out command);
                switch (command as string)
                {
                    case "Connect":
                        object role;
                        m.TryGetValue("Role", out role);
                        if (role as string == "DataStreamerConnect") // Client
                        { 
                            _dataConnection = sender;
                        }
                        var response = new ValueSet();
                        response.Add("Response", "OK");
                        await request.SendResponseAsync(response);
                        SendStatusAsync();
                        break;
                    case "Write":
                        object data;
                        if (m.TryGetValue("Data", out data))
                        {
                            Debug.WriteLine($"Write data:{data}");
                            if (_excelConnection != null)
                            {
                                result = await _excelConnection.SendMessageAsync(m);

                                if (result.Status != AppServiceResponseStatus.Success)
                                {
                                    Debug.WriteLine($"Failed to send data: {result.Status.ToString()}");
                                }
                                else
                                {
                                    Debug.WriteLine($"Sent: {data as string}");
                                }
                            }
                            else
                            {
                                Debug.WriteLine($"Failed to send data: no Excel Connection exists");
                            }
                        }
                        break;
                    case "Read":
                        var msg = new ValueSet();
                        AppServiceResponse res = null;
                        msg["Command"] = "Read";

                        if (_excelConnection != null)
                        {
                            res = _excelConnection.SendMessageAsync(msg).AsTask().Result; ;
                            if (res.Status == AppServiceResponseStatus.Success)
                            {
                                Debug.WriteLine($"Data recieved from Excel: {res.Message.Count}");
                                if (_dataConnection != null)
                                {
                                    //var clientRes = _dataConnection.SendMessageAsync(res.Message).AsTask().Result;
                                    var clientRes = await request.SendResponseAsync(res.Message);

                                    if (clientRes != AppServiceResponseStatus.Success)
                                    {
                                        Debug.WriteLine($"Failed to send read data to client: {clientRes.ToString()}");
                                    } else
                                    {
                                        Debug.WriteLine($"Data sent to client: {res.Message.Count}");
                                    }
                                }
                            }
                            else
                            {
                                Debug.WriteLine($"Failed to send data: {res.Status.ToString()}");
                            }
                        }
                        break;
                    case "Status":
                        var message = new ValueSet();
                        message.Add("Command", "Status");
                        message.Add("Data", String.Format("Client:{0},Excel:{1}", _dataConnection != null, _excelConnection != null));
                        resultStatus = request.SendResponseAsync(message).AsTask().Result;
                        if (resultStatus != AppServiceResponseStatus.Success)
                        {
                            Debug.WriteLine($"Failed to send data: {resultStatus}");
                        }
                        break;
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

        private async void SendStatusAsync()
        {
            var message = new ValueSet();
            message.Add("Command", "Status");
            message.Add("Data", String.Format("Client:{0},Excel:{1}", _dataConnection != null, _excelConnection != null));
            if (_dataConnection != null)
            {
                var result = await _dataConnection.SendMessageAsync(message);
                if (result.Status != AppServiceResponseStatus.Success)
                {
                    Debug.WriteLine($"Failed to send data: {result}");
                }
            }
        }
    }
}
