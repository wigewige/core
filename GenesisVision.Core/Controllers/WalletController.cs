using GenesisVision.Core.Models;
using GenesisVision.Core.Services.Interfaces;
using GenesisVision.Core.ViewModels.Other;
using GenesisVision.Core.ViewModels.Wallet;
using GenesisVision.DataModel.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace GenesisVision.Core.Controllers
{
    [Authorize]
    [Route("api")]
    [ApiVersion("1.0")]
    public class WalletController : BaseController
    {
        private readonly IWalletService walletService;

        public WalletController(UserManager<ApplicationUser> userManager, IWalletService walletService)
            : base(userManager)
        {
            this.walletService = walletService;
        }

        /// <summary>
        /// Get user wallet transactions
        /// </summary>
        [HttpPost]
        [AllowAnonymous]
        [Route("investor/wallet/transactions")]
        [Route("manager/wallet/transactions")]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(WalletTransactionsViewModel))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Type = typeof(ErrorViewModel))]
        public IActionResult GetWalletTransactions([FromBody]TransactionsFilter filter)
        {
            var data = walletService.GetTransactionHistory(CurrentUserId.Value, filter);
            if (!data.IsSuccess)
                return BadRequest(ErrorResult.GetResult(data));

            return Ok(new WalletTransactionsViewModel
                      {
                          Transactions = data.Data.Item1,
                          Total = data.Data.Item2
                      });
        }
    }
}
