using System.Threading.Tasks;
using AliasMailApi.Models;
using AliasMailApi.Repository;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using AliasMailApi.Interfaces;
using System;
using Microsoft.Extensions.Logging;

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
                var attachments = JsonConvert.DeserializeObject<MailAttachment[]>(convertedMessage.MessageHeaders);
            }
            else
            {
                throw new NotImplementedException();
            }
            return Task.FromResult(result);
        }

        private object parseFirstReceived(string received)
        {
            
        }
    }
}