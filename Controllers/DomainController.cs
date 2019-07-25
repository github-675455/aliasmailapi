using System.Threading.Tasks;
using AliasMailApi.Configuration;
using AliasMailApi.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;
using AliasMailApi.Interfaces;
using aliasmailapi.Extensions;

namespace AliasMailApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DomainController : ControllerBase
    {
        private readonly IMessageService _messageService;
        private readonly AppOptions _options;
        private readonly MessageContext _context;

        public DomainController(MessageContext context, IMessageService messageService, IOptions<AppOptions> options) {
            _messageService = messageService;
            _options = options.Value;
            _context = context;
        }
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            return Ok(await _context.Domains.GetPagedResult());
        }
    }
}
