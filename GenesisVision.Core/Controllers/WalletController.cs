using GenesisVision.Core.Models;
using GenesisVision.Core.Services.Interfaces;
using GenesisVision.Core.ViewModels.Common;
using GenesisVision.Core.ViewModels.Wallet;
using GenesisVision.DataModel.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace GenesisVision.Core.Controllers
{
    [Authorize]
    [Route("api")]
    [ApiVersion("1.0")]
    public class WalletController : BaseController
    {
        private readonly IWalletService walletService;
        private readonly ILogger<WalletController> logger;

        public WalletController(UserManager<ApplicationUser> userManager, IWalletService walletService, ILogger<WalletController> logger)
            : base(userManager)
        {
            this.walletService = walletService;
            this.logger = logger;
        }

        /// <summary>
        /// Get user wallet transactions
        /// </summary>
        [HttpPost]
        [Route("investor/wallet/transactions")]
        [Route("manager/wallet/transactions")]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(WalletTransactionsViewModel))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Type = typeof(ErrorViewModel))]
        public IActionResult GetWalletTransactions([FromBody]TransactionsFilter filter)
        {
            var data = walletService.GetTransactionHistory(CurrentUser.Id, filter ?? new TransactionsFilter());
            if (!data.IsSuccess)
                return BadRequest(ErrorResult.GetResult(data));

            return Ok(new WalletTransactionsViewModel
                      {
                          Transactions = data.Data.Item1,
                          Total = data.Data.Item2
                      });
        }

        /// <summary>
        /// Get eth address for GVT depositing
        /// </summary>
        [HttpGet]
        [Route("investor/wallet/address")]
        [Route("manager/wallet/address")]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(WalletAddressViewModel))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Type = typeof(ErrorViewModel))]
        public IActionResult GetWalletAddress()
        {
            var address = walletService.GetUserWallet(CurrentUser.Id);
            if (!address.IsSuccess)
                return BadRequest(ErrorResult.GetResult(address));

            return Ok(new WalletAddressViewModel
                      {
                          Address = address.Data
                      });
        }
    }
}
