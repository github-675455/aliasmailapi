using System.Net.Mail;
using System.Threading.Tasks;
using AliasMailApi.Models;
using AliasMailApi.Repository;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using AliasMailApi.Interfaces;
using AutoMapper;

namespace AliasMailApi.Services
{
    public class MailboxService : IMailboxService
    {
        private readonly MessageContext _context;
        private readonly IMessageService _messageService;
        private readonly IMapper _mapper;
        public MailboxService(MessageContext context, IMessageService messageService, IMapper mapper)
        {
            _context = context;
            _messageService = messageService;
            _mapper = mapper;
        }

        public async Task<Domain> GetDomain(string domain){
            return await _context.Domains.FirstOrDefaultAsync(e => e.Name == domain);
        }
        public async Task<Mailbox> GetMailbox(string email){
            return await _context.Mailboxes.FirstOrDefaultAsync(e => e.Email == email);
        }
        public async Task<Mailbox> CreateMailbox(Mailbox mail){
            await _context.Mailboxes.AddAsync(mail);
            await _context.SaveChangesAsync();
            return mail;
        }

        public async Task import(BaseMessage message)
        {
            if(message == null){
                return;
            }

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
                    mailboxFound = await CreateMailbox(newMailbox);
                }

                try{
                    
                    var mail = _mapper.Map<Mail>(mailgunMessage);

                    await _context.Mails.AddAsync(mail);
                    await _context.SaveChangesAsync();
                }
                catch(AutoMapperMappingException autoMapperException)
                {
                    mailgunMessage.Error = true;
                    mailgunMessage.ErrorMessage = autoMapperException.InnerException.Message;
                }
            }
        }
    }
}