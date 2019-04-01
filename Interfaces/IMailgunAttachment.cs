using System.Collections.Generic;
using System.Threading.Tasks;
using AliasMailApi.Models;

namespace AliasMailApi.Interfaces
{
    public interface IMailgunAttachment
    {
        Task<MailAttachment> get(MailAttachment mailAttachment);
    }
}