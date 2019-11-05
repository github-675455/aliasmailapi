using System.Threading.Tasks;
using AliasMailApi.Configuration;
using AliasMailApi.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Caching.Distributed;
using AutoMapper;
using AliasMailApi.Models.DTO.Response;
using AliasMailApi.Interfaces;
using System.Linq;
using aliasmailapi.Extensions;
using System;
using AliasMailApi.Models.DTO;

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
            return Ok(await _context.MailgunMessages.GetPagedResult());
        }
        
        [HttpDelete]
        public async Task<IActionResult> Delete([FromBody] MailgunMessageRequest request)
        {
            return Ok(await _messageService.delete(request));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            return Ok(await _context.MailgunMessages
                                    .GetOneResult(e => e.Id == id));
        }

        [HttpGet("simple")]
        public async Task<IActionResult> GetSimple()
        {
            return Ok(await _context.MailgunMessages
                                    .Select(item => _mapper.Map<SimpleMailgunResponse>(item))
                                    .GetPagedResult());
        }

        [HttpGet("simple/{id}")]
        public async Task<IActionResult> GetSimple(Guid id)
        {
            return Ok(await _context.MailgunMessages
                                    .Where(e => e.Id.Equals(id))
                                    .Select(item => _mapper.Map<SimpleMailgunResponse>(item))
                                    .FormatOneResult());
        }
    }
}
