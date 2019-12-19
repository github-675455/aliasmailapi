using System.Threading.Tasks;
using aliasmailapi.Models;
using aliasmailapi.Services;
using AliasMailApi.Configuration;
using AliasMailApi.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace aliasmailapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        private readonly NotificationService _notificationService;
        private readonly AppOptions _options;
        private readonly MessageContext _context;

        public NotificationController(MessageContext context, NotificationService notificationService, IOptions<AppOptions> options) {
            _notificationService = notificationService;
            _options = options.Value;
            _context = context;
        }
        
        [HttpPost]
        public async Task<IActionResult> Post([Bind("PushEndpoint,PushP256DH,PushAuth")] DeviceSubscription device)
        {
            _context.DevicesSubscriptions.Add(device);
            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}