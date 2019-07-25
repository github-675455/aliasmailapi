using System.Threading.Tasks;
using AliasMailApi.Configuration;
using AliasMailApi.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
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
        private readonly MessageContext _context;

        public WebhookController(MessageContext context, IMessageService messageService, IOptions<AppOptions> options, IMailboxService mailboxService) {
            _messageService = messageService;
            _options = options.Value;
            _context = context;
            _mailboxService = mailboxService;
        }
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Post([FromForm]MailgunMessageRequest message)
        {   
            return Ok(await _messageService.create(message));
        }
    }
}
