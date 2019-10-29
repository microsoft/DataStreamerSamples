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
using System.Threading;
using System.Threading.Tasks;

using Windows.ApplicationModel.AppService;
using Windows.Foundation.Collections;

using DataStreamer.App.Core;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

using Microsoft.DataStreamer.UWP;

namespace Microsoft.DataStreamer.Samples.EarthquakeSimulator
{
    public class EarthquakeService : AppServiceStreamingService
    {
        private readonly EarthquakeRepository _repo;

        public EarthquakeService(EarthquakeRepository repository) : base(repository)
        {
            _repo = repository;
        }

        public void StartEarthquake(double pga, double duration)
        {
            _repo.StartEarthquake(pga, duration);
        }

        public void StopEarthquake()
        {
            _repo.StopEarthquake();
        }
    }
}
