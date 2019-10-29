using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.DataStreamer.UWP
{
    public interface IStreamingRepository
    {
        Task<string>        GetData();
        DataSourceManifest  Manifest { get; }
    }
}
