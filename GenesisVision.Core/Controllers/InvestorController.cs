﻿using GenesisVision.Core.Models;
using GenesisVision.Core.Services.Interfaces;
using GenesisVision.Core.Services.Validators.Interfaces;
using GenesisVision.Core.ViewModels.Account;
using GenesisVision.Core.ViewModels.Common;
using GenesisVision.Core.ViewModels.Investment;
using GenesisVision.DataModel.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Linq;
using GenesisVision.Core.ViewModels.Trades;
using GenesisVision.Core.ViewModels.Wallet;

namespace GenesisVision.Core.Controllers
{
    [Authorize]
    [Route("api")]
    [ApiVersion("1.0")]
    public class InvestorController : BaseController
    {
        private readonly ITrustManagementService trustManagementService;
        private readonly IInvestorValidator investorValidator;
        private readonly IUserService userService;
        private readonly IStatisticService statisticService;
        private readonly ITradesService tradesService;
        private readonly IWalletService walletService;
        private readonly ILogger<InvestorController> logger;

        public InvestorController(ITrustManagementService trustManagementService, IUserService userService, IInvestorValidator investorValidator, UserManager<ApplicationUser> userManager, IStatisticService statisticService, ITradesService tradesService, IWalletService walletService, ILogger<InvestorController> logger)
            : base(userManager)
        {
            this.trustManagementService = trustManagementService;
            this.investorValidator = investorValidator;
            this.userService = userService;
            this.statisticService = statisticService;
            this.tradesService = tradesService;
            this.walletService = walletService;
            this.logger = logger;
        }

