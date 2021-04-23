using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Text;

namespace Svbnet.NativeDnssd.Interop.Win32
{
    internal class Win32ServiceBrowser : ServiceBrowser, IDisposable
    {
        private const int DNS_REQUEST_PENDING = 0x2522;

        private DNS_SERVICE_CANCEL cancelHandle;

        public override void Start(string queryName)
        {
            StartInternal(queryName, 0);
        }

        public override void Start(string queryName, NetworkInterface networkInterface)
        {
            if (!(networkInterface.Supports(NetworkInterfaceComponent.IPv4) ||
                  networkInterface.Supports(NetworkInterfaceComponent.IPv6)))
            {
                throw new ArgumentException("Network interface does not support IP.");
            }

            var props = networkInterface.GetIPProperties().GetIPv4Properties();
            StartInternal(queryName, (uint)props.Index);
        }

        private void StartInternal(string queryName, uint ifIndex)
        {
            if (Running) throw new InvalidOperationException("The service browser is already running.");
            var cbd = new DNS_QUERY_COMPLETION_ROUTINE(BrowseCallback);
            var request = new DNS_SERVICE_BROWSE_REQUEST
            {
                InterfaceIndex = ifIndex,
                pQueryContext = IntPtr.Zero,
                QueryName = queryName,
                pBrowseCallbackV2 = cbd
            };
            cancelHandle = new DNS_SERVICE_CANCEL();
            var result = NativeMethods.DnsServiceBrowse(request, cancelHandle);
            if (result != DNS_REQUEST_PENDING)
                throw new Win32Exception(result);
            Running = true;
        }

        private void NotifyOfNewServiceFromRecords(IntPtr recordPtr)
        {
            var service = new NetworkService();
            var ipAddresses = new List<IPAddress>();
            var srvRecords = new List<DNS_SRV_DATA>();
            var txtRecords = new List<string>();

            while (recordPtr != IntPtr.Zero)
            {
                var record = Marshal.PtrToStructure<DNS_RECORD>(recordPtr);
                switch (record.wType)
                {
                    case DnsRecordType.DNS_TYPE_A:
                        var addr = new IPAddress(record.Data.A.IpAddress);
                        ipAddresses.Add(addr);
                        break;

                    case DnsRecordType.DNS_TYPE_PTR:
                        var nameHost = Marshal.PtrToStringUni(record.Data.PTR.pNameHost);
                        var dotIndex = nameHost.IndexOf('.');
                        if (dotIndex == -1) break;
                        service.Name = nameHost.Substring(0, dotIndex);
                        service.Type = nameHost.Substring(dotIndex + 1);
                        break;

                    case DnsRecordType.DNS_TYPE_AAAA:
                        var bytes = new Span<byte>(new byte[sizeof(ulong) * 2]);
                        BinaryPrimitives.WriteUInt64BigEndian(bytes, record.Data.AAAA.IP6Addr1);
                        BinaryPrimitives.WriteUInt64BigEndian(bytes, record.Data.AAAA.IP6Addr2);
                        ipAddresses.Add(new IPAddress(bytes));
                        break;

                    case DnsRecordType.DNS_TYPE_SRV:
                        srvRecords.Add(record.Data.SRV);
                        break;

                    case DnsRecordType.DNS_TYPE_TEXT:
                        // DNS_TXT_DATA is a flexible last member struct
                        var dataMem = recordPtr + Marshal.SizeOf<DNS_RECORD_NO_DATA_STRUCT>() +
                                      (sizeof(uint) * 2); // 64-bit packing?
                        for (var i = 0; i < record.Data.TXT.dwStringCount; i++)
                        {
                            var strPtr = Marshal.ReadIntPtr(dataMem + (i * IntPtr.Size));
                            var entry = Marshal.PtrToStringUni(strPtr);
                            txtRecords.Add(entry);
                        }
                        break;
                }
                recordPtr = record.pNext;
            }

            service.IpAddresses = ipAddresses.ToArray();
            service.TxtRecords = txtRecords.ToArray();
            srvRecords.Sort((a, b) =>
            {
                if (a.wPriority != b.wPriority) return a.wPriority - b.wPriority;
                return b.wWeight - a.wWeight;
            });
            var bestSrvRecord = srvRecords.FirstOrDefault();
            service.HostName = Marshal.PtrToStringUni(bestSrvRecord.pNameTarget);
            service.Port = bestSrvRecord.wPort;
            OnServiceDiscovered(new ServiceDiscoveredEventArgs(service));
        }

        private void BrowseCallback(IntPtr pQueryContext, DNS_QUERY_RESULT pQueryResults)
        {
            var recordPtr = pQueryResults.pQueryRecords;
            try
            {
                NotifyOfNewServiceFromRecords(recordPtr);
            }
            finally
            {
                NativeMethods.DnsFree(recordPtr, DNS_FREE_TYPE.DnsFreeRecordList);
            }
        }

        public override void Stop()
        {
            if (cancelHandle == null) return;  
            NativeMethods.DnsServiceBrowseCancel(cancelHandle);
            cancelHandle = null;
        }

        private void ReleaseUnmanagedResources()
        {
            if (cancelHandle != null) Stop();
        }

        public void Dispose()
        {
            ReleaseUnmanagedResources();
            GC.SuppressFinalize(this);
        }

        ~Win32ServiceBrowser()
        {
            ReleaseUnmanagedResources();
        }
    }
}
