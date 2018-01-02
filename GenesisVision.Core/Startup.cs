using GenesisVision.DataModel;
using GenesisVision.Core.Services;
using GenesisVision.Core.Services.Interfaces;
using GenesisVision.Core.Services.Validators;
using GenesisVision.Core.Services.Validators.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

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
            services.AddMvc();

            var connectionString = Configuration["DbContextSettings:ConnectionString"];

            var dbContextOptions = 
                new System.Action<Microsoft.EntityFrameworkCore.Infrastructure.NpgsqlDbContextOptionsBuilder>(
                    options => options.MigrationsAssembly("GenesisVision.Core"));

            services.AddEntityFrameworkNpgsql()
                    .AddDbContext<ApplicationDbContext>(x => x.UseNpgsql(connectionString, dbContextOptions));
            var ipfsHost = Configuration["IpfsHost"];
            if (!string.IsNullOrEmpty(ipfsHost) && !string.IsNullOrWhiteSpace(ipfsHost))
                Constants.IpfsHost = ipfsHost;


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

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
