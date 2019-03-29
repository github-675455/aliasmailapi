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
    public class WebhookController : ControllerBase
    {
        private readonly IMessageService _messageService;
        private readonly IMailboxService _mailboxService;
        private readonly AppOptions _options;
        private readonly IDistributedCache _cache;
        private readonly MessageContext _context;

        public WebhookController(MessageContext context, IMessageService messageService, IOptions<AppOptions> options, IDistributedCache cache, IMailboxService mailboxService) {
            _messageService = messageService;
            _options = options.Value;
            _context = context;
            _cache = cache;
            _mailboxService = mailboxService;
        }
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Post([FromForm]MailgunMessageRequest message)
        {
            var result = await _messageService.create(message);
            
            _mailboxService.import(result.Data);
            
            return Ok(result);
        }
    }
}
