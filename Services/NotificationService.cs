using AliasMailApi.Configuration;
using AliasMailApi.Repository;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WebPush;

namespace aliasmailapi.Services
{
    public class NotificationService
    {
        private readonly AppOptions _options;
        private readonly MessageContext _context;
        private readonly ILogger _logger;
        public NotificationService(
            IOptions<AppOptions> options,
            MessageContext context,
            ILogger<NotificationService> logger)
            {
            _options = options.Value;
            _context = context;
            _logger = logger;
        }

        public void send(string message)
        {
            var devicesSubscriptions = _context.DevicesSubscriptions;
            foreach (var device in devicesSubscriptions)
            {
                var subscription = new PushSubscription(device.PushEndpoint, device.PushP256DH, device.PushAuth);
                var vapidDetails = new VapidDetails(message, _options.vapidPublicKeyEnviroment, _options.vapidPrivateKeyEnviroment);

                var webPushClient = new WebPushClient();
                try
                {
                    webPushClient.SendNotification(subscription, "payload", vapidDetails);
                    //device.RegistrationId
                    _logger.LogInformation("Message push send: {message}", message);
                    //webPushClient.SendNotification(subscription, "payload", gcmAPIKey);
                }
                catch (WebPushException exception)
                {
                    _logger.LogInformation("Message: {Message}", exception.Message);
                    //Console.WriteLine("Http STATUS code" + exception.StatusCode);
                }
            }
        }
    }
}