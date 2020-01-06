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
using Windows.Media.ClosedCaptioning;

namespace Microsoft.DataStreamer.Samples.SensorSimulator
{
    public class SensorRepository : IStreamingRepository
    {
        public SensorRepository()
        {
        }
        
        public SensorChannel AngularSensors  => this.Manifest.Channels[0] as SensorChannel;
        public SensorChannel Accelerometer   => this.Manifest.Channels[0].SubChannels[0] as SensorChannel;
        public SensorChannel Gyrometer       => this.Manifest.Channels[0].SubChannels[1] as SensorChannel;
        public SensorChannel Inclinometer    => this.Manifest.Channels[0].SubChannels[2] as SensorChannel;
        public SensorChannel Environmental   => this.Manifest.Channels[1] as SensorChannel;
        public SensorChannel Illuminance     => this.Manifest.Channels[1].SubChannels[0] as SensorChannel;
        public SensorChannel Altimeter       => this.Manifest.Channels[1].SubChannels[1] as SensorChannel;
        public SensorChannel Barometer       => this.Manifest.Channels[1].SubChannels[2] as SensorChannel;
        public SensorChannel Compass         => this.Manifest.Channels[2] as SensorChannel;
        public SensorChannel MagneticNorth   => this.Manifest.Channels[2].SubChannels[0] as SensorChannel;
        public SensorChannel TrueNorth       => this.Manifest.Channels[2].SubChannels[1] as SensorChannel;

        public void UpdatePending()
        {
            UpdatePending(this.Manifest.Channels);
        }

        private static void UpdatePending(IList<Channel> channels)
        {
            if(channels != null)
            { 
                foreach(SensorChannel channel in channels)
                { 
                    channel.Active = channel.PendingActive;

                    UpdatePending(channel.SubChannels);
                }
            }
        }

        #region IStreamingRepository

        private readonly Random _random = new Random();

        public async Task<string> GetData()
        {
            var list = new List<string>();

            foreach(SensorChannel channel in this.Manifest.Channels)
                GetChannelData(channel, list);

            return await Task.FromResult(string.Join(",", list));
        }

        private void GetChannelData(SensorChannel channel, List<string> target)
        {
            if(channel.Active)
            {
                if((channel.SubChannels?.Count ?? 0) > 0)
                {
                    foreach(SensorChannel subChannel in channel.SubChannels)
                        GetChannelData(subChannel, target);
                }
                else
                    target.Add(channel.GetData());
            }
        }

        #endregion

        #region Manifest

