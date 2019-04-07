using System.Threading.Tasks;
using AliasMailApi.Interfaces;
using AliasMailApi.Models;
using AliasMailApi.Repository;
using Microsoft.EntityFrameworkCore;

namespace AliasMailApi.Services
{
    public class DomainService : IDomainService
    {
        private readonly MessageContext _context;

        public DomainService(MessageContext context)
        {
            _context = context;
        }

        public async Task<Domain> get(string domain)
        {
            return await _context.Domains.FirstOrDefaultAsync(e => e.Name == domain);
        }
    }
}