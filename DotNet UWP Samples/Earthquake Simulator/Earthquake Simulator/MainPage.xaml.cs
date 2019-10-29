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
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Microsoft.DataStreamer.Samples.EarthquakeSimulator
{
    public sealed partial class MainPage : Page
    {
        // default state is to send data

        public MainPage()
        {
            this.InitializeComponent();

            this.DataContext = App.Instance.ViewModel;
            App.Instance.Dispatcher = this.Dispatcher;
        }

        private void ButtonStartEarthquake_Click(object sender, RoutedEventArgs e)
        {
            try
            { 
                App.Instance.StartEarthquake();
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
                App.Instance.StopEarthquake();
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

                await App.StreamerService.StartRecording(Path.Combine(docs, "DataStreamer", "Earthquake.csv"));
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
                await App.StreamerService.StopRecording();
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
                await App.StreamerService.Reset();
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
                await App.Instance.ViewModel.ClearOutput();
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
                await App.StreamerService.StartData();
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
                await App.StreamerService.StopData();
            }
            catch(Exception ex)
            {
                Debug.WriteLine($"Exception while attempting to stop data streaming: {ex.Message}");
            }         
        }
    }

}
