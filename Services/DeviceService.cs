using aliasmailapi.Models;
using AliasMailApi.Configuration;
using AliasMailApi.Repository;
using Microsoft.Extensions.Options;

namespace aliasmailapi.Services
{
    public class DeviceService
    {
        private readonly AppOptions _options;
        private readonly MessageContext _context;
        public DeviceService(
            IOptions<AppOptions> options,
            MessageContext context)
            {
            _options = options.Value;
            _context = context;
        }

        public async void Create(Device device)
        {
            await _context.Devices.AddAsync(device);
            await _context.SaveChangesAsync();
        }
    }
}