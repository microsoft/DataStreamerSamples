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
using System.IO;
using System.Threading.Tasks;

using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Popups;

namespace Microsoft.DataStreamer.Samples.SensorSimulator
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();

            App.ViewModel.Dispatcher = this.Dispatcher;
            this.DataContext = App.ViewModel;
        }

        #region Button Handlers

        private async void buttonStartStreaming_Click(object sender, RoutedEventArgs e)
        {
            try
            { 
                await App.Service.StartData();
            }
            catch(Exception ex)
            {
                Debug.WriteLine($"Exception while attempting to start data streaming: {ex.Message}");
            }        
        }

        private async void buttonStopStreaming_Click(object sender, RoutedEventArgs e)
        {
            try
            { 
                await App.Service.StopData();
            }
            catch(Exception ex)
            {
                Debug.WriteLine($"Exception while attempting to stop data streaming: {ex.Message}");
            }         
        }
        
        private async void ButtonStartRecording_Click(object sender, RoutedEventArgs e)
        {
            try
            { 
                var docs = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

                await App.Service.StartRecording(Path.Combine(docs, "DataStreamer", "Sensors_" + DateTime.Now.ToString("yyyyMMddhhmmssfff") + ".csv"), DispatchShowError);
            }
            catch(Exception ex)
            {
                Debug.WriteLine($"Exception while attempting to start recording: {ex.Message}");
            }
        }

        private async void ButtonStopRecording_Click(object sender, RoutedEventArgs e)
        {
            try
            { 
                await App.Service.StopRecording(DispatchShowError);
            }
            catch(Exception ex)
            {
                Debug.WriteLine($"Exception while attempting to stop recording: {ex.Message}");
            } 
        }

        private async void ButtonUpdateManifest_Click(object sender, RoutedEventArgs e)
        {
            try
            { 
                await App.Service.UpdateManifest(DispatchShowError);
            }
            catch(Exception ex)
            {
                Debug.WriteLine($"Exception while attempting to update manifest: {ex.Message}");
            } 
        }

        private async void ButtonReset_Click(object sender, RoutedEventArgs e)
        {
            try
            { 
                await App.Service.Reset();
            }
            catch(Exception ex)
            {
                Debug.WriteLine($"Exception while attempting to reset Data Streamer: {ex.Message}");
            }
        }

        private void ButtonClearOutput_Click(object sender, RoutedEventArgs e)
        {
            try
            { 
                App.Service.ClearOutput();
            }
            catch(Exception ex)
            {
                Debug.WriteLine($"Exception while attempting clear output: {ex.Message}");
            }  
        }

        #endregion

        #region Message Box

        private async Task ShowError(string msg)
        {
            var messageDialog = new MessageDialog(msg, "Sensor Simulator");

            await messageDialog.ShowAsync();
        }

        private async Task DispatchShowError(string msg)
        {
            await this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async ()=>
            { 
                await ShowError(msg);
            });
        }

        #endregion
    }
}
