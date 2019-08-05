using System.Net;

namespace AliasMailApi.Configuration
{
    public class DnsEntry
    {
        public IPAddress IP { get { return this.IP; } set { value = IPAddress.Parse(value.ToString()); } }
        public string Name { get; set; }
    }
}
