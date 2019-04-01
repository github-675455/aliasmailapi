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
using Microsoft.AspNetCore.Authorization;
using AliasMailApi.Models.DTO;
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

        public MailgunController(MessageContext context, IMessageService messageService, IOptions<AppOptions> options, IDistributedCache cache) {
            _messageService = messageService;
            _options = options.Value;
            _context = context;
            _cache = cache;
        }
        
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            if(HttpContext.Request.Headers["Authorization"] != _options.consumerToken){
                return Unauthorized();
            }
            return Ok(await _context.MailgunMessages.ToListAsync());
        }
    }
}
