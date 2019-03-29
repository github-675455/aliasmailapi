using System.Threading.Tasks;
using AliasMailApi.Models;

namespace AliasMailApi.Interfaces
{
    public interface IMailboxService
    {
        Task import(BaseMessage message);
    }
}