using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using aliasmailapi.Utils;
using DNS.Protocol;
using System.Text;
using aliasmailapi.Interfaces;

namespace AliasMailApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        private readonly IDNSManager _dnsManager;

        public TestController(IDNSManager dnsManager)
        {
            _dnsManager = dnsManager;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var result =  await _dnsManager.Resolve("vinicius.sl", RecordType.TXT);
            return Ok(result.Select(e => Encoding.UTF8.GetString(e.Data)));
        }
    }
}
