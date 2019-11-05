using System.Collections.Generic;
using System.Threading.Tasks;
using AliasMailApi.Models;
using AliasMailApi.Models.DTO;

namespace AliasMailApi.Interfaces
{
    public interface IMessageService
    {
        Task<BaseResponse<MailgunMessage>> create(MailgunMessageRequest messageRequest);
        Task<BaseResponse<MailgunMessage>> delete(MailgunMessageRequest messageRequest);
        Task<BaseResponse<MailgunMessage>> update(MailgunMessageRequest messageRequest);
        Task<BaseOneResponse<MailgunMessage>> get(string id);
        Task<List<MailgunMessage>> getNextForProcessing();
    }
}