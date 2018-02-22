using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.PlatformAbstractions;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace GV.Payment.Api
{
	public class AuthorizationHeaderParameterOperationFilter : IOperationFilter
	{
		public void Apply(Operation operation, OperationFilterContext context)
		{
			var filterPipeline = context.ApiDescription.ActionDescriptor.FilterDescriptors;
			var isAuthorized = filterPipeline.Select(filterInfo => filterInfo.Filter).Any(filter => filter is AuthorizeFilter);
			var allowAnonymous = filterPipeline.Select(filterInfo => filterInfo.Filter).Any(filter => filter is IAllowAnonymousFilter);

			if (!isAuthorized || allowAnonymous)
				return;

			if (operation.Parameters == null)
				operation.Parameters = new List<IParameter>();

			operation.Parameters.Add(new NonBodyParameter
			{
				Name = "Authorization",
				In = "header",
				Description = "ApiKey",
				Required = true,
				Type = "string"
			});
		}
	}

	public class AuthResponsesOperationFilter : IOperationFilter
	{
		public void Apply(Operation operation, OperationFilterContext context)
		{
			var authAttributes = context.ApiDescription
				.ControllerAttributes()
				.Union(context.ApiDescription.ActionAttributes())
				.OfType<AuthorizeAttribute>();

			if (authAttributes.Any())
				operation.Responses.Add("401", new Response { Description = "Unauthorized" });
		}
	}

	public class Startup
	{
		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		private string GetXmlCommentsPath()
		{
			var basePath = PlatformServices.Default.Application.ApplicationBasePath;
			var xmlPath = Path.Combine(basePath, "GV.Payment.Api.xml");
			return xmlPath;
		}

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddMvc();
			services.AddSwaggerGen(c =>
			{
				c.IncludeXmlComments(GetXmlCommentsPath());
				c.DescribeAllEnumsAsStrings();
				c.SwaggerDoc("v1", new Info { Title = "Genesis Payment API", Version = "v1" });
				c.OperationFilter<AuthorizationHeaderParameterOperationFilter>();
			});

			services.AddAuthentication(options =>
			{
				// the scheme name has to match the value we're going to use in AuthenticationBuilder.AddScheme(...)
				options.DefaultAuthenticateScheme = ApiKeyAuthOptions.DefaultScheme;
				options.DefaultChallengeScheme = ApiKeyAuthOptions.DefaultScheme;
			})
			.AddApiKeyAuth(o =>
			{
				o.ApiKeys = new Dictionary<string, int>
				{
					{ "TESTAPIKEY1", 1 },
					{ "TESTAPIKEY2", 2 }
				};
			});
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IHostingEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}

			app.UseSwagger();

			app.UseAuthentication();

			// Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), specifying the Swagger JSON endpoint.
			app.UseSwaggerUI(c =>
			{
				c.SwaggerEndpoint("/swagger/v1/swagger.json", "Genesis Payment API V1");
			});
			app.UseMvc();
		}
	}
}
