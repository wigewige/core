using GenesisVision.Common.Models;
using GenesisVision.DataModel.Enums;
using GenesisVision.PaymentService.Models;
using GenesisVision.PaymentService.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace GenesisVision.PaymentService.Controllers
{
	[Route("PaymentCallback")]
	public class PaymentCallbackController : Controller
	{
		private ILogger<PaymentCallbackController> logger;
		private IPaymentTransactionService paymentService;
		private readonly GvPaymentGatewayConfig config;

		public PaymentCallbackController(ILogger<PaymentCallbackController> logger, IPaymentTransactionService paymentsService, GvPaymentGatewayConfig config)
		{
			this.logger = logger;
			this.paymentService = paymentsService;
			this.config = config;
		}

		[Route("index")]
		[HttpGet]
		public ActionResult Index()
		{
			return Ok(nameof(PaymentCallbackController));
		}

		[Route("notify")]
		[HttpPost]
		public async Task<ActionResult> Notify([FromQuery]string customKey, [FromHeader]string HMAC, [FromForm]PaymentNotifyRequest model)
		{
			try
			{
				logger.LogInformation("Start processing callback {Notify} ", nameof(Notify), customKey);

				if (ModelState.IsValid)
				{
					var calculatedHMAC = CalculateHMACSHA512Hex(Request.Form);

					var request = new ProcessPaymentTransaction()
					{
						TransactionHash = model.Tx_hash,
						Address = model.Address,
						Amount = model.Amount,
						Currency = Enum.Parse<Currency>(model.Currency), // TODO
						Status = model.IsConfirmed ? PaymentTransactionStatus.ConfirmedAndValidated : PaymentTransactionStatus.Pending,
						CustomKey = customKey // TODO check
					};

					var rs = await paymentService.ProcessCallback(request);
					if (rs.IsValid)
					{
						logger.LogInformation("End processing Notify");
					}
					else
					{
						logger.LogError("End processing Notify with Error");
					}
				}
				else
				{
					logger.LogError("ModelState Error", JsonConvert.SerializeObject(ModelState));
				}
			}
			catch (Exception e)
			{
				logger.LogError("paymentTransactionService.ProcessCallback Exception {error}", e.ToString());
			}

			logger.LogInformation("Finish processing callback {Notify} {CustomKey}", nameof(Notify), customKey);

			return Content("OK");
		}

		private string CalculateHMACSHA512Hex(IFormCollection request)
		{
			var requestContent = string.Join("&", request.OrderBy(x => x.Key).Select(p => $"{p.Key}={Uri.EscapeDataString(p.Value)}"));
			var key = Encoding.UTF8.GetBytes(config.ApiSecret);
			using (var hm = new HMACSHA512(key))
			{
				var signed = hm.ComputeHash(Encoding.UTF8.GetBytes(requestContent));
				return BitConverter.ToString(signed).Replace("-", string.Empty);
			}
		}
	}
}
