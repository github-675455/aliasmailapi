using System;
using System.Threading;
using System.Threading.Tasks;
using AliasMailApi.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace AliasMailApi.Jobs
{
    public class MailImportJob : BackgroundService
    {
        private readonly ILogger _logger;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        public MailImportJob(ILogger<MailImportJob> logger, IServiceScopeFactory serviceScopeFactory){
            _logger = logger;
            _serviceScopeFactory = serviceScopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var mailService = scope.ServiceProvider.GetService<IMailService>();
                var messageService = scope.ServiceProvider.GetService<IMessageService>();

                while (!stoppingToken.IsCancellationRequested)
                {
                    _logger.LogInformation("Starting to import mail at {Now}", DateTime.Now);

                    var result = await messageService.getNextForProcessing();
                    _logger.LogInformation("Processing {Count} itens", result.Count);
                    foreach(var item in result)
                    {
                        _logger.LogInformation("Processing item {Id}", item.Id);
                        var itemResult = await mailService.import(item);
                        foreach(var error in itemResult.Errors){
                            _logger.LogError("Error {description} ", error.description);
                        }
                        _logger.LogInformation("Processing item {Id} ended", item.Id);
                    }
                    
                    await Task.Delay(TimeSpan.FromMilliseconds(10), stoppingToken);
                    _logger.LogInformation("Ending to import mail at {Now}", DateTime.Now);
                }
                _logger.LogInformation("Grace period, ending to import mail at {Now}", DateTime.Now);
            }
        }
    }
}