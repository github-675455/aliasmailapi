using System.Net.Http.Formatting;
using System.Threading.Tasks;
using AliasMailApi.Models;
using AliasMailApi.Models.DTO;
using Microsoft.AspNetCore.Http;

namespace AliasMailApi.Services
{
    public interface IMessageService
    {
        Task<BaseResponse<MailgunMessage>> create(MailgunMessageRequest messageRequest);
        Task<BaseResponse<MailgunMessage>> delete(MailgunMessageRequest messageRequest);
        Task<BaseResponse<MailgunMessage>> update(MailgunMessageRequest messageRequest);
        Task<BaseResponse<MailgunMessage>> get(string id);
    }
}