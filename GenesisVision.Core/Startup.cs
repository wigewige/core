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

            services.AddIdentity<ApplicationUser, ApplicationRole>()
                    .AddEntityFrameworkStores<ApplicationDbContext>()
                    .AddDefaultTokenProviders();

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                    .AddJwtBearer(options =>
                    {
                        options.TokenValidationParameters = new TokenValidationParameters
                                                            {
                                                                ValidateIssuer = true,
                                                                ValidateAudience = true,
                                                                ValidateLifetime = true,
                                                                ValidateIssuerSigningKey = true,

                                                                ValidIssuer = "GenesisVision.Core",
                                                                ValidAudience = "GenesisVision.Core",
                                                                IssuerSigningKey = JwtSecurityKey.Create(Constants.SecretKey)
                                                            };

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
            
            services.AddMvc();
            
            ConfigureCustomServices(services);
        }

        private void ConfigureConstants()
        {
            var ipfsHost = Configuration["IpfsHost"];
            if (!string.IsNullOrEmpty(ipfsHost) && !string.IsNullOrWhiteSpace(ipfsHost))
                Constants.IpfsHost = ipfsHost;

            Constants.SecretKey = Configuration["SecretKey"];
        }

        private void ConfigureCustomServices(IServiceCollection services)
        {
            services.AddTransient<ITrustManagementService, TrustManagementService>();
            services.AddTransient<IManagerService, ManagerService>();
            services.AddTransient<ISmartContractService, SmartContractService>();
            services.AddTransient<ITradesService, TradesService>();

            services.AddTransient<IManagerValidator, ManagerValidator>();
            services.AddTransient<IBrokerValidator, BrokerValidator>();
            services.AddTransient<IInvestorValidator, InvestorValidator>();

            services.AddSingleton<IIpfsService, IpfsService>();
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
