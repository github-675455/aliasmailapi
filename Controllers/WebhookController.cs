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

namespace AliasMailApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WebhookController : ControllerBase
    {
        private readonly IMapService _mapService;
        private readonly IOptions<AppOptions> _options;
        private readonly IDistributedCache _cache;
        private readonly MessageContext _context;

        public WebhookController(MessageContext context, IMapService mapService, IOptions<AppOptions> options, IDistributedCache cache) {
            _mapService = mapService;
            _options = options;
            _context = context;
            _cache = cache;
        }
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Post([FromForm]IFormCollection formDataCollection)
        {
            string value = _options.Value.mailgunApiToken;
            var obj = _mapService.mapFrom(formDataCollection);
            var tokenFound = await _context.MailGunMessages.FirstOrDefaultAsync(e => e.Token == obj.Token);

            if(tokenFound != null)
                return BadRequest(new BaseResponse<MailGunMessage>{ Errors = new ApiError[]{ new ApiError{ description = "Token already used." } } });
            
            var mailgunApiToken = Encoding.ASCII.GetBytes(_options.Value.mailgunApiToken);
            var hash = new HMACSHA256(mailgunApiToken);
            if(BitConverter.ToString(hash.ComputeHash(Encoding.ASCII.GetBytes(obj.Timestamp + obj.Token))).Replace("-", "").ToLower().Equals(obj.Signature))
            {
                obj.Valid = true;
            }
            obj.Validated = DateTime.Now;
            obj.Host = HttpContext.Request.Host.ToString();
            
            _context.Add(obj);
            _context.SaveChanges();

            return Ok();
        }

        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
        
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            if(HttpContext.Request.Headers["Authorization"] != _options.Value.consumerToken){
                return Unauthorized();
            }
            return Ok(await _context.MailGunMessages.ToListAsync());
        }
    }
}
