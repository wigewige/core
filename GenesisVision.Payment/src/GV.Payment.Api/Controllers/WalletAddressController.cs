using GV.Payment.Api.Models;
using GV.Payment.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace GV.Payment.Api.Controllers
{
	[Route("api/walletAddress")]
	public class WalletAddressController : Controller
	{
		private readonly PaymentWalletService walletService;

		public WalletAddressController(PaymentWalletService walletService)
		{
			this.walletService = walletService;
		}
				
		[HttpPost]
		public async Task<WalletResponse> Create([FromBody]WalletRequest request)
		{
			var rs = await walletService.GenerateWallet(request.Currency, User.Identity.Name, request.PayoutAddress);

			return new WalletResponse()
			{
				Address = rs.Address,
				Currency = rs.Currency,
				WalletId = rs.WalletId.ToString(),
				VerificationCode = rs.VerificationCode
			};
		}

		//[HttpGet]
		//[Route("initData")]
		//[SwaggerResponse(StatusCodes.Status200OK, Type = typeof(TradeServerViewModel))]
		//[SwaggerResponse(StatusCodes.Status400BadRequest, Type = typeof(ErrorViewModel))]
		// GET api/values/5
		[HttpGet("{currency}/{address}")]
		[ProducesResponseType(typeof(WalletBalance), 200)]
		[ProducesResponseType(404)]
		[ProducesResponseType(400)]
		public string Get(string currency, string address)
		{
			return "value";
		}
	}
}
