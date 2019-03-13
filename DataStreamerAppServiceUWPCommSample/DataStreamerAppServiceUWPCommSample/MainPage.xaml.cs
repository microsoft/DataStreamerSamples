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
using System.Threading.Tasks;
using Windows.ApplicationModel.AppService;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace DataStreamerAppServiceUWPCommSample
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class MainPage : Page
    {
		// default state is to send data
		private bool _shouldSendData = true;

		public MainPage()
        {
            this.InitializeComponent();

			radioButtonSend.IsChecked = _shouldSendData;
			textBox.IsReadOnly = false;
			radioButtonSend.IsChecked = false;
			radioButtonSend.IsChecked = false;
		}

		private void RadioButtonSend_Checked(object sender, RoutedEventArgs e)
		{
			radioButtonSend.IsChecked = true;
			radioButtonReceive.IsChecked = false;
			_shouldSendData = true;
			textBox.IsReadOnly = false;
			textBox.IsEnabled = true;
			this.textBlockStatus.Text = string.Empty;
			this.textBox.Text = string.Empty;
		}

		private void RadioButtonReceive_Checked(object sender, RoutedEventArgs e)
		{
			radioButtonSend.IsChecked = false;
			radioButtonReceive.IsChecked = true;
			_shouldSendData = false;
			textBox.IsReadOnly = true;
			textBox.IsEnabled = false;
			this.textBlockStatus.Text = string.Empty;
			this.textBox.Text = string.Empty;
		}

		private async void ButtonSubmit_Click(object sender, RoutedEventArgs e)
		{
			if(_shouldSendData)
			{
				// Send data to data streamer
				await SendDataAsync();
			}
			else
			{
				// Receive data from data streamer
				await ReceiveDataAsync();
			}
		}

		private async Task ReceiveDataAsync()
		{
			try
			{
				var message = new ValueSet
					{
						//Send Read command to request data from Data Streamer
						{ "Command", "Read" }
					};

				AppServiceResponse response = await App.DataStreamerConnection.SendMessageAsync(message);

				if (response.Message.Count > 0)
				{
					//Grab data sent from Data Streamer and display it in the textBox
					textBox.Text = response.Message["ReadData"].ToString();
					textBlockStatus.Text = "Received";
				}
				else
				{
					textBlockStatus.Text = response.Status.ToString();
				}
			}
			catch (Exception ex)
			{
				textBlockStatus.Text = ex is NullReferenceException ? "Excel -> DataStreamer -> Connect a Device -> Connect to DataStreamerAppServiceUWPCommSample" : ex.Message;
				App.IsConnectedToDataStreamerAppService = false;
			}
		}

		private async Task SendDataAsync()
		{
			try
			{
				//Create a message to send to Data Streamer
				var message = new ValueSet
					{
						//When sending data to Data Streamer we always use a "Write" command 
						{ "Command", "Write" }, 
						//This is the data we will send to Data Streamer. The data should always be a string.
						{ "Data", textBox.Text }
					};

				AppServiceResponse response = await App.DataStreamerConnection.SendMessageAsync(message);

				textBlockStatus.Text = response.Message.Count > 0 ? response.Message["Result"].ToString() : response.Status.ToString();
			}
			catch (Exception ex)
			{
				textBlockStatus.Text = ex is NullReferenceException ? "Excel -> DataStreamer -> Connect a Device -> Connect to DataStreamerAppServiceUWPCommSample" : ex.Message;
				App.IsConnectedToDataStreamerAppService = false;
			}
		}
	}
}
