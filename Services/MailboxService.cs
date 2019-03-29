using System.Net.Mail;
using System.Threading.Tasks;
using AliasMailApi.Models;
using AliasMailApi.Repository;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace AliasMailApi.Services
{
    public class MailboxService : IMailboxService
    {
        private readonly MessageContext _context;
        private readonly IMessageService _messageService;
        public MailboxService(MessageContext context, IMessageService messageService)
        {
            _context = context;
            _messageService = messageService;
        }

        public async Task<Domain> GetDomain(string domain){
            return await _context.Domains.FirstOrDefaultAsync(e => e.Name == domain);
        }
        public async Task<Mailbox> GetMailbox(string email){
            return await _context.Mailboxes.FirstOrDefaultAsync(e => e.Email == email);
        }
        public async void CreateMailbox(Mailbox mail){
            await _context.Mailboxes.AddAsync(mail);
            await _context.SaveChangesAsync();
        }

        public async void import(BaseMessage message)
        {
            if(!message.Valid)
            {
                return;
            }
            
            if(message is MailgunMessage)
            {
                var mailgunMessage = (MailgunMessage)message;
                var toEmail = new MailAddress(mailgunMessage.To);
                
                var domainFound = await GetDomain(toEmail.Host);

                if(domainFound == null){
                    //mark to delete mailgunmMessage
                    return;
                }

                var mailboxFound = await GetMailbox(toEmail.Address);
                
                if(mailboxFound == null){
                    var newMailbox = new Mailbox{
                        Email = toEmail.Address,
                        StoreQuantity = 1,
                        Description = toEmail.DisplayName,
                        DomainId = domainFound.Id,
                        Domain = domainFound
                    };
                    CreateMailbox(newMailbox);
                }
            }
        }
    }
}