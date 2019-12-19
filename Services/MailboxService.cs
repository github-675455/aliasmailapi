using System.Net.Mail;
using System.Threading.Tasks;
using AliasMailApi.Models;
using AliasMailApi.Repository;
using Microsoft.EntityFrameworkCore;
using AliasMailApi.Interfaces;
using Microsoft.Extensions.Logging;

namespace AliasMailApi.Services
{
    public class MailboxService : IMailboxService
    {
        private readonly MessageContext _context;
        private readonly ILogger _logger;
        public MailboxService(
            MessageContext context,
            ILogger<MailboxService> logger){
            _context = context;
            _logger = logger;
        }
        public async Task<Mailbox> GetMailbox(string email){
            return await _context.Mailboxes.FirstOrDefaultAsync(e => e.Email == email);
        }
        public async Task<Mailbox> CreateMailbox(Mailbox mail){
            await _context.Mailboxes.AddAsync(mail);
            await _context.SaveChangesAsync();
            return mail;
        }

        public Mailbox CreateDefaultMailbox(MailAddress mailAdress, Domain domain){
            return new Mailbox
            {
                Email = mailAdress.Address,
                StoreQuantity = 1,
                DomainId = domain.Id,
                Domain = domain
            };
        }
    }
}