# NativeDnssd
A native DNS-SD browser library for .NET. Currently only Windows 10 is supported through the `DnsServiceBrowse` API.
Avahi support is TBA.

## TODO
* Avahi support for Linux
* Cache entries for `Win32ServiceBrowser`

## Limitations
* The Windows 10 `DnsServiceBrowse` API only supports `.local` domains.

## Usage

    var serviceBrowser = ServiceBrowser.Create();
    serviceBrowser.ServiceDiscovered += (sender, args) => Console.WriteLine($"Found service {args.Service.Name}!");
    // Find all HTTP services
    serviceBrowser.Start("_http._tcp.local");

    // Or, find all services
    serviceBrowser.Start(ServiceBrowser.ServiceDiscoveryName);
