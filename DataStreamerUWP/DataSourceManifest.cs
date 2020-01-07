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

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Microsoft.DataStreamer.UWP
{
    /// <summary>
    /// Object model for a manifest to be sent to Data Streamer. A manifest gives Data Streamer information about this 
    ///   application and the data it will send.
    /// <summary>
    public class DataSourceManifest
    {
        public string         Id            { get; set; }
        public string         Version       { get; set; } = "1.0";
        public string         Name          { get; set; }
        public int            DataInterval  { get; set; }
        public IList<Channel> Channels      { get; set; } = new List<Channel>();
        public IList<Command> Commands      { get; set;} = new List<Command>();
 
        /// <summary>
        /// A command is a procedure this application can perform which can be called from Data Streamer
        /// <summary>
        public class Command
        {
            public string       Name                 { get; set;}
            public string       Description          { get; set;}
            public IList<Param> Params               { get; set;} = new List<Param>(); 
                                                     
            public class Param                       
            {                                        
                public string   Name                 { get; set;}
                public string   Type                 { get; set;}
                public string   Description          { get; set;}
                public string   Value                { get; set;}
                public Range    Range                { get; set; }
                public IList<LookupValue> LookupList { get; set; } = new List<LookupValue>();
            }
        
            public class LookupValue
            {
                public string Value                  { get; set;}
                public string Caption                { get; set; }
            }
        
            public class Range
            {
                public double? Min                   { get; set; }
                public double? Max                   { get; set; }
                public string  ErrorMessage          { get; set; }
            }
        }

        public DataSourceManifest ShallowClone()
        {
            return new DataSourceManifest
            {
                Id           = this.Id,
                Version      = this.Version,
                Name         = this.Name,
                DataInterval = this.DataInterval,
                Channels     = new List<Channel>(this.Channels),
                Commands     = new List<Command>(this.Commands)
            };
        }
   }

    /// <summary>
    /// A Channel describes the data sent in a single column
    /// <summary>
    public class Channel
    {
        public string         Id            { get; set; }
        public string         Name          { get; set; }
        public string         Description   { get; set; }
        public string         UnitOfMeasure { get; set; }
        public IList<Channel> SubChannels   { get; set; } = new List<Channel>();
    }
}
