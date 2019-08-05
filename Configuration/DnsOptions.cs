namespace AliasMailApi.Configuration
{
    public class DnsOptions
    {
        public bool PrefererLocalDns { get; set; }
        public DnsEntry[] Servers { get; set; }
    }
}
