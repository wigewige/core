using GenesisVision.Core.Helpers;
using GenesisVision.Core.Helpers.TokenHelper;
using GenesisVision.Core.Services;
using GenesisVision.Core.Services.Interfaces;
using GenesisVision.Core.Services.Validators;
using GenesisVision.Core.Services.Validators.Interfaces;
using GenesisVision.DataModel;
using GenesisVision.DataModel.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Threading.Tasks;

namespace GenesisVision.Core
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
            ConfigureConstants();

            var connectionString = Configuration["DbContextSettings:ConnectionString"];
            var dbContextOptions = new Action<NpgsqlDbContextOptionsBuilder>(options => options.MigrationsAssembly("GenesisVision.Core"));

            services.AddEntityFrameworkNpgsql()
                    .AddDbContext<ApplicationDbContext>(x => x.UseNpgsql(connectionString, dbContextOptions));

            services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
                    {
                        options.Password.RequiredLength = 6;
                        options.Password.RequireLowercase = false;
                        options.Password.RequireUppercase = false;
                        options.Password.RequireNonAlphanumeric = false;
                        options.Password.RequireDigit = false;
                    })
                    .AddEntityFrameworkStores<ApplicationDbContext>()
                    .AddDefaultTokenProviders();

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                    .AddJwtBearer(options =>
                    {
                        options.TokenValidationParameters = new TokenValidationParameters
                                                            {
                                                                RequireExpirationTime = true,
                                                                RequireSignedTokens = true,

                                                                ValidateIssuer = true,
                                                                ValidateAudience = true,
                                                                ValidateLifetime = true,
                                                                ValidateIssuerSigningKey = true,

                                                                ValidIssuer = Constants.JwtValidIssuer,
                                                                ValidAudience = Constants.JwtValidAudience,
                                                                IssuerSigningKey = JwtSecurityKey.Create(Constants.JwtSecretKey)
                                                            };
                        //options.RequireHttpsMetadata = true; todo: enable it
                        options.SaveToken = true;
                        options.Events = new JwtBearerEvents
                                         {
                                             OnAuthenticationFailed = context =>
                                             {
                                                 Console.WriteLine("OnAuthenticationFailed: " + context.Exception.Message);
                                                 return Task.CompletedTask;
                                             },
                                             OnTokenValidated = context =>
                                             {
                                                 Console.WriteLine("OnTokenValidated: " + context.SecurityToken);
                                                 return Task.CompletedTask;
                                             }
                                         };
                    });

            services.AddMvcCore()
                    .AddJsonFormatters();
            
            ConfigureCustomServices(services);
        }

        private void ConfigureConstants()
        {
            var ipfsHost = Configuration["IpfsHost"];
            if (!string.IsNullOrEmpty(ipfsHost) && !string.IsNullOrWhiteSpace(ipfsHost))
                Constants.IpfsHost = ipfsHost;

            var gethHost = Configuration["GethHost"];
            if (!string.IsNullOrEmpty(gethHost) && !string.IsNullOrWhiteSpace(gethHost))
                Constants.GethHost = gethHost;


            Constants.JwtValidIssuer = Configuration["JWT:ValidIssuer"];
            Constants.JwtValidAudience = Configuration["JWT:ValidAudience"];
            Constants.JwtSecretKey = Configuration["JWT:SecretKey"];

            var expiryInMinutesStr = Configuration["JWT:ExpiryInMinutes"];
            if (!string.IsNullOrEmpty(expiryInMinutesStr) && int.TryParse(expiryInMinutesStr, out var expiryInMinutes))
                Constants.JwtExpiryInMinutes = expiryInMinutes;


            Constants.SendGridApiKey = Configuration["EmailSender:SendGrid:ApiKey"];
            Constants.SendGridFromEmail = Configuration["EmailSender:SendGrid:FromEmail"];
            Constants.SendGridFromName = Configuration["EmailSender:SendGrid:FromName"];
        }

        private void ConfigureCustomServices(IServiceCollection services)
        {
            services.AddTransient<ITrustManagementService, TrustManagementService>();
            services.AddTransient<IManagerService, ManagerService>();
            services.AddTransient<ISmartContractService, SmartContractService>();
            services.AddTransient<ITradesService, TradesService>();
            services.AddTransient<IEmailSender, EmailSender>();

            services.AddTransient<IManagerValidator, ManagerValidator>();
            services.AddTransient<IBrokerValidator, BrokerValidator>();
            services.AddTransient<IInvestorValidator, InvestorValidator>();

            services.AddSingleton<IIpfsService, IpfsService>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseAuthentication();

            app.UseMvcWithDefaultRoute();
        }
    }
}
