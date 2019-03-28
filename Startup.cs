using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AliasMailApi.Configuration;
using AliasMailApi.Repository;
using AliasMailApi.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using KubeClient;
using KubeClient.Extensions.Configuration;
using Newtonsoft.Json;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;

namespace AliasMailApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var env = services.BuildServiceProvider().GetRequiredService<IHostingEnvironment>();

            services.AddOptions();
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = "127.0.0.1";
                options.InstanceName = "master";
            });

            var defaultConnection = Environment.GetEnvironmentVariable("DefaultConnection");
            var consumerToken = Environment.GetEnvironmentVariable("ConsumerToken");
            var mailgunApiTokenEnviroment = Environment.GetEnvironmentVariable("MailgunApiKey");
            var mailgunApiDomainEnviroment = Environment.GetEnvironmentVariable("MailgunDomain");

            defaultConnection = String.IsNullOrEmpty(defaultConnection) ? Configuration.GetConnectionString("DefaultConnection") : defaultConnection;

            services.AddDbContextPool<MessageContext>(options => options.UseMySql(defaultConnection));

            services.AddOptions<AppOptions>().Configure(o =>
            {
                o.mailgunApiToken = mailgunApiTokenEnviroment;
                o.consumerToken = consumerToken;
                o.mailgunApiDomain = mailgunApiDomainEnviroment;
            });

            services.Configure<AppOptions>(Configuration);
            services.AddSingleton<IMapService, MapService>();
            services.AddMvcCore().AddJsonOptions(o => {
#if DEBUG
                o.SerializerSettings.Formatting = Formatting.Indented;
#else
                o.SerializerSettings.Formatting = Formatting.None;
#endif
                o.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
            }).AddJsonFormatters().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, MessageContext context)
        {
            context.Database.Migrate();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseMvc();
        }
    }
}
