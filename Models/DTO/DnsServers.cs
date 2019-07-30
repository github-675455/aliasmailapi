using System.Net;

namespace aliasmailapi.Models.Enum
{
    public static class DnsServers
    {
        public readonly static IPAddress CloudFlare = IPAddress.Parse("1.1.1.1");
        public readonly static IPAddress GooglePrimary = IPAddress.Parse("8.8.8.8");
        public readonly static IPAddress GoogleSecondary = IPAddress.Parse("8.8.4.4");
    }
}