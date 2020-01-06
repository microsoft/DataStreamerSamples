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
using System.Threading.Tasks;

using Windows.UI.Core;

using Microsoft.DataStreamer.UWP;

namespace Microsoft.DataStreamer.Samples.SensorSimulator
{
    public class SensorViewModel : DataStreamerViewModel
    {
        private StringBuilder _outputLines = new StringBuilder();
        private readonly SensorRepository _repository;
        private bool _metric = false;

        public SensorViewModel(SensorRepository repository)
        {
            _repository = repository;
        }                           
            
        public bool IsMetric  
        { 
            get { return _metric; }
            set { _metric = value; OnPropertyChanged(nameof(IsMetric)); OnPropertyChanged(nameof(IsStandard));}
        }

        public bool IsStandard  
        { 
            get { return !_metric; }
            set { _metric = !value; OnPropertyChanged(nameof(IsStandard));  OnPropertyChanged(nameof(IsMetric)); }
        }

        public bool AngularSensors  
        { 
            get { return _repository.AngularSensors.PendingActive; }
            set { _repository.AngularSensors.PendingActive = value; OnPropertyChanged(nameof(AngularSensors)); }
        }

        public bool Accelerometer  
        { 
            get { return _repository.Accelerometer.PendingActive; }
            set { _repository.Accelerometer.PendingActive = value; OnPropertyChanged(nameof(Accelerometer)); }
        }

        public bool Gyrometer  
        { 
            get { return _repository.Gyrometer.PendingActive; }
            set { _repository.Gyrometer.PendingActive = value; OnPropertyChanged(nameof(Gyrometer)); }
        }

        public bool Inclinometer  
        { 
            get { return _repository.Inclinometer.PendingActive; }
            set { _repository.Inclinometer.PendingActive = value; OnPropertyChanged(nameof(Inclinometer)); }
        }

        public bool Environmental  
        { 
            get { return _repository.Environmental.PendingActive; }
            set { _repository.Environmental.PendingActive = value; OnPropertyChanged(nameof(Environmental)); }
        }

        public bool Illuminance  
        { 
            get { return _repository.Illuminance.PendingActive; }
            set { _repository.Illuminance.PendingActive = value; OnPropertyChanged(nameof(Illuminance)); }
        }

        public bool Altimeter  
        { 
            get { return _repository.Altimeter.PendingActive; }
            set { _repository.Altimeter.PendingActive = value; OnPropertyChanged(nameof(Altimeter)); }
        }

        public bool Barometer  
        { 
            get { return _repository.Barometer.PendingActive; }
            set { _repository.Barometer.PendingActive = value; OnPropertyChanged(nameof(Barometer)); }
        }

        public bool Compass  
        { 
            get { return _repository.Compass.PendingActive; }
            set { _repository.Compass.PendingActive = value; OnPropertyChanged(nameof(Compass)); }
        }

        public bool MagneticNorth  
        { 
            get { return _repository.MagneticNorth.PendingActive; }
            set { _repository.MagneticNorth.PendingActive = value; OnPropertyChanged(nameof(MagneticNorth)); }
        }

        public bool TrueNorth  
        { 
            get { return _repository.TrueNorth.PendingActive; }
            set { _repository.TrueNorth.PendingActive = value; OnPropertyChanged(nameof(TrueNorth)); }
        }

        public StringBuilder OutputLines => _outputLines; 
                           
        public void ClearOutput()
        { 
            _outputLines.Clear();

            OnPropertyChanged("OutputLines"); 
        }            
            
        public void AppendLine(string line)
        { 
            _outputLines.AppendLine(line);

             OnPropertyChanged("OutputLines"); 
        }            
            
        public async Task AppendOutputLine(string val)
        { 
            var dispatcher  = this.Dispatcher;

            await dispatcher.RunAsync(CoreDispatcherPriority.Normal, ()=>
            {
                this.AppendLine(val);
            });
        }
    }
}
