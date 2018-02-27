using GenesisVision.Core.Models;
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
        private readonly ILogger<InvestorController> logger;

        public InvestorController(ITrustManagementService trustManagementService, IUserService userService, IInvestorValidator investorValidator, UserManager<ApplicationUser> userManager, IStatisticService statisticService, ITradesService tradesService, ILogger<InvestorController> logger)
            : base(userManager)
        {
            this.trustManagementService = trustManagementService;
            this.investorValidator = investorValidator;
            this.userService = userService;
            this.statisticService = statisticService;
            this.tradesService = tradesService;
            this.logger = logger;
        }

        /// <summary>
        /// Invest in manager
        /// </summary>
        [HttpPost]
        [Route("investor/investmentPrograms/invest")]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(ProfileShortViewModel))]
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

            var user = userService.GetUserProfileShort(CurrentUser.Id);
            return Ok(user.Data);
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
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(InvestmentProgramsViewModel))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Type = typeof(ErrorViewModel))]
        public IActionResult GetInvestmentPrograms([FromBody]InvestmentProgramsFilter filter)
        {
            var data = trustManagementService.GetInvestmentPrograms(filter ?? new InvestmentProgramsFilter());
            if (!data.IsSuccess)
                return BadRequest(ErrorResult.GetResult(data));

            return Ok(new InvestmentProgramsViewModel
                      {
                          Investments = data.Data.Item1,
                          Total = data.Data.Item2
                      });
        }

        /// <summary>
        /// Get investment program details by id
        /// </summary>
        [HttpGet]
        [AllowAnonymous]
        [Route("investor/investmentProgram")]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(InvestmentProgramViewModel))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Type = typeof(ErrorViewModel))]
        public IActionResult GetInvestmentProgram(Guid investmentProgramId)
        {
            var investment = trustManagementService.GetInvestmentProgram(investmentProgramId);
            if (!investment.IsSuccess)
                return BadRequest(ErrorResult.GetResult(investment));

            var statistic = statisticService.GetInvestmentProgramStatistic(investmentProgramId);
            if (!statistic.IsSuccess)
                return BadRequest(ErrorResult.GetResult(statistic));

            return Ok(new InvestmentProgramViewModel
                      {
                          InvestmentProgram = investment.Data,
                          Statistic = statistic.Data
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
            var data = trustManagementService.GetInvestorDashboard(CurrentUser.Id);
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
