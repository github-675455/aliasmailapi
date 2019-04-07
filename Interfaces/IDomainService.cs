using System.Threading.Tasks;
using AliasMailApi.Models;

namespace AliasMailApi.Interfaces
{
    public interface IDomainService
    {
        Task<Domain> get(string domain);
    }
}