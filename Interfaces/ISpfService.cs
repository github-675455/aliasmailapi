using System.Threading.Tasks;
using AliasMailApi.Models;

namespace AliasMailApi.Interfaces
{
    public interface ISpfService
    {
        Task<BaseMessageSpf> validate(BaseMessage message);
    }
}