        public DataSourceManifest Manifest { get; private set; } = new DataSourceManifest
        {
            Id           = "806A1E8C-0F7C-4964-B5AF-1A518867CA4E",
            Name         = "Sensor Simulator",
            DataInterval = 40,
            Channels     = new List<Channel>
            {
                new SensorChannel 
                {
                    Name          = "Angular Sensors",
                    Description   = "Sensors with components in 3 planes (e.g. X, Y, Z) measured in degrees",
                    SubChannels   = new List<Channel>
                    {
                        new SensorChannel 
                        {
                            Name          = "Accelerometer",
                            Description   = "G-force acceleration",
                            UnitOfMeasure = "g's",
                            SubChannels   = new List<Channel>
                            {
                                new SensorChannel 
                                {
                                    Name          = "X",
                                    Id            = "ACC_X",
                                    Description   = "G-force acceleration along the x-axis",
                                    UnitOfMeasure = "g's",
                                    Index         = 1
                                },
                                new SensorChannel 
                                {
                                    Name          = "Y",
                                    Id            = "ACC_Y",
                                    Description   = "G-force acceleration along the y-axis",
                                    UnitOfMeasure = "g's",
                                    Index         = 2
                                },
                                new SensorChannel 
                                {
                                    Name          = "Z",
                                    Id            = "ACC_Z",
                                    Description   = "G-force acceleration along the z-axis",
                                    UnitOfMeasure = "g's",
                                    Index         = 3
                                }
                            }
                        },
                        new SensorChannel 
                        {
                            Name          = "Gyrometer",
                            Description   = "Angular velocity, in degrees per second",
                            UnitOfMeasure = "°/sec",
                            SubChannels   = new List<Channel>
                            {
                                new SensorChannel 
                                {
                                    Name          = "X",
                                    Id            = "GYRO_X",
                                    Description   = "Angular velocity, in degrees per second, about the x-axis",
                                    UnitOfMeasure = "°/sec",
                                    Index         = 4
                                },
                                new SensorChannel 
                                {
                                    Name          = "Y",
                                    Id            = "GYRO_Y",
                                    Description   = "Angular velocity, in degrees per second, about the y-axis",
                                    UnitOfMeasure = "°/sec",
                                    Index         = 5
                                },
                                new SensorChannel 
                                {
                                    Name          = "Z",
                                    Id            = "GYRO_Z",
                                    Description   = "Angular velocity, in degrees per second, about the z-axis",
                                    UnitOfMeasure = "°/sec",
                                    Index         = 6
                                }
                            }
                        },
                        new SensorChannel 
                        {
                            Name          = "Inclinometer",
                            Id            = "PITCH",
                            Description   = "Rotation in degrees",
                            UnitOfMeasure = "°",
                            SubChannels   = new List<Channel>
                            {
                                new SensorChannel 
                                {
                                    Name          = "Pitch",
                                    Id            = "PITCH",
                                    Description   = "Rotation in degrees around the x-axis",
                                    UnitOfMeasure = "°",
                                    Index         = 7
                                },
                                new SensorChannel 
                                {
                                    Name          = "Roll",
                                    Id            = "ROLL",
                                    Description   = "Rotation in degrees around the y-axis",
                                    UnitOfMeasure = "°",
                                    Index         = 8
                                },
                                new SensorChannel 
                                {
                                    Name          = "Yaw",
                                    Id            = "YAW",
                                    Description   = "Rotation in degrees around the z-axis",
                                    UnitOfMeasure = "°",
                                    Index         = 9
                                }
                            }
                        }
                    }
                },
                new SensorChannel 
                {
                    Name          = "Environmental Sensors",
                    Description   = "Sensors that measure the environment",
                    SubChannels   = new List<Channel>
                    {
                        new SensorChannel 
                        {
                            Name          = "Illuminance",
                            Id            = "ILLUM",
                            Description   = "Illuminance in Lux",
                            UnitOfMeasure = "lux",
                            Index         = 10
                        },
                                                                       
                        new SensorChannel 
                        {
                            Name          = "Altimeter",
                            Id            = "ALT",
                            Description   = "Current altitude determined by the altimeter sensor in meters",
                            UnitOfMeasure = "m",
                            Index         = 11
                        },
                        new SensorChannel 
                        {
                            Name          = "Barometer",
                            Id            = "BAR",
                            Description   = "Barometric pressure in Hectopascals",
                            UnitOfMeasure = "hPA",
                            Index         = 12
                        }
                    }
                },
                new SensorChannel 
                {
                    Name          = "Compass",
                    Description   = "Compass heading in degrees",
                    UnitOfMeasure = "°",
                    SubChannels   = new List<Channel>
                    {
                        new SensorChannel
                        {
                            Name          = "Magnetic North",
                            Id            = "COMPASS_MN",
                            Description   = "Heading in degrees relative to magnetic-north",
                            UnitOfMeasure = "°",
                            Index         = 13
                        },
                        new SensorChannel 
                        {
                            Name          = "True North",
                            Id            = "COMPASS_TN",
                            Description   = "Heading in degrees relative to geographic true-north",
                            UnitOfMeasure = "°",
                            Index         = 14
                        }
                    }
                }
            },
            Commands     = new List<DataSourceManifest.Command>
            {                                
                new DataSourceManifest.Command
                {
                    Name        = "Measurement System",
                    Description = "Choose unit of measure system for certain sensors",
                    Params      = new List<DataSourceManifest.Command.Param>
                    {
                        new DataSourceManifest.Command.Param
                        {
                            Name        = "Measurement System",
                            Description = "Choose Metric or British/US",
                            Type        = "string",
                            LookupList  = new List<DataSourceManifest.Command.LookupValue>
                            {
                                new DataSourceManifest.Command.LookupValue
                                {
                                    Caption = "British/US",
                                    Value   = "Standard"
                                },
                                new DataSourceManifest.Command.LookupValue
                                {
                                    Caption = "Metric",
                                    Value   = "Metric"
                                }
                            }
                        }
                    }
                }
                #if false
                ,new DataSourceManifest.Command
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
                #endif
            }               
        };
        
        public DataSourceManifest ActiveManifest
        {
            get
            { 
                // First do a shallow clone
                var manifest = this.Manifest.ShallowClone();

                manifest.Channels = DeepCloneChannels(manifest.Channels);

                return manifest;
            }
        }

        private IList<Channel> DeepCloneChannels(IList<Channel> channels)
        {
            if(channels == null)
                return null;

            var list = new List<Channel>();

            foreach(SensorChannel channel in channels)
            {
                if(channel.Active)
                {
                    Channel newChannel = channel;

                    if((channel.SubChannels?.Count ?? 0) > 0)
                    { 
                        var subChannels = DeepCloneChannels(channel.SubChannels);

                        // Don't include this channel if no subchannels were active
                        if((subChannels?.Count ?? 0) == 0)
                            continue;

                        // Depp clone the channel
                        newChannel = new Channel
                        {
                            Id             = channel.Id,
                            Name           = channel.Name,
                            Description    = channel.Description,
                            UnitOfMeasure  = channel.UnitOfMeasure,
                            SubChannels    = subChannels,
                        };                        
                    }

                    list.Add(newChannel);
                }
            }

            return list.Count > 0 ? list : null;
        }

        #endregion
    }
}
