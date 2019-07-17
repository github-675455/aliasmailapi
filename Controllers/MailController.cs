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
using AutoMapper;
using AliasMailApi.Models.DTO.Response;

namespace AliasMailApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MailController : ControllerBase
    {
        private readonly IMessageService _messageService;
        private readonly IMailService _mailService;
        private readonly AppOptions _options;
        private readonly MessageContext _context;
        private readonly IMapper _mapper;

        public MailController(
            MessageContext context,
            IMessageService messageService,
            IMailService mailService,
            IMapper mapper,
            IOptions<AppOptions> options)
        {
            _messageService = messageService;
            _mailService = mailService;
            _options = options.Value;
            _mapper = mapper;
            _context = context;
        }
        [HttpPost("import")]
        public async Task<IActionResult> ImportAsync([FromBody] MailRequest mail)
        {
            var response = await _messageService.get(mail.Id);

            if (!response.Success)
            {
                return Ok(response);
            }

            var messageFound = response.Data;

            return Ok(await _mailService.import(messageFound));
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(MailgunMessageRequest message)
        {
            return Ok(await _messageService.delete(message));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            return Ok(await _context.Mails.Include(e => e.MailAttachments).FirstOrDefaultAsync(e => e.Id == Guid.Parse(id)));
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            return Ok(await _context.Mails.Include(e => e.MailAttachments).ToListAsync());
        }

        [HttpGet("simple/{skip?}/{take?}")]
        public async Task<IActionResult> GetSimple(int? skip, int? take)
        {
            if(skip.HasValue && !take.HasValue)
                return Ok(await _context.Mails.Select(item => _mapper.Map<SimpleMailResponse>(item)).Skip(skip.Value).ToListAsync());
            
            if(take.HasValue && !skip.HasValue)
                return Ok(await _context.Mails.Select(item => _mapper.Map<SimpleMailResponse>(item)).Take(take.Value).ToListAsync());

            if(take.HasValue && skip.HasValue)
                return Ok(await _context.Mails.Select(item => _mapper.Map<SimpleMailResponse>(item)).Skip(skip.Value).Take(take.Value).ToListAsync());

            return Ok(await _context.Mails.Select(item => _mapper.Map<SimpleMailResponse>(item)).ToListAsync());
        }
    }
}
