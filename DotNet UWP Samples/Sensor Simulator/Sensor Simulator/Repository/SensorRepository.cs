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
using System.Threading.Tasks;

using Microsoft.DataStreamer.UWP;
using System.Globalization;

namespace Microsoft.DataStreamer.Samples.SensorSimulator
{
    public class SensorRepository : IStreamingRepository
    {
        private DateTime        _start        = DateTime.MinValue;
        private readonly object _lock         = new object();
        private const double    MaxSampleData = 1.94d;

        public SensorRepository()
        {
            this.NumChannels = _manifest.Channels.GetLeaves<Channel>("SubChannels").Count();
        }
        
        public int NumChannels { get; set; }

        #region IStreamingRepository

        private readonly Random _random = new Random();

        public async Task<string> GetData()
        {
            var list        = new List<string>();
            var numChannels = Math.Min(Math.Max(1, this.NumChannels), 99);

            for(int i = 0; i < numChannels; ++i)
                list.Add(Math.Floor(_random.Next() / 1000000d).ToString());

            return await Task.FromResult(string.Join(",", list));
        }

        public DataSourceManifest Manifest => _manifest;

        #endregion

        #region Manifest

        private readonly DataSourceManifest _manifest = new DataSourceManifest
        {
            Id           = "806A1E8C-0F7C-4964-B5AF-1A518867CA4E",
            Name         = "Sensor Simulator",
            DataInterval = 40,
            Channels     = new List<Channel>
            {
                new Channel 
                {
                    Name          = "Angular Sensors",
                    Description   = "Sensors with components in 3 planes (e.g. X, Y, Z) measured in degrees",
                    SubChannels   = new List<Channel>
                    {
                        new Channel 
                        {
                            Name          = "Accelerometer",
                            Description   = "G-force acceleration",
                            UnitOfMeasure = "g's",
                            SubChannels   = new List<Channel>
                            {
                                new Channel 
                                {
                                    Name          = "X",
                                    Id            = "ACC_X",
                                    Description   = "G-force acceleration along the x-axis",
                                    UnitOfMeasure = "g's"
                                },
                                new Channel 
                                {
                                    Name          = "Y",
                                    Id            = "ACC_Y",
                                    Description   = "G-force acceleration along the y-axis",
                                    UnitOfMeasure = "g's"
                                },
                                new Channel 
                                {
                                    Name          = "Z",
                                    Id            = "ACC_Z",
                                    Description   = "G-force acceleration along the z-axis",
                                    UnitOfMeasure = "g's"
                                }
                            }
                        },
                        new Channel 
                        {
                            Name          = "Gyrometer",
                            Description   = "Angular velocity, in degrees per second",
                            UnitOfMeasure = "°/sec",
                            SubChannels   = new List<Channel>
                            {
                                new Channel 
                                {
                                    Name          = "X",
                                    Id            = "GYRO_X",
                                    Description   = "Angular velocity, in degrees per second, about the x-axis",
                                    UnitOfMeasure = "°/sec"
                                },
                                new Channel 
                                {
                                    Name          = "Y",
                                    Id            = "GYRO_Y",
                                    Description   = "Angular velocity, in degrees per second, about the y-axis",
                                    UnitOfMeasure = "°/sec"
                                },
                                new Channel 
                                {
                                    Name          = "Z",
                                    Id            = "GYRO_Z",
                                    Description   = "Angular velocity, in degrees per second, about the z-axis",
                                    UnitOfMeasure = "°/sec"
                                }
                            }
                        },
                        new Channel 
                        {
                            Name          = "Inclinometer",
                            Id            = "PITCH",
                            Description   = "Rotation in degrees",
                            UnitOfMeasure = "°",
                            SubChannels   = new List<Channel>
                            {
                                new Channel 
                                {
                                    Name          = "Pitch",
                                    Id            = "PITCH",
                                    Description   = "Rotation in degrees around the x-axis",
                                    UnitOfMeasure = "°"
                                },
                                new Channel 
                                {
                                    Name          = "Roll",
                                    Id            = "ROLL",
                                    Description   = "Rotation in degrees around the y-axis",
                                    UnitOfMeasure = "°"
                                },
                                new Channel 
                                {
                                    Name          = "Yaw",
                                    Id            = "YAW",
                                    Description   = "Rotation in degrees around the z-axis",
                                    UnitOfMeasure = "°"
                                }
                            }
                        }
                    }
                },
                new Channel 
                {
                    Name          = "Environmental Sensors",
                    Description   = "Sensors that measure the environment",
                    SubChannels   = new List<Channel>
                    {
                        new Channel 
                        {
                            Name          = "Illuminance",
                            Id            = "ILLUM",
                            Description   = "Illuminance in Lux",
                            UnitOfMeasure = "lux"
                        },
                                                                       
                        new Channel 
                        {
                            Name          = "Altimeter",
                            Id            = "ALT",
                            Description   = "Current altitude determined by the altimeter sensor in meters",
                            UnitOfMeasure = "m"
                        },
                        new Channel 
                        {
                            Name          = "Barometer",
                            Id            = "BAR",
                            Description   = "Barometric pressure in Hectopascals",
                            UnitOfMeasure = "hPA"
                        }
                    }
                },
                new Channel 
                {
                    Name          = "Compass",
                    Description   = "Compass heading in degrees",
                    UnitOfMeasure = "°",
                    SubChannels   = new List<Channel>
                    {
                        new Channel
                        {
                            Name          = "Magnetic North",
                            Id            = "COMPASS_MN",
                            Description   = "Heading in degrees relative to magnetic-north",
                            UnitOfMeasure = "°"
                        },
                        new Channel 
                        {
                            Name          = "True North",
                            Id            = "COMPASS_TN",
                            Description   = "Heading in degrees relative to geographic true-north",
                            UnitOfMeasure = "°"
                        }
                    }
                }
            },
            Commands     = new List<DataSourceManifest.Command>
            {                                
                new DataSourceManifest.Command
                {
                    Name        = "BlastOff",
                    Description = "Instruct the space shuttle to take off",
                    Params      = new List<DataSourceManifest.Command.Param>
                    {
                        new DataSourceManifest.Command.Param
                        {
                            Name        = "Altitude",
                            Description = "The highest altitude to attain",
                            Type        = "integer"
                        }
                    }
                },
                new DataSourceManifest.Command
                {
                    Name        = "Land",
                    Description = "Instruct the space shuttle to land",
                    Params      = new List<DataSourceManifest.Command.Param>
                    {
                        new DataSourceManifest.Command.Param
                        {
                            Name        = "Location",
                            Description = "The location (city) of the landing pad",
                            Type        = "string"
                        }
                    }
                },
                new DataSourceManifest.Command
                {
                    Name        = "SetMode",
                    Description = "Put space shuttle in a specific mode",
                    Params      = new List<DataSourceManifest.Command.Param>
                    {
                        new DataSourceManifest.Command.Param
                        {
                            Name        = "Mode",
                            Description = "The mode to put the space shuttle in",
                            Type        = "string"
                        }
                    }
                },
                new DataSourceManifest.Command
                {
                    Name        = "StartCountdown",
                    Description = "Start the countdown"
                }
            }               
        };

        #endregion
    }
}
