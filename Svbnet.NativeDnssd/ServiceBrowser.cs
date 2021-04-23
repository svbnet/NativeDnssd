using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Text;
using Svbnet.NativeDnssd.Interop.Win32;

namespace Svbnet.NativeDnssd
{
    public abstract class ServiceBrowser
    {
        public const string ServiceDiscoveryName = "_services._dns-sd._udp.local";

        public static ServiceBrowser Create()
        {
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                return new Win32ServiceBrowser();
            }
            throw new PlatformNotSupportedException("Service browsers can currently only be created on Windows.");
        }

        public abstract void Start(string queryName);

        public abstract void Start(string queryName, NetworkInterface networkInterface);

        public bool Running { get; protected set; }

        public abstract void Stop();

        protected void OnServiceDiscovered(ServiceDiscoveredEventArgs eventArgs)
        {
            ServiceDiscovered?.Invoke(this, eventArgs);
        }

        public event EventHandler<ServiceDiscoveredEventArgs> ServiceDiscovered;
    }
}
