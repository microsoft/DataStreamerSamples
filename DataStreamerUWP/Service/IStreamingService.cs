using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.DataStreamer.UWP
{
    public interface IStreamingService
    {
        Task<string> Connect();
        Task         Disconnect();

        Task         StartData();
        Task         StopData();
        Task         StartRecording(string fileName);
        Task         StopRecording();
        Task         Reset();
        Task         Ready();
        Task         NotReady();
        Task         UpdateManifest();
    }
}
