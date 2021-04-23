using System;
using System.Collections.Generic;
using System.Text;

namespace Svbnet.NativeDnssd
{
    public class ServiceDiscoveredEventArgs : EventArgs
    {
        internal ServiceDiscoveredEventArgs(NetworkService service)
        {
            Service = service;
        }
        public NetworkService Service { get; }
    }
}
