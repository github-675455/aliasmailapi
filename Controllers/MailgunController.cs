using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Formatting;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using AliasMailApi.Configuration;
using AliasMailApi.Models;
using AliasMailApi.Repository;
using AliasMailApi.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using AliasMailApi.Models.DTO;
using AliasMailApi.Models.DTO.Response;
using AliasMailApi.Interfaces;

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
            if(HttpContext.Request.Headers["Authorization"] != _options.consumerToken){
                return Unauthorized();
            }
            return Ok(await _context.MailgunMessages.ToListAsync());
        }

        [HttpGet("simple")]
        public async Task<IActionResult> GetSimple()
        {
            if (HttpContext.Request.Headers["Authorization"] != _options.consumerToken)
            {
                return Unauthorized();
            }

            return Ok(await _context.MailgunMessages.Select(item => _mapper.Map<SimpleMailgunResponse>(item)).ToListAsync());
        }
    }
}
