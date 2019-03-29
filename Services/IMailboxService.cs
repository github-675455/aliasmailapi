using AliasMailApi.Models;

namespace AliasMailApi.Services
{
    public interface IMailboxService
    {
        void import(BaseMessage message);
    }
}