using AliasMailApi.Models;

namespace AliasMailApi.Interfaces
{
    public interface IMailboxService
    {
        void import(BaseMessage message);
    }
}