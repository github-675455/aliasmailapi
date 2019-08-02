using System.Threading.Tasks;
using AliasMailApi.Models;
using AliasMailApi.Repository;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using AliasMailApi.Interfaces;
using System;
using Microsoft.Extensions.Logging;
using aliasmailapi.Models.DTO;
using System.Text.RegularExpressions;
using aliasmailapi.Utils;
using System.Net;
using aliasmailapi.Models.Enum;
using DNS.Protocol;
using AliasMailApi.Models.Enum;
using System.Text;
using System.Net.Sockets;
using DNS.Protocol.ResourceRecords;
using System.Collections.Generic;
using AliasMailApi.Extensions;

namespace AliasMailApi.Services
{
    public class IPSubnet
    {
        private readonly byte[] _address;
        private readonly int _prefixLength;

        public IPSubnet(string value)
        {
            if (value == null)
                throw new ArgumentNullException("value");

            string[] parts = value.Split('/');
            if (parts.Length != 2)
                throw new ArgumentException("Invalid CIDR notation.", "value");

            _address = IPAddress.Parse(parts[0]).GetAddressBytes();
            _prefixLength = Convert.ToInt32(parts[1], 10);
        }

        public bool Contains(string address)
        {
            return this.Contains(IPAddress.Parse(address).GetAddressBytes());
        }

        public bool Contains(byte[] address)
        {
            if (address == null)
                throw new ArgumentNullException("address");

            if (address.Length != _address.Length)
                return false; // IPv4/IPv6 mismatch

            int index = 0;
            int bits = _prefixLength;

            for (; bits >= 8; bits -= 8)
            {
                if (address[index] != _address[index])
                    return false;
                ++index;
            }

            if (bits > 0)
            {
                int mask = (byte)~(255 >> bits);
                if ((address[index] & mask) != (_address[index] & mask))
                    return false;
            }

            return true;
        }
    }
    public static class SpfRecordParserAndEvaluate
    {
        public static SpfRecordResult Execute(string record)
        {
            var result = new SpfRecordResult();

            record = record.Trim().ToLower();

            if(!record.NotEmpty())
                throw new Exception("Empty record");

            if(isSpfRecord(record))
                throw new Exception("Is not spf record");
            
            var segments = record.Split(" ");

            if(segments.Length == 1)
            {
                result.Result = SpfResult.NEUTRAL;
                return result;
            }

            foreach(var segment in segments)
            {
                var maches = Regex.Matches(segment, @"^(?<qualifier>[\+\-\~\?])?(?<mechanism>all|include|a|mx|ptr|ip4|ip6|exists{1})(\:(?<mechanismParameter>[a-zA-Z0-9\-\.]+))?");
                var firstMatch = maches.FirstOrDefault();

                var qualifier = firstMatch.Groups["qualifier"].Value;
                var selectedQualifier = selectQualifier(qualifier);
                var mechanism = firstMatch.Groups["mechanism"].Value;
                var mechanismParameter = firstMatch.Groups["mechanismParameter"].Value;

                SpfGenericDirective currentDirective = null;

                if(mechanism.Equals("ipv4"))
                    currentDirective = new SpfIpv4(selectedQualifier, new IPSubnet(mechanismParameter));
                if(mechanism.Equals("ipv6"))
                    currentDirective = new SpfIpv6(selectedQualifier, new IPSubnet(mechanismParameter));
                if(mechanism.Equals("a"))
                    currentDirective = new SpfRecordAorAAAA(selectedQualifier, mechanismParameter);
                else if(mechanism.Equals("mx"))
                    currentDirective = new SpfRecordMX(selectedQualifier, mechanismParameter);
                else if(mechanism.Equals("include"))
                    currentDirective = new SpfRecordInclude(selectedQualifier, mechanismParameter);
                else if(mechanism.Equals("all"))
                    currentDirective = new SpfRecordAll(selectedQualifier, mechanismParameter);
                else if(mechanism.Equals("ptr"))
                    throw new NotImplementedException("ptr not implemented");
                else if(mechanism.Equals("exists"))
                    throw new NotImplementedException("ptr not implemented");

                result.Directives.Add(currentDirective);

            }
        
            return result;
        }

