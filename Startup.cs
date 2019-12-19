using System;
using AliasMailApi.Configuration;
using AliasMailApi.Repository;
using AliasMailApi.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using AutoMapper;
using AliasMailApi.Interfaces;
using AliasMailApi.Jobs;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Internal;
using Microsoft.Extensions.Options;
using aliasmailapi.Extensions;
using Microsoft.AspNetCore.Http;

namespace AliasMailApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            var env = services.BuildServiceProvider()
            .GetRequiredService<IHostingEnvironment>();

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
            var vapidPrivateKeyEnviroment = Environment.GetEnvironmentVariable("VapidPrivateKey");
            var vapidPublicKeyEnviroment = Environment.GetEnvironmentVariable("VapidPublicKey");

            defaultConnection = String.IsNullOrEmpty(defaultConnection) ? Configuration.GetConnectionString("DefaultConnection") : defaultConnection;

            services.AddDbContextPool<MessageContext>(options => {
                options.UseMySql(defaultConnection);
            });

            services.AddHealthChecks()
                .AddMySql(defaultConnection);

            services.AddOptions<AppOptions>().Configure(o =>
            {
                o.mailgunApiToken = mailgunApiTokenEnviroment;
                o.consumerToken = consumerToken;
                o.mailgunApiDomain = mailgunApiDomainEnviroment;
                o.buildInfo = Configuration.GetSection("BuildInfo").Value;
            });

            var mappingConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new MappingProfile());
            });

            IMapper mapper = mappingConfig.CreateMapper();
            services.AddSingleton(mapper);

            services.Configure<AppOptions>(Configuration);
            services.AddTransient<IMessageService, MessageService>();
            services.AddTransient<IDomainService, DomainService>();
            services.AddTransient<IMailboxService, MailboxService>();
            services.AddTransient<IMailService, MailService>();
            services.AddTransient<IMailgunAttachment, MailgunAttachmentService>();
            services.AddTransient<IAuthenticationService, AuthenticationService>();
            services.AddHostedService<MailImportJob>();
            services.AddHttpContextAccessor();
            services.AddMvcCore().AddJsonOptions(o => {
#if DEBUG
                o.SerializerSettings.Formatting = Formatting.Indented;
#else
                o.SerializerSettings.Formatting = Formatting.None;
#endif
                o.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
            }).AddJsonFormatters().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, MessageContext context, IOptions<AppOptions> options)
        {
            var dropDatabase = Environment.GetEnvironmentVariable("DropDatabase");
            if(!string.IsNullOrWhiteSpace(dropDatabase))
            {
                context.Database.EnsureDeleted();
            }

            ResultExtension.Configure(app.ApplicationServices.GetRequiredService<IHttpContextAccessor>());

            context.Database.Migrate();

            var seedData = Environment.GetEnvironmentVariable("SeedData");
            if(!string.IsNullOrWhiteSpace(seedData))
            {
                context.Database.EnsureCreated();
            }

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseHealthChecks("/health", new HealthJsonResult(options));

            app.UseEndpointRouting();
            app.UseMiddleware<AutorizationMiddleware>();

            app.UseMvc();
        }
    }
}
