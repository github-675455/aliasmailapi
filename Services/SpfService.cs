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
using DNS.Client;
using DNS.Protocol;
using AliasMailApi.Models.Enum;
using DNS.Protocol.ResourceRecords;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using Polly;
using Polly.Timeout;

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
    public abstract class SpfDirective<T>
    {
        public SpfQualifier Qualifier = SpfQualifier.PLUS;
        public SpfMechanism Mechanism;
        public T MecanismParameter;
        public int netMask;
        public abstract bool Matches(string domain, string sender);
        public SpfResult Result;

        protected string convertToEncodedString(byte[] data)
        {
            return Encoding.UTF8.GetString(data);
        }
    }
    public sealed class SpfIpv4 : SpfDirective<IPSubnet>
    {
        public SpfIpv4()
        {
            Mechanism = SpfMechanism.IPv4;
        }
        public override bool Matches(string domain, string sender)
        {
            var localResult = MecanismParameter.Contains(Encoding.UTF8.GetBytes(sender));
            
            Result = localResult ? SpfResult.PASS : SpfResult.NONE;

            return localResult;
        }
    }

    public sealed class SpfIpv6 : SpfDirective<IPSubnet>
    {
        public SpfIpv6()
        {
            Mechanism = SpfMechanism.IPv6;
        }
        public override bool Matches(string domain, string sender)
        {
            var localResult = MecanismParameter.Contains(Encoding.UTF8.GetBytes(sender));
            
            Result = localResult ? SpfResult.PASS : SpfResult.NONE;
            
            return localResult;
        }
    }

    public sealed class SpfRecordAorAAAA : SpfDirective<string>
    {
        public SpfRecordAorAAAA()
        {
            Mechanism = SpfMechanism.AorAAAA;
        }
        public override bool Matches(string domain, string sender)
        {
            IPAddress senderIp = IPAddress.Parse(sender);
            var isIpv4 = senderIp.AddressFamily == AddressFamily.InterNetwork;

            netMask = isIpv4 ? 32: 128;

            var response = DNSManager.Resolve(domain, isIpv4 ?  RecordType.A : RecordType.AAAA);

            var ipsRecords = response
                .Select(e => IPAddress.Parse(convertToEncodedString(e.Data)))
                .ToList();

            var ipsCount = ipsRecords.Count();

            if(ipsCount == 0)
                Result = SpfResult.NONE; //https://tools.ietf.org/html/rfc7208#section-4.3

            if(ipsCount > 1)
                Result = SpfResult.PERMERROR; //https://tools.ietf.org/html/rfc7208#section-4.5

            var ipMatch = ipsRecords.Any(e => e.Equals(senderIp));

            if(ipMatch)
                Result = SpfResult.PASS;

            return ipMatch;
        }
    }

    public sealed class SpfRecordMX : SpfDirective<string>
    {
        public SpfRecordMX()
        {
            Mechanism = SpfMechanism.MX;
        }

        public override bool Matches(string domain, string sender)
        {
            throw new NotImplementedException();
        }
    }

    public sealed class SpfRecordInclude : SpfDirective<string>
    {
        public SpfRecordInclude()
        {
            Mechanism = SpfMechanism.INCLUDE;
        }

        public override bool Matches(string domain, string sender)
        {
            throw new NotImplementedException();
        }
    }

    public sealed class SpfRecordAll : SpfDirective<string>
    {
        public SpfRecordAll()
        {
            Mechanism = SpfMechanism.ALL;
        }

        public override bool Matches(string domain, string sender)
        {
            throw new NotImplementedException();
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
        PLUS,
        DASH,
        QUESTION,
        TILDE
    }
    public static class DNSManager
    {
        public static IList<IResourceRecord> Resolve(string domain, RecordType recordType)
        {
            IResponse response = null;

            var timeoutPolicy = Policy.Timeout(1);

            var waitAndRetryPolicy = Policy
                .Handle<ResponseException>()
                .Or<TimeoutRejectedException>()
                .WaitAndRetry(3, (c, t) => TimeSpan.FromMilliseconds(100));

            waitAndRetryPolicy.Execute(() =>
                timeoutPolicy.Execute(() =>
                {
                    ClientRequest request = new ClientRequest(DnsServers.CloudFlare);

                    request.Questions.Add(new Question(DNS.Protocol.Domain.FromString(domain), recordType));
                    request.RecursionDesired = true;           

                    var taskResolution = request.Resolve();
                    taskResolution.Start();
                    taskResolution.Wait();

                    response = taskResolution.Result;
                })
            );

            return response.AnswerRecords;
        }
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

                Check(domain, received.Ip);

                var baseMessageSpf = new BaseMessageSpf();
                //baseMessageSpf. = received.Ip;
                _context.MessagesSpf.Add(baseMessageSpf);
                _context.SaveChanges();
            }
            else
            {
                throw new NotImplementedException();
            }
            return Task.FromResult(result);
        }

        private SpfChecker Check(string domain, IPAddress sendingIp)
        {
            
            

            return null;
        }

        private SpfResult parseSpfRules(string data)
        {

            return SpfResult.PASS;
        }
        private bool isSpfRecord(string data)
        {
            return data.Trim().IndexOf("v=spf1") != -1;
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