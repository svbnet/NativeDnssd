using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net;
using System.Text;

namespace Svbnet.NativeDnssd
{
    public sealed class NetworkService
    {
        private NameValueCollection parsedTxtRecords;

        internal NetworkService()
        {

        }

        /// <summary>
        /// The service's name.
        /// </summary>
        public string Name { get; internal set; }

        /// <summary>
        /// The service's type in the form _[service]._[protocol]. This is returned from the first PTR record.
        /// </summary>
        public string Type { get; internal set; }

        /// <summary>
        /// The service's hostname. This is returned from the first SRV record.
        /// </summary>
        public string HostName { get; internal set; }

        /// <summary>
        /// All IP addresses that point to the service. This is populated by the first A and AAAA record if it exists.
        /// </summary>
        public IPAddress[] IpAddresses { get; internal set;  }

        /// <summary>
        /// The port of the service. This is returned from the first SRV record.
        /// </summary>
        public ushort Port { get; internal set; }

        /// <summary>
        /// The bare TXT records returned from the service.
        /// </summary>
        public string[] TxtRecords { get; internal set; }

        /// <summary>
        /// A collection of name-value pairs parsed out from the <see cref="TxtRecords"/> property.
        /// </summary>
        public NameValueCollection ParsedTxtRecords {
            get
            {
                if (parsedTxtRecords == null)
                {
                    parsedTxtRecords = new NameValueCollection();
                    foreach (var record in TxtRecords)
                    {
                        var equalsIndex = record.IndexOf('=');
                        if (equalsIndex == -1) continue;
                        var name = record.Substring(0, equalsIndex);
                        var value = record.Substring(equalsIndex + 1);
                        parsedTxtRecords.Add(name, value);
                    }
                }

                return parsedTxtRecords;
            }

        }
    }
}
