using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace GV.Payment.Api.Controllers
{
	[Authorize]
	[Route("api/ping")]
	public class PingController : Controller
	{
		[HttpGet]
		public string Get()
		{
			return $"Pong {DateTime.UtcNow}";
		}
	}
}
