using GenesisVision.DataModel.Enums;
using GenesisVision.PaymentService.Models;
using GenesisVision.PaymentService.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
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
        public async Task<ActionResult> Notify([FromQuery]string customKey, [FromForm]PaymentNotifyRequest model)
        {
            try
            {
                logger.LogInformation("Start processing callback {Notify} ", nameof(Notify), customKey);

				if (ModelState.IsValid)
                {
					var requestContent = string.Join("&", Request.Form.Select(p => $"{p.Key}={Uri.EscapeDataString(p.Value)}"));

					var isValidSign = HMACSHA512Hex(requestContent);

					var request = new ProcessPaymentTransaction()
                    {
                        TransactionHash = model.Tx_hash,
                        Address = model.Address,
                        Amount = model.Amount,
                        Currency = Enum.Parse<Currency>(model.Currency), // TODO
                        Status = model.Confirmations >= 12 ? PaymentTransactionStatus.ConfirmedByGate : PaymentTransactionStatus.Pending,
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

		private string HMACSHA512Hex(string input)
		{
			var key = Encoding.UTF8.GetBytes(config.ApiSecret);
			using (var hm = new HMACSHA512(key))
			{
				var signed = hm.ComputeHash(Encoding.UTF8.GetBytes(input));
				return BitConverter.ToString(signed).Replace("-", string.Empty);
			}
		}
	}
}
