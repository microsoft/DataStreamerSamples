using System;
using System.Collections.Generic;
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

        private void ButtonStartData_Click(object sender, RoutedEventArgs e)
        {
            App.Instance.StartEarthquake();
        }

        private void ButtonStopData_Click(object sender, RoutedEventArgs e)
        {
            App.Instance.StopEarthquake();
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
                await Task.CompletedTask;
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
                await Task.CompletedTask;
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
                await Task.CompletedTask;
            }
        }

        private async void ButtonClearOutput_Click(object sender, RoutedEventArgs e)
        {
            try
            { 
                await App.Instance.ViewModel.SetOutputLines("");
            }
            catch(Exception ex)
            {
                await Task.CompletedTask;
            }  
        }
        
        private async void ButtonReady_Click(object sender, RoutedEventArgs e)
        {
            try
            { 
                await App.StreamerService.Ready();
            }
            catch(Exception ex)
            {
                await Task.CompletedTask;
            }
        }        
        
        private async void ButtonNotReady_Click(object sender, RoutedEventArgs e)
        {
            try
            { 
                await App.StreamerService.NotReady();
            }
            catch
            {

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
                await Task.CompletedTask;
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
                await Task.CompletedTask;
            }         
        }
    }

}
