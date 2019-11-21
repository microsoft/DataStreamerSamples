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

namespace Microsoft.DataStreamer.Samples.EarthquakeSimulator
{
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();

            this.DataContext = App.ViewModel;
        }

        private void ButtonStartEarthquake_Click(object sender, RoutedEventArgs e)
        {
            try
            { 
                App.Service.StartEarthquake();
            }
            catch(Exception ex)
            {
                Debug.WriteLine($"Exception while attempting to start earthquake: {ex.Message}");
            }
        }

        private void ButtonStopEarthquake_Click(object sender, RoutedEventArgs e)
        {
            try
            { 
                App.Service.StopEarthquake();
            }
            catch(Exception ex)
            {
                Debug.WriteLine($"Exception while attempting to stop earthquake: {ex.Message}");
            }
        }

        private async void ButtonStartRecording_Click(object sender, RoutedEventArgs e)
        {
            try
            { 
                var docs = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

                await App.Service.StartRecording(Path.Combine(docs, "DataStreamer", "Earthquake_" + DateTime.Now.ToString("yyyyMMddhhmmssfff") + ".csv"), DispatchShowError);
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

        private async void ButtonClearOutput_Click(object sender, RoutedEventArgs e)
        {
            try
            { 
                await App.Service.ClearOutput();
            }
            catch(Exception ex)
            {
                Debug.WriteLine($"Exception while attempting clear output: {ex.Message}");
            }  
        }

        private async void buttonStartSeismometer_Click(object sender, RoutedEventArgs e)
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

        private async void buttonStopSeismometer_Click(object sender, RoutedEventArgs e)
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

        private async Task ShowError(string msg)
        {
            var messageDialog = new MessageDialog(msg, "Earthquake Simulator");

            await messageDialog.ShowAsync();
        }

        private async Task DispatchShowError(string msg)
        {
            await this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async ()=>
            { 
                await ShowError(msg);
            });
        }
    }

}
