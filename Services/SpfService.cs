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

namespace AliasMailApi.Services
{
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

                ClientRequest request = new ClientRequest(DnsServers.CloudFlare);

                GenericUtil.TreatEmptyMailAddress(convertedMessage.Sender, EmailSection.Address);

                request.Questions.Add(new Question(DNS.Protocol.Domain.FromString(convertedMessage.Sender), RecordType.TXT));
                request.RecursionDesired = true;

                var taskResolution = request.Resolve();
                Task.WaitAll(taskResolution);

                IResponse response = taskResolution.Result;

                // IList<string> spfRules = response.AnswerRecords
                //     .Where(e => isSpfRule(e.Data))
                //     .Select(e => parseSpfRules(e.Data))
                //     .ToList();

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

        private SpfResult parseSpfRules(byte[] data)
        {
            var dataEncoded = Encoding.UTF8.GetString(data);



            return SpfResult.PASS;
        }

        private bool isSpfRule(byte[] data)
        {
            return Encoding.UTF8.GetString(data).Trim().IndexOf("v=spf1") != -1;
        }

        //from mta2.e.mozilla.org (mta2.e.mozilla.org [68.232.195.239]) by mxa.mailgun.org with ESMTP id 5d324468.7f2d1cc65ca8-smtp-in-n02; Fri, 19 Jul 2019 22:30:00 -0000 (UTC)
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
                Ip = strippedIp,
                Received = dataReceived.Value
            };
        }
    }
}