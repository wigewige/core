﻿using GenesisVision.Common.Services;
using GenesisVision.Common.Services.Interfaces;
using GenesisVision.Core.Helpers;
using GenesisVision.Core.Helpers.TokenHelper;
using GenesisVision.Core.Infrastructure.Filters;
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
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Swashbuckle.AspNetCore.Swagger;
using System;
using System.Globalization;
using System.IO;
using System.Linq;
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

            services.AddAuthentication(options =>
                    {
                        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                    })
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

            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder.AllowAnyOrigin()
                                      .AllowAnyMethod()
                                      .AllowAnyHeader()
                                      .AllowCredentials());
            });

            services.Configure<FormOptions>(options =>
            {
                options.MultipartBodyLengthLimit = GetMultipartBodyLengthLimit();
            });

            services.AddMemoryCache()
                    .AddMvcCore()
                    .AddApiExplorer()
                    .AddAuthorization()
                    .AddDataAnnotations()
                    .AddJsonFormatters()
                    .AddJsonOptions(options =>
                    {
                        options.SerializerSettings.DateFormatString = "yyyy-MM-ddTHH\\:mm\\:ss.fffffffzzz";
                    });

            services.AddApiVersioning(option =>
            {
                option.ReportApiVersions = true;
                option.AssumeDefaultVersionWhenUnspecified = true;
                option.DefaultApiVersion = new ApiVersion(1, 0);
                option.ApiVersionReader = new HeaderApiVersionReader("api-version");
            });

            ConfigureCustomServices(services);

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info
                                   {
                                       Title = "Core API",
                                       Version = "v1",
                                       Contact = new Contact
                                                 {
                                                     Name = "Genesis Vision",
                                                     Url = "https://genesis.vision/"
                                                 }
                                   });
                c.DescribeAllEnumsAsStrings();
                c.TagActionsBy(x => x.RelativePath.Split("/").Take(2).Last());
                c.OperationFilter<AuthorizationHeaderParameterOperationFilter>();
                c.OperationFilter<FileUploadOperation>();

                var xmlPath = Path.Combine(AppContext.BaseDirectory, "GenesisVision.Core.xml");
                c.IncludeXmlComments(xmlPath);
            });
        }

        private void ConfigureConstants()
        {
            var ipfsHost = Configuration["IpfsHost"];
            if (!string.IsNullOrEmpty(ipfsHost) && !string.IsNullOrWhiteSpace(ipfsHost))
                Constants.IpfsHost = ipfsHost;

            var gethHost = Configuration["GethHost"];
            if (!string.IsNullOrEmpty(gethHost) && !string.IsNullOrWhiteSpace(gethHost))
                Constants.GethHost = gethHost;

            var periodInMinutesStr = Configuration["IsPeriodInMinutes"];
            if (!string.IsNullOrEmpty(periodInMinutesStr) && bool.TryParse(periodInMinutesStr, out var periodInMinutes))
                Constants.IsPeriodInMinutes = periodInMinutes;


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

        private int GetMultipartBodyLengthLimit()
        {
            var multipartBodyLengthLimit = 2097152;

            var str = Configuration["MultipartBodyLengthLimit"];
            if (!string.IsNullOrEmpty(str) && int.TryParse(str, out var expiryInMinutes))
                multipartBodyLengthLimit = expiryInMinutes;

            return multipartBodyLengthLimit;
        }

        private void ConfigureCustomServices(IServiceCollection services)
        {
            services.AddTransient<ITrustManagementService, TrustManagementService>();
            services.AddTransient<IUserService, UserService>();
            services.AddTransient<IWalletService, WalletService>();
            services.AddTransient<IManagerService, ManagerService>();
            services.AddTransient<ISmartContractService, SmartContractService>();
            services.AddTransient<IStatisticService, StatisticService>();
            services.AddTransient<ITradesService, TradesService>();
            services.AddTransient<IEmailSender, EmailSender>();
            services.AddTransient<IRateService, RateService>();
            services.AddTransient<IEthService, EthService>();
            services.AddTransient<IFileService, FileService>();

            services.AddTransient<IManagerValidator, ManagerValidator>();
            services.AddTransient<IBrokerValidator, BrokerValidator>();
            services.AddTransient<IInvestorValidator, InvestorValidator>();

            services.AddSingleton<IIpfsService, IpfsService>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            Constants.UploadPath = Path.Combine(env.WebRootPath, "uploads");
            Constants.IsDevelopment = true; // todo: remove it

            if (env.IsDevelopment())
            {
                Constants.IsDevelopment = true;
                app.UseDeveloperExceptionPage();
            }

            var supportedCultures = new[]
                                    {
                                        new CultureInfo("en-US"),
                                    };
            app.UseRequestLocalization(new RequestLocalizationOptions
                                       {
                                           DefaultRequestCulture = new RequestCulture("en-US"),
                                           SupportedCultures = supportedCultures,
                                           SupportedUICultures = supportedCultures
                                       });

            app.UseCors("CorsPolicy");
            app.UseAuthentication();
            app.UseStaticFiles();
            app.UseMvcWithDefaultRoute();

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Core API v1");
            });
        }
    }
}
