using System.Threading.Tasks;
using AliasMailApi.Models;

namespace AliasMailApi.Interfaces
{
    public interface IMailService
    {
        Task<BaseResponse<BaseMessage>> process(BaseMessage message);
    }
}