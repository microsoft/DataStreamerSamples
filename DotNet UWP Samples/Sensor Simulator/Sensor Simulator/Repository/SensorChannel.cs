using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Microsoft.DataStreamer.UWP;

namespace Microsoft.DataStreamer.Samples.SensorSimulator
{
    public class SensorChannel : Channel
    {
        [JsonIgnore]
        public bool Active        { get; set; } = true;

        [JsonIgnore]
        public bool PendingActive { get; set; } = true;


        [JsonIgnore]
        public int Index          { get; set; }

        private readonly Random _random = new Random();

        public virtual string GetData()
        {
            return (this.Index + (1 / (Math.Floor(_random.Next() / 1000000d) + 1.789))).ToString();
        }
    }
}