        private static SpfQualifier selectQualifier(string qualifier)
        {
            if(qualifier.Equals("+"))
                return SpfQualifier.POSITIVE;
            else if(qualifier.Equals("-"))
                return SpfQualifier.NEGATIVE;
            else if(qualifier.Equals("~"))
                return SpfQualifier.TILDE;
            else if(qualifier.Equals("?"))
                return SpfQualifier.QUESTION;
            return SpfQualifier.POSITIVE;
        }

        private static bool isSpfRecord(string data)
        {
            return !Regex.IsMatch(data, "$v=spf1\b");
        }
    }
    public class SpfRecordResult
    {
        private const int DnsRequestsLimit = 10;
        public List<SpfGenericDirective> Directives = new List<SpfGenericDirective>();

        public SpfResult Result;
    }
    public interface SpfGenericDirective{

    }
    public abstract class SpfDirective<T> : SpfGenericDirective
    {
        public string Domain;
        public IPAddress SenderIp;
        public int DnsRequestsCount;
        public bool IsIpv4 { get => SenderIp != null ? SenderIp.AddressFamily == AddressFamily.InterNetwork : false; }
        public SpfQualifier Qualifier = SpfQualifier.POSITIVE;
        public T MecanismParameter;
        public int NetMask;
        public abstract Task<bool> Matches();
        public SpfDirective() { }
        public SpfDirective(SpfQualifier qualifier, T parameter)
        {
            Qualifier = qualifier;
            MecanismParameter = parameter;
        }
        protected void setResultOfQualifier(bool matches)
        {
            if(Qualifier == SpfQualifier.NEGATIVE)
                if(matches)
                    Result = SpfResult.FAIL;
                else
                    Result = SpfResult.PASS;
            else if(Qualifier == SpfQualifier.POSITIVE)
                if(matches)
                    Result = SpfResult.PASS;
                else
                    Result = SpfResult.FAIL;
            else if(Qualifier == SpfQualifier.TILDE)
                if(matches)
                    Result = SpfResult.SOFTFAIL;
                else
                    Result = SpfResult.PASS;
            else if(Qualifier == SpfQualifier.QUESTION)
                if(matches)
                    Result = SpfResult.NEUTRAL;
            
        }
        public SpfResult? Result;
    }
    public sealed class SpfIpv4 : SpfDirective<IPSubnet>
    {
        public SpfIpv4(SpfQualifier qualifier, IPSubnet parameter): base(qualifier, parameter) { }
        public override Task<bool> Matches()
        {
            var localResult = MecanismParameter.Contains(Encoding.ASCII.GetBytes(SenderIp.ToString()));

            setResultOfQualifier(localResult);

            return Task.FromResult(localResult);
        }
    }

    public sealed class SpfIpv6 : SpfDirective<IPSubnet>
    {
        public SpfIpv6(SpfQualifier qualifier, IPSubnet parameter): base(qualifier, parameter) { }
        public override Task<bool> Matches()
        {
            var localResult = MecanismParameter.Contains(Encoding.UTF8.GetBytes(SenderIp.ToString()));

            setResultOfQualifier(localResult);
            
            return Task.FromResult(localResult);
        }
    }

    public sealed class SpfRecordAorAAAA : SpfDirective<string>
    {
        public SpfRecordAorAAAA(SpfQualifier qualifier, string parameter): base(qualifier, parameter) { }
        public override Task<bool> Matches()
        {
            NetMask = IsIpv4 ? 32: 128;

            if(string.IsNullOrWhiteSpace(MecanismParameter))
                MecanismParameter = Domain;

            var response = DNSManager.Resolve(MecanismParameter, IsIpv4 ?  RecordType.A : RecordType.AAAA);

            DnsRequestsCount++;

            var ipsRecords = response.Result
                .Select(e => new IPAddress(e.Data))
                .ToList();

            var ipMatch = ipsRecords.Any(e => e.Equals(SenderIp));

            setResultOfQualifier(ipMatch);

            return Task.FromResult(ipMatch);
        }
    }

