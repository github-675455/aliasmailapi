using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using aliasmailapi.Interfaces;
using AliasMailApi.Configuration;
using DNS.Client;
using DNS.Protocol;
using DNS.Protocol.ResourceRecords;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Timeout;

namespace aliasmailapi.Utils
{
    public class DNSManager : IDNSManager
    {
        private readonly DnsOptions _dnsOptions;
        public DNSManager(IOptions<DnsOptions> dnsOptions)
        {
            _dnsOptions = dnsOptions.Value;
            loadLocalDns();
        }

        private void loadLocalDns()
        {
            if(_dnsOptions.PrefererLocalDns)
                _dnsOptions.Servers.Prepend(getLocalDnsServer());
        }

        private DnsEntry getLocalDnsServer()
        {
            var localDnsServer = NetworkInterface.GetAllNetworkInterfaces()
            .Select(adapter =>
            {
                        if(adapter.OperationalStatus != OperationalStatus.Down &&
                        adapter.NetworkInterfaceType != NetworkInterfaceType.Loopback)
                        {
                            IPInterfaceProperties adapterProperties = adapter.GetIPProperties();
                            if(
                                (adapterProperties.IsDnsEnabled || adapterProperties.IsDynamicDnsEnabled) &&
                                adapterProperties.DhcpServerAddresses.Count() != 0)
                            {
                                return adapterProperties.DnsAddresses.FirstOrDefault();
                            }
                        }
                        return null;
            }).Where(e => e != null).FirstOrDefault();

            return new DnsEntry {
                IP = localDnsServer,
                Name = "Local Dns Server"
            };
        }

        public async Task<IList<IResourceRecord>> Resolve(string domain, RecordType recordType)
        {
            IResponse response = await Policy
                .Handle<ResponseException>()
                .Or<TimeoutRejectedException>()
                .WaitAndRetry(3, (c, t) => TimeSpan.FromMilliseconds(100))
                .Wrap(Policy.Timeout(1))
                .Execute(async () =>
                {
                    ClientRequest request = new ClientRequest(IPAddress.Parse("172.18.210.6"));

                    request.Questions.Add(new Question(DNS.Protocol.Domain.FromString(domain), recordType));
                    request.RecursionDesired = true;           

                    return await request.Resolve();
                });

            return response.AnswerRecords;
        }
    }
}