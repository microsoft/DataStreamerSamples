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
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Windows.ApplicationModel.AppService;
using Windows.Foundation.Collections;

using Microsoft.DataStreamer.UWP;
using System.Globalization;

namespace Microsoft.DataStreamer.Samples.EarthquakeSimulator
{
    public class EarthquakeRepository : IStreamingRepository
    {
        private string[]        _backgroundData;
        private string[]        _earthQuakedata;
        private int             _currentIndex = 0;
        private double          _pga          = 0d;
        private double          _duration     = 0d;
        private DateTime        _start        = DateTime.MinValue;
        private readonly object _lock         = new object();
        private const double    MaxSampleData = 1.94d;

        public EarthquakeRepository()
        {
            // Background seismic data
            _backgroundData = LoadSeismicData("Microsoft.DataStreamer.Samples.EarthquakeSimulator.Resources.Data.Sample Data.csv");

            // Earthquake seismic data
            _earthQuakedata = LoadSeismicData("Microsoft.DataStreamer.Samples.EarthquakeSimulator.Resources.Data.Earthquake.csv");
        }
        
        public void StartEarthquake(double pga, double duration)
        {
            if(pga == 0d)
                return;

            lock(_lock)
            {
                _pga      = Math.Abs(pga);
                _duration = duration;
                _start    = DateTime.Now;
                _currentIndex = 0;
            }
        }
        
        public void StopEarthquake()
        {
            lock(_lock)
            {
                _pga          = 0d;
                _duration     = 0d;
                _start        = DateTime.MinValue;
                _currentIndex = 0;
            }
        }

        #region IStreamingRepository

        public async Task<string> GetData()
        {
            var pga      = 0d;
            var duration = 0d;
            var data     = _backgroundData;
            var factor   = 1d;

            lock(_lock)
            {
                pga = _pga;
                duration = _duration;
            }

            // We're in the middle of an earthquake
            if(pga != 0d)
            {
                if((DateTime.Now - _start).TotalSeconds > duration)
                   StopEarthquake();
                else
                { 
                    data = _earthQuakedata;
                    factor = (pga / MaxSampleData);
                }
            }

            if((data?.Length ?? 0) > 0)
            { 
                if(_currentIndex == data.Length)
                    _currentIndex = 0;
 
                var sVal = data[_currentIndex++];

                if(double.TryParse(sVal, NumberStyles.Any, CultureInfo.InvariantCulture, out double dValue))
                    return string.Format("{0:0.00}", dValue * factor);
            }

            return await Task.FromResult("");
        }

        public DataSourceManifest Manifest => _manifest;

        #endregion

        #region Manifest

        private readonly DataSourceManifest _manifest = new DataSourceManifest
        {
            Id           = "B727D370-E1A3-4C5A-928E-F5CAD9261791",
            Name         = "Earthquake Simulator",
            DataInterval = 10,
            Channels     = new List<Channel>
            {
                new Channel { 
                                Name          = "Seismometer",
                                Id            = "SEIS",
                                Description   = "A measurement of the vertical ground motion in g's",
                                UnitOfMeasure = "g's"
                            }                
            }
        };

        #endregion

        #region Private

        private static string[] LoadSeismicData(string fileName)
        {
            var result   = "";

            using(var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(fileName))
            { 
                using (StreamReader reader = new StreamReader(stream))
                {
                    result = reader.ReadToEnd();
                }
            }

            return result.Replace("\r\n", "\n").Replace("\r", "\n").Split("\n", StringSplitOptions.RemoveEmptyEntries);
        }

        #endregion
    }
}
