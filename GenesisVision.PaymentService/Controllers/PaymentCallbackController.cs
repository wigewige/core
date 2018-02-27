using GenesisVision.DataModel.Enums;
using GenesisVision.PaymentService.Models;
using GenesisVision.PaymentService.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GenesisVision.PaymentService.Controllers
{
    public class PaymentCallbackController : Controller
    {
        private ILogger<PaymentCallbackController> logger;
        private IPaymentTransactionService paymentService;

        public PaymentCallbackController(ILogger<PaymentCallbackController> logger, IPaymentTransactionService paymentsService)
        {
            this.logger = logger;
            this.paymentService = paymentsService;
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
                    var request = new ProcessPaymentTransaction()
                    {
                        TransactionHash = model.Tx_hash,
                        Address = model.Address,
                        Amount = model.Amount,
                        Currency = Currency.ETH, // TODO
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
    }
}