        /// <summary>
        /// Invest in manager
        /// </summary>
        [HttpPost]
        [Route("investor/investmentPrograms/invest")]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(WalletsViewModel))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Type = typeof(ErrorViewModel))]
        public IActionResult Invest([FromBody]Invest model)
        {
            model.UserId = CurrentUser.Id;

            var errors = investorValidator.ValidateInvest(CurrentUser, model);
            if (errors.Any())
                return BadRequest(ErrorResult.GetResult(errors, ErrorCodes.ValidationError));

            var res = trustManagementService.Invest(model);
            if (!res.IsSuccess)
                return BadRequest(ErrorResult.GetResult(res.Errors));

            var wallets = walletService.GetUserWallets(CurrentUser.Id);
            return Ok(wallets.Data);
        }

        /// <summary>
        /// Withdraw from investment program
        /// </summary>
        [HttpPost]
        [Route("investor/investmentPrograms/withdraw")]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(void))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Type = typeof(ErrorViewModel))]
        public IActionResult RequestForWithdraw([FromBody]Invest model)
        {
            model.UserId = CurrentUser.Id;

            var errors = investorValidator.ValidateWithdraw(CurrentUser, model);
            if (errors.Any())
                return BadRequest(ErrorResult.GetResult(errors, ErrorCodes.ValidationError));

            var res = trustManagementService.RequestForWithdraw(model);
            if (!res.IsSuccess)
                return BadRequest(ErrorResult.GetResult(res.Errors));

            return Ok();
        }

        /// <summary>
        /// Cancel investment request
        /// </summary>
        [HttpPost]
        [Route("investor/investmentPrograms/cancelInvestmentRequest")]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(void))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Type = typeof(ErrorViewModel))]
        public IActionResult CancelInvestmentRequest(Guid requestId)
        {
            var errors = investorValidator.ValidateCancelInvestmentRequest(CurrentUser, requestId);
            if (errors.Any())
                return BadRequest(ErrorResult.GetResult(errors, ErrorCodes.ValidationError));

            var res = trustManagementService.CancelInvestmentRequest(requestId);
            if (!res.IsSuccess)
                return BadRequest(ErrorResult.GetResult(res.Errors));

            return Ok();
        }

        /// <summary>
        /// Get public investment program's list
        /// </summary>
        [HttpPost]
        [AllowAnonymous]
        [Route("investor/investmentPrograms")]
        [Route("manager/investmentPrograms")]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(InvestmentProgramsViewModel))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Type = typeof(ErrorViewModel))]
        public IActionResult GetInvestmentPrograms([FromHeader(Name = "Authorization")] string token, [FromBody]InvestmentProgramsFilter filter)
        {
            var data = trustManagementService.GetInvestmentPrograms(filter ?? new InvestmentProgramsFilter(), CurrentUser?.Id, CurrentUser?.Type);
            if (!data.IsSuccess)
                return BadRequest(ErrorResult.GetResult(data));

            return Ok(new InvestmentProgramsViewModel
                      {
                          InvestmentPrograms = data.Data.Item1,
                          Total = data.Data.Item2
                      });
        }

        /// <summary>
        /// Get investment program details by id
        /// </summary>
        [HttpGet]
        [AllowAnonymous]
        [Route("investor/investmentProgram")]
        [Route("manager/investmentProgram")]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(InvestmentProgramViewModel))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Type = typeof(ErrorViewModel))]
        public IActionResult GetInvestmentProgram([FromHeader(Name = "Authorization")] string token, Guid investmentProgramId)
        {
            var investment = trustManagementService.GetInvestmentProgram(investmentProgramId, CurrentUser?.Id, CurrentUser?.Type);
            if (!investment.IsSuccess)
                return BadRequest(ErrorResult.GetResult(investment));

            var statistic = statisticService.GetInvestmentProgramStatistic(investmentProgramId);
            if (!statistic.IsSuccess)
                return BadRequest(ErrorResult.GetResult(statistic));

            return Ok(new InvestmentProgramViewModel
                      {
                          InvestmentProgram = investment.Data
                      });
        }
        
        /// <summary>
        /// Get investment program's requests
        /// </summary>
        [HttpPost]
        [Route("investor/investmentProgram/requests")]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(InvestmentProgramRequests))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Type = typeof(ErrorViewModel))]
        public IActionResult GetInvestmentProgramRequests([FromBody]InvestmentProgramRequestsFilter filter)
        {
            if (!ModelState.IsValid)
                return BadRequest(ErrorResult.GetResult(ModelState));

            var errors = investorValidator.ValidateInvestor(CurrentUser);
            if (errors.Any())
                return BadRequest(ErrorResult.GetResult(errors, ErrorCodes.ValidationError));

            var requests = trustManagementService.GetInvestmentProgramRequests(filter, CurrentUser.Id);
            if (!requests.IsSuccess)
                return BadRequest(ErrorResult.GetResult(requests));

            return Ok(new InvestmentProgramRequests
                      {
                          Requests = requests.Data.Item1,
                          Total = requests.Data.Item2
                      });
        }
        
        /// <summary>
        /// Get investor dashboard
        /// </summary>
        [HttpGet]
        [Route("investor/dashboard")]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(InvestorDashboard))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Type = typeof(ErrorViewModel))]
        public IActionResult InvestorDashboard()
        {
            var data = trustManagementService.GetInvestorDashboard(CurrentUser.Id, CurrentUser?.Id, CurrentUser?.Type);
            if (!data.IsSuccess)
                return BadRequest(ErrorResult.GetResult(data));

            return Ok(data.Data);
        }

        /// <summary>
        /// Get manager trade history
        /// </summary>
        [HttpPost]
        [Route("investor/investmentProgram/trades")]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(TradesViewModel))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Type = typeof(ErrorViewModel))]
        public IActionResult GetManagerTrades([FromBody]TradesFilter filter)
        {
            var trades = tradesService.GetManagerTrades(filter);
            if (!trades.IsSuccess)
                return BadRequest(ErrorResult.GetResult(trades));

            return Ok(new TradesViewModel
                      {
                          Trades = trades.Data.Item1,
                          Total = trades.Data.Item2
                      });
        }

        /// <summary>
        /// Get manager open trades
        /// </summary>
        [HttpPost]
        [Route("investor/investmentProgram/openTrades")]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(OpenTradesViewModel))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Type = typeof(ErrorViewModel))]
        public IActionResult GetManagerOpenTrades([FromBody]TradesFilter filter)
        {
            var trades = tradesService.GetManagerOpenTrades(filter);
            if (!trades.IsSuccess)
                return BadRequest(ErrorResult.GetResult(trades));

            return Ok(new OpenTradesViewModel
                      {
                          Trades = trades.Data.Item1,
                          Total = trades.Data.Item2
                      });
        }
    }
}
