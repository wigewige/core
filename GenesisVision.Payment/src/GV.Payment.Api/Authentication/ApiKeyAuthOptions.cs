using Microsoft.AspNetCore.Authentication;
using System.Collections.Generic;

namespace GV.Payment.Api
{
	public class ApiKeyAuthOptions : AuthenticationSchemeOptions
	{
		public const string DefaultScheme = "ApiKey";

		public string Scheme => DefaultScheme;
		public Dictionary<string, int> ApiKeys { get; set; }
		public ApiKeyAuthOptions()
		{

		}
	}

}
