using System.Threading.Tasks;
using AliasMailApi.Configuration;
using AliasMailApi.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;
using AliasMailApi.Interfaces;
using aliasmailapi.Extensions;
using System;

namespace AliasMailApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AttachmentController : ControllerBase
    {
        private readonly IMessageService _messageService;
        private readonly AppOptions _options;
        private readonly MessageContext _context;

        public AttachmentController(MessageContext context, IMessageService messageService, IOptions<AppOptions> options) {
            _messageService = messageService;
            _options = options.Value;
            _context = context;
        }
        
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            return Ok(await _context.MailAttachments.GetPagedResult());
        }
        
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var mailAttachmentEncontrado = await _context.MailAttachments.FirstAsync(e => e.Id == id);
            _context.MailAttachments.Remove(mailAttachmentEncontrado);
            await _context.SaveChangesAsync();
            return Ok(mailAttachmentEncontrado.FormatOneResult());
        }
    }
}
