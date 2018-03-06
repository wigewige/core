using GenesisVision.Common.Services;
using GenesisVision.Common.Services.Interfaces;
using GenesisVision.DataModel;
using GenesisVision.PaymentService.Models;
using GenesisVision.PaymentService.Services;
using GenesisVision.PaymentService.Services.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;

namespace GenesisVision.PaymentService
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
			var connectionString = Configuration["DbContextSettings:ConnectionString"];
			var dbContextOptions = new Action<NpgsqlDbContextOptionsBuilder>(options => options.MigrationsAssembly("GenesisVision.Core"));

			services.AddEntityFrameworkNpgsql()
					.AddDbContext<ApplicationDbContext>(x => x.UseNpgsql(connectionString, dbContextOptions));

			services.AddMvc();
			services.AddTransient<IEthService, EthService>();
			services.AddTransient<IPaymentTransactionService, PaymentTransactionService>();

			services.Configure<GvPaymentGatewayConfig>(Configuration.GetSection(nameof(GvPaymentGatewayConfig)));
			services.AddScoped(cfg => cfg.GetService<IOptionsSnapshot<GvPaymentGatewayConfig>>().Value);
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IHostingEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}

			app.UseMvc();
		}
	}
}
