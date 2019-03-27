using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using KubeClient;
using KubeClient.Extensions.Configuration;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace AliasMailApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateWebHostBuilder(args);

            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            var isProduction = environment == EnvironmentName.Production;

            if (isProduction)
                host.ConfigureAppConfiguration(
                    configuration => configuration.AddKubeSecret(
                        clientOptions: KubeClientOptions.FromPodServiceAccount(),
                        secretName: "mailgun",
                        kubeNamespace: "default",
                        reloadOnChange: true
                    )
                );
            
            host.Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();
    }
}
