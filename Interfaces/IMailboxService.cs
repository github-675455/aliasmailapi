using System.Net.Mail;
using System.Threading.Tasks;
using AliasMailApi.Models;

namespace AliasMailApi.Interfaces
{
    public interface IMailboxService
    {
        Task<Mailbox> GetMailbox(string email);
        Task<Mailbox> CreateMailbox(Mailbox mail);
        Mailbox CreateDefaultMailbox(MailAddress mailAdress, Domain domain);
    }
}