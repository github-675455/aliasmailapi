using System.Threading.Tasks;
using AliasMailApi.Configuration;
using AliasMailApi.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using AutoMapper;
using AliasMailApi.Models.DTO.Response;
using AliasMailApi.Interfaces;
using System.Linq;

namespace AliasMailApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MailgunController : ControllerBase
    {
        private readonly IMessageService _messageService;
        private readonly AppOptions _options;
        private readonly IDistributedCache _cache;
        private readonly MessageContext _context;
        private readonly IMapper _mapper;

        public MailgunController(
            MessageContext context,
            IMessageService messageService,
            IOptions<AppOptions> options,
            IDistributedCache cache,
            IMapper mapper) {
            _messageService = messageService;
            _options = options.Value;
            _context = context;
            _cache = cache;
            _mapper = mapper;
        }
        
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            return Ok(await _context.MailgunMessages.ToListAsync());
        }

        [HttpGet("simple/{skip?}/{take?}")]
        public async Task<IActionResult> GetSimple(int? skip, int? take)
        {
            if(skip.HasValue && !take.HasValue)
                return Ok(await _context.MailgunMessages.Select(item => _mapper.Map<SimpleMailgunResponse>(item)).Skip(skip.Value).ToListAsync());
            
            if(take.HasValue && !skip.HasValue)
                return Ok(await _context.MailgunMessages.Select(item => _mapper.Map<SimpleMailgunResponse>(item)).Take(take.Value).ToListAsync());

            if(take.HasValue && skip.HasValue)
                return Ok(await _context.MailgunMessages.Select(item => _mapper.Map<SimpleMailgunResponse>(item)).Skip(skip.Value).Take(take.Value).ToListAsync());

            return Ok(await _context.MailgunMessages.Select(item => _mapper.Map<SimpleMailgunResponse>(item)).ToListAsync());
        }
    }
}
