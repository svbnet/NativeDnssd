using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Text;

namespace Svbnet.NativeDnssd.Interop.Win32
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    internal class DNS_SERVICE_BROWSE_REQUEST
    {
        // Only implemented version 2 of the structure
        public uint Version = 0x2;
        public uint InterfaceIndex;
        public string QueryName;
        public DNS_QUERY_COMPLETION_ROUTINE pBrowseCallbackV2;
        public IntPtr pQueryContext;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal class DNS_SERVICE_CANCEL
    {
        public IntPtr reserved;
    }

    internal delegate void DNS_QUERY_COMPLETION_ROUTINE(IntPtr pQueryContext, DNS_QUERY_RESULT pQueryResults);

    internal enum DNS_FREE_TYPE
    {
        DnsFreeFlat,
        DnsFreeRecordList,
        DnsFreeParsedMessageFields
    }

    [StructLayout(LayoutKind.Sequential)]
    internal class DNS_QUERY_RESULT
    {
        // Only one version
        // #define DNS_QUERY_REQUEST_VERSION1  0x1
        public uint Version = 0x1;
        public int QueryStatus;
        public ulong QueryOptions;
        public IntPtr pQueryRecords;
        public IntPtr Reserved;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct DNS_A_DATA
    {
        public uint IpAddress;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct DNS_AAAA_DATA
    {
        public ulong IP6Addr1;
        public ulong IP6Addr2;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    internal struct DNS_PTR_DATA
    {
        public IntPtr pNameHost;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    internal struct DNS_SRV_DATA
    {
        public IntPtr pNameTarget; // string
        public ushort wPriority;
        public ushort wWeight;
        public ushort wPort;
        public ushort Pad;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct DNS_TXT_DATA
    {
        public uint dwStringCount;
        public IntPtr pStringArray; // string[]
    }

    [StructLayout(LayoutKind.Explicit)]
    internal struct DnsRecordData
    {
        [FieldOffset(0)] public DNS_A_DATA A;
        [FieldOffset(0)] public DNS_PTR_DATA PTR;
        [FieldOffset(0)] public DNS_TXT_DATA TXT;
        [FieldOffset(0)] public DNS_AAAA_DATA AAAA;
        [FieldOffset(0)] public DNS_SRV_DATA SRV;
    }

    internal enum DnsRecordType : ushort
    {
        DNS_TYPE_ZERO = 0x0000,

        //  RFC 1034/1035
        DNS_TYPE_A = 0x0001,      //  1
        DNS_TYPE_NS = 0x0002,      //  2
        DNS_TYPE_MD = 0x0003,      //  3
        DNS_TYPE_MF = 0x0004,      //  4
        DNS_TYPE_CNAME = 0x0005,      //  5
        DNS_TYPE_SOA = 0x0006,      //  6
        DNS_TYPE_MB = 0x0007,      //  7
        DNS_TYPE_MG = 0x0008,      //  8
        DNS_TYPE_MR = 0x0009,      //  9
        DNS_TYPE_NULL = 0x000a,      //  10
        DNS_TYPE_WKS = 0x000b,      //  11
        DNS_TYPE_PTR = 0x000c,      //  12
        DNS_TYPE_HINFO = 0x000d,      //  13
        DNS_TYPE_MINFO = 0x000e,      //  14
        DNS_TYPE_MX = 0x000f,      //  15
        DNS_TYPE_TEXT = 0x0010,      //  16

        //  RFC 1183
        DNS_TYPE_RP = 0x0011,      //  17
        DNS_TYPE_AFSDB = 0x0012,      //  18
        DNS_TYPE_X25 = 0x0013,      //  19
        DNS_TYPE_ISDN = 0x0014,      //  20
        DNS_TYPE_RT = 0x0015,      //  21

        //  RFC 1348
        DNS_TYPE_NSAP = 0x0016,      //  22
        DNS_TYPE_NSAPPTR = 0x0017,      //  23

        //  RFC 2065    (DNS security)
        DNS_TYPE_SIG = 0x0018,      //  24
        DNS_TYPE_KEY = 0x0019,      //  25

        //  RFC 1664    (X.400 mail)
        DNS_TYPE_PX = 0x001a,      //  26

        //  RFC 1712    (Geographic position)
        DNS_TYPE_GPOS = 0x001b,      //  27

        //  RFC 1886    (IPv6 Address)
        DNS_TYPE_AAAA = 0x001c,      //  28

        //  RFC 1876    (Geographic location)
        DNS_TYPE_LOC = 0x001d,      //  29

        //  RFC 2065    (Secure negative response)
        DNS_TYPE_NXT = 0x001e,      //  30

        //  Patton      (Endpoint Identifier)
        DNS_TYPE_EID = 0x001f,      //  31

        //  Patton      (Nimrod Locator)
        DNS_TYPE_NIMLOC = 0x0020,      //  32

        //  RFC 2052    (Service location)
        DNS_TYPE_SRV = 0x0021,      //  33

        //  ATM Standard something-or-another (ATM Address)
        DNS_TYPE_ATMA = 0x0022,      //  34

        //  RFC 2168    (Naming Authority Pointer)
        DNS_TYPE_NAPTR = 0x0023,      //  35

        //  RFC 2230    (Key Exchanger)
        DNS_TYPE_KX = 0x0024,      //  36

        //  RFC 2538    (CERT)
        DNS_TYPE_CERT = 0x0025,      //  37

        //  A6 Draft    (A6)
        DNS_TYPE_A6 = 0x0026,      //  38

        //  DNAME Draft (DNAME)
        DNS_TYPE_DNAME = 0x0027,      //  39

        //  Eastlake    (Kitchen Sink)
        DNS_TYPE_SINK = 0x0028,      //  40

        //  RFC 2671    (EDNS OPT)
        DNS_TYPE_OPT = 0x0029,      //  41

        //  RFC 4034    (DNSSEC DS)
        DNS_TYPE_DS = 0x002b,      //  43

        //  RFC 4034    (DNSSEC RRSIG)
        DNS_TYPE_RRSIG = 0x002e,      //  46

        //  RFC 4034    (DNSSEC NSEC)
        DNS_TYPE_NSEC = 0x002f,      //  47

        //  RFC 4034    (DNSSEC DNSKEY)
        DNS_TYPE_DNSKEY = 0x0030,      //  48

        //  RFC 4701    (DHCID)
        DNS_TYPE_DHCID = 0x0031,      //  49

        //  RFC 5155    (DNSSEC NSEC3)
        DNS_TYPE_NSEC3 = 0x0032,      //  50

        //  RFC 5155    (DNSSEC NSEC3PARAM)
        DNS_TYPE_NSEC3PARAM = 0x0033,      //  51

        //RFC 6698	(TLSA)
        DNS_TYPE_TLSA = 0x0034,      //  52

        //
        //  IANA Reserved
        //

        DNS_TYPE_UINFO = 0x0064,      //  100
        DNS_TYPE_UID = 0x0065,      //  101
        DNS_TYPE_GID = 0x0066,      //  102
        DNS_TYPE_UNSPEC = 0x0067,      //  103

        //
        //  Query only types (1035, 1995)
        //      - Crawford      (ADDRS)
        //      - TKEY draft    (TKEY)
        //      - TSIG draft    (TSIG)
        //      - RFC 1995      (IXFR)
        //      - RFC 1035      (AXFR up)
        //

        DNS_TYPE_ADDRS = 0x00f8,      //  248
        DNS_TYPE_TKEY = 0x00f9,      //  249
        DNS_TYPE_TSIG = 0x00fa,      //  250
        DNS_TYPE_IXFR = 0x00fb,      //  251
        DNS_TYPE_AXFR = 0x00fc,      //  252
        DNS_TYPE_MAILB = 0x00fd,      //  253
        DNS_TYPE_MAILA = 0x00fe,      //  254
        DNS_TYPE_ALL = 0x00ff,      //  255
        DNS_TYPE_ANY = 0x00ff,      //  255

        //
        //  Private use Microsoft types --  See www.iana.org/assignments/dns-parameters
        //

        DNS_TYPE_WINS = 0xff01,      //  64K - 255
        DNS_TYPE_WINSR = 0xff02,      //  64K - 254
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    internal class DNS_RECORD
    {
        public IntPtr pNext;
        public string pName;
        public DnsRecordType wType;
        public ushort wDataLength;
        public uint dwFlags;
        public uint dwTtl;
        public uint dwReserved;
        public DnsRecordData Data;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    internal struct DNS_RECORD_NO_DATA_STRUCT
    {
        public IntPtr pNext;
        public string pName;
        public DnsRecordType wType;
        public ushort wDataLength;
        public uint dwFlags;
        public uint dwTtl;
        public uint dwReserved;
    }

    internal class NativeMethods
    {
        private const string DnsApi = "dnsapi";

        [DllImport(DnsApi, ExactSpelling = true)]
        public static extern int DnsServiceBrowse([In] DNS_SERVICE_BROWSE_REQUEST pRequest, [In] DNS_SERVICE_CANCEL pCancel);

        [DllImport(DnsApi, ExactSpelling = true)]
        public static extern int DnsServiceBrowseCancel([In] DNS_SERVICE_CANCEL pCancelHandle);

        // DnsRecordListFree is a macro
        [DllImport(DnsApi, ExactSpelling = true)]
        public static extern int DnsFree([In] IntPtr pData, [In] DNS_FREE_TYPE FreeType);
    }
}