    public sealed class SpfRecordMX : SpfDirective<string>
    {
        public SpfRecordMX(SpfQualifier qualifier, string parameter): base(qualifier, parameter) { }

        public override Task<bool> Matches()
        {
            var task = DNSManager.Resolve(Domain, RecordType.MX);

            List<Task<IList<IResourceRecord>>> asyncDnsRequests = new List<Task<IList<IResourceRecord>>>();

            foreach(var record in task.Result)
            {
                var mxRecord = (MailExchangeResourceRecord)record;
                var resolveMxRecord = DNSManager.Resolve(mxRecord.ExchangeDomainName.ToString(), IsIpv4 ? RecordType.A : RecordType.AAAA);
                DnsRequestsCount++;
                asyncDnsRequests.Add(resolveMxRecord);
            }

            Task.WaitAll(asyncDnsRequests.ToArray());

            foreach(var result in asyncDnsRequests)
            {
                foreach(var aOrAaaaResult in result.Result)
                {
                    var ipReourceRecord = (IPAddressResourceRecord)aOrAaaaResult;
                    if(ipReourceRecord.IPAddress == SenderIp)
                    {
                        setResultOfQualifier(true);
                        return Task.FromResult(true);
                    }
                }
            }
            setResultOfQualifier(false);
            return Task.FromResult(false);
        }
    }

    public sealed class SpfRecordInclude : SpfDirective<string>
    {
        public SpfRecordInclude(SpfQualifier qualifier, string parameter): base(qualifier, parameter) { }

        public override Task<bool> Matches()
        {
            var task = DNSManager.Resolve(Domain, RecordType.TXT);
            DnsRequestsCount++;

            List<Task<IList<IResourceRecord>>> asyncDnsRequests = new List<Task<IList<IResourceRecord>>>();

            foreach(var record in task.Result)
            {
                var mxRecord = (MailExchangeResourceRecord)record;
                var resolveMxRecord = DNSManager.Resolve(mxRecord.ExchangeDomainName.ToString(), IsIpv4 ? RecordType.A : RecordType.AAAA);
                DnsRequestsCount++;
                asyncDnsRequests.Add(resolveMxRecord);
            }

            Task.WaitAll(asyncDnsRequests.ToArray());

            return Task.FromResult(true);
        }
    }

    public sealed class SpfRecordAll : SpfDirective<string>
    {
        public SpfRecordAll(SpfQualifier qualifier, string parameter): base(qualifier, parameter) { }

        public override Task<bool> Matches()
        {
            setResultOfQualifier(true);
            return Task.FromResult(true);
        }
    }

    public enum SpfMechanism
    {
        ALL,
        INCLUDE,
        AorAAAA,
        MX,
        PTR,
        IPv4,
        IPv6,
        EXISTS
    }
    public enum SpfQualifier
    {
        POSITIVE,
        NEGATIVE,
        QUESTION,
        TILDE
    }
    
    public class SpfService : ISpfService
    {
        private readonly MessageContext _context;
        private readonly ILogger _logger;

