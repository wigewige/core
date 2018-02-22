using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace GV.Payment.Api
{
	public class ApiKeyAuthHandler : AuthenticationHandler<ApiKeyAuthOptions>
	{
		private readonly ILoggerFactory logger;

		public ApiKeyAuthHandler(IOptionsMonitor<ApiKeyAuthOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock) : base(options, logger, encoder, clock)
		{
			this.logger = logger;
		}

		protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
		{
			// Get Authorization header value
			if (!Request.Headers.TryGetValue(HeaderNames.Authorization, out var authorization))
			{
				return AuthenticateResult.Fail($"Cannot read {HeaderNames.Authorization} header.");
			}

			var apiKey = authorization.First();
			if (!Options.ApiKeys.ContainsKey(apiKey))
			{
				return AuthenticateResult.Fail("ApiKey not found");
			}

			var userId = Options.ApiKeys[apiKey].ToString();

			var identities = new List<ClaimsIdentity>
			{
				new ClaimsIdentity(
					new List<Claim>
					{
						new Claim(ClaimTypes.Name, apiKey, ClaimValueTypes.String),
						new Claim(ClaimTypes.NameIdentifier, userId, ClaimValueTypes.Integer)
					},
					ApiKeyAuthOptions.DefaultScheme
				)
			};

			var ticket = new AuthenticationTicket(new ClaimsPrincipal(identities), Options.Scheme);

			return AuthenticateResult.Success(ticket);
		}
	}

}