        public SpfService(
            MessageContext context,
            ILogger<MailService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public Task<BaseMessageSpf> validate(BaseMessage message)
        {
            var result = new BaseMessageSpf();
            if(message is MailgunMessage)
            {
                var convertedMessage = (MailgunMessage)message;
                var received = parseFirstReceived(convertedMessage.Received);

                var domainAndAddress = GenericUtil.ParseMailAddress(convertedMessage.Sender, EmailSection.Address).Split("@");

                if(domainAndAddress.Length != 2)
                    throw new Exception("Invalid format, not a valid sender address.");

                var domain = domainAndAddress.LastOrDefault();

                var resolvedTxtRecords = DNSManager.Resolve(domain, RecordType.TXT);
                var countResolvedTxtRecords = resolvedTxtRecords.Result.Count();

                if(countResolvedTxtRecords == 0)
                {
                    result.Result = SpfResult.NONE; //https://tools.ietf.org/html/rfc7208#section-4.3
                    return SaveResultingSpf(result);
                }

                if(countResolvedTxtRecords > 1)
                {
                    result.Result = SpfResult.PERMERROR; //https://tools.ietf.org/html/rfc7208#section-4.5
                    return SaveResultingSpf(result);
                }

                var record = resolvedTxtRecords.Result.FirstOrDefault();
                
                var parsedRecord = SpfRecordParserAndEvaluate.Execute(Encoding.UTF8.GetString(record.Data));
                

                //Check(domain, received.Ip);

                
                
            }
            else
            {
                throw new NotImplementedException();
            }
            return Task.FromResult(result);
        }

        private async Task<BaseMessageSpf> SaveResultingSpf(BaseMessageSpf baseMessageSpf)
        {
            baseMessageSpf.Validated = DateTimeOffset.Now;
            _context.MessagesSpf.Add(baseMessageSpf);
            await _context.SaveChangesAsync();
            return baseMessageSpf;
        }

        private SpfResult parseSpfRules(string data)
        {
            return SpfResult.PASS;
        }
        
        private ReceivedDto parseFirstReceived(string received)
        {
            var receivedSatanized = received.ToLower();
            var fromPartIndex = receivedSatanized.IndexOf("from");
            var fromSplit = receivedSatanized.Split("from");
            var semicolonPart = receivedSatanized.Split(";");
            
            if(fromSplit.Length == 0)
                throw new Exception("Invalid format, first 'from' part not found");
            
            var firstFromSplit = fromSplit[1];

            var statingParenthesesPart = firstFromSplit.IndexOf("(");
            var endingParenthesesPart = firstFromSplit.IndexOf(")");

            var hostAndIpPart = firstFromSplit.Substring(statingParenthesesPart, endingParenthesesPart);
            var hostAndIpPartSanatized = hostAndIpPart.Trim();

            var hostAndIpApart = hostAndIpPartSanatized.Split(" ");
            if(hostAndIpApart.Length != 2)
                throw new Exception("Invalid format, host and [ip] separeted with space not found");
            
            var host = hostAndIpApart[0];
            var ip = hostAndIpApart[1];

            var strippedIp = ip.Replace("[", string.Empty).Replace("]",string.Empty).ToString();

            if(Regex.IsMatch(strippedIp, @"(?:^|(?<=\s))(([0-9a-fA-F]{1,4}:){7,7}[0-9a-fA-F]{1,4}|([0-9a-fA-F]{1,4}:){1,7}:|([0-9a-fA-F]{1,4}:){1,6}:[0-9a-fA-F]{1,4}|([0-9a-fA-F]{1,4}:){1,5}(:[0-9a-fA-F]{1,4}){1,2}|([0-9a-fA-F]{1,4}:){1,4}(:[0-9a-fA-F]{1,4}){1,3}|([0-9a-fA-F]{1,4}:){1,3}(:[0-9a-fA-F]{1,4}){1,4}|([0-9a-fA-F]{1,4}:){1,2}(:[0-9a-fA-F]{1,4}){1,5}|[0-9a-fA-F]{1,4}:((:[0-9a-fA-F]{1,4}){1,6})|:((:[0-9a-fA-F]{1,4}){1,7}|:)|fe80:(:[0-9a-fA-F]{0,4}){0,4}%[0-9a-zA-Z]{1,}|::(ffff(:0{1,4}){0,1}:){0,1}((25[0-5]|(2[0-4]|1{0,1}[0-9]){0,1}[0-9])\.){3,3}(25[0-5]|(2[0-4]|1{0,1}[0-9]){0,1}[0-9])|([0-9a-fA-F]{1,4}:){1,4}:((25[0-5]|(2[0-4]|1{0,1}[0-9]){0,1}[0-9])\.){3,3}(25[0-5]|(2[0-4]|1{0,1}[0-9]){0,1}[0-9]))(?=\s|$)"))
                throw new Exception("Invalid ip format");

            if(semicolonPart.Length != 2)
                throw new Exception("Invalid date format");

            var dataReceived = GenericUtil.CustomDateEmailFormat(semicolonPart[1]);

            if(dataReceived.HasValue)
                throw new Exception("Invalid date format");
            

            return new ReceivedDto{
                Host = host,
                Ip = IPAddress.Parse(strippedIp),
                Received = dataReceived.Value
            };
        }
    }
}