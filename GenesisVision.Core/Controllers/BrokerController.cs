using GenesisVision.Core.Models;
using GenesisVision.Core.Services.Interfaces;
using GenesisVision.Core.Services.Validators.Interfaces;
using GenesisVision.Core.ViewModels.Broker;
using GenesisVision.Core.ViewModels.Common;
using GenesisVision.Core.ViewModels.Trades;
using GenesisVision.DataModel.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Linq;

namespace GenesisVision.Core.Controllers
{
    [Authorize]
    [Route("api")]
    [ApiVersion("1.0")]
    public class BrokerController : BaseController
    {
        private readonly IManagerService managerService;
        private readonly ITrustManagementService trustManagementService;
        private readonly ITradesService tradesService;
        private readonly IBrokerValidator brokerValidator;
        private readonly ILogger<BrokerController> logger;

        public BrokerController(IManagerService managerService, ITrustManagementService trustManagementService,
            IBrokerValidator brokerValidator, UserManager<ApplicationUser> userManager, ITradesService tradesService, ILogger<BrokerController> logger)
            : base(userManager)
        {
            this.managerService = managerService;
            this.trustManagementService = trustManagementService;
            this.tradesService = tradesService;
            this.brokerValidator = brokerValidator;
            this.logger = logger;
        }

        /// <summary>
        /// Get broker initial data
        /// </summary>
        [HttpGet]
        [Route("broker/initData")]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(BrokerInitData))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Type = typeof(ErrorViewModel))]
        public IActionResult GetBrokerInitData(Guid brokerTradeServerId)
        {
            var errors = brokerValidator.ValidateGetBrokerInitData(CurrentUser, brokerTradeServerId);
            if (errors.Any())
                return BadRequest(ErrorResult.GetResult(errors, ErrorCodes.ValidationError));

            var requests = managerService.GetCreateAccountRequests(brokerTradeServerId);
            if (!requests.IsSuccess)
                return BadRequest(ErrorResult.GetResult(requests.Errors));

            var investments = trustManagementService.GetBrokerInvestmentsInitData(brokerTradeServerId);
            if (!investments.IsSuccess)
                return BadRequest(ErrorResult.GetResult(investments.Errors));

            var data = new BrokerInitData
                       {
                           NewManagerRequest = requests.Data,
                           Investments = investments.Data
                       };
            return Ok(data);
        }

        /// <summary>
        /// Create manager
        /// </summary>
        [HttpPost]
        [Route("broker/account/create")]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(Guid))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Type = typeof(ErrorViewModel))]
        public IActionResult CreateManagerAccount([FromBody]NewManager request)
        {
            var errors = brokerValidator.ValidateCreateManagerAccount(CurrentUser, request);
            if (errors.Any())
                return BadRequest(ErrorResult.GetResult(errors, ErrorCodes.ValidationError));

            var result = trustManagementService.CreateInvestmentProgram(request);
            if (!result.IsSuccess)
                return BadRequest(ErrorResult.GetResult(result));

            return Ok(result.Data);
        }

        /// <summary>
        /// Get data for closing investment period
        /// </summary>
        [HttpGet]
        [Route("broker/period/сlosingData")]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(ClosePeriodData))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Type = typeof(ErrorViewModel))]
        public IActionResult GetClosingPeriodData(Guid investmentProgramId)
        {
            var errors = brokerValidator.ValidateGetClosingPeriodData(CurrentUser, investmentProgramId);
            if (errors.Any())
                return BadRequest(ErrorResult.GetResult(errors, ErrorCodes.ValidationError));

            var data = trustManagementService.GetClosingPeriodData(investmentProgramId);
            if (!data.IsSuccess)
                return BadRequest(ErrorResult.GetResult(data.Errors));

            return Ok(data.Data);
        }

        /// <summary>
        /// Close investment period
        /// </summary>
        [HttpPost]
        [Route("broker/period/close")]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(void))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Type = typeof(ErrorViewModel))]
        public IActionResult ClosePeriod(Guid investmentProgramId)
        {
            var errors = brokerValidator.ValidateClosePeriod(CurrentUser, investmentProgramId);
            if (errors.Any())
                return BadRequest(ErrorResult.GetResult(errors, ErrorCodes.ValidationError));

            var result = trustManagementService.ClosePeriod(investmentProgramId);
            if (!result.IsSuccess)
                return BadRequest(ErrorResult.GetResult(result.Errors));

            return Ok();
        }

        /// <summary>
        /// Set investment period start balance
        /// </summary>
        [HttpPost]
        [Route("broker/period/setStartBalance")]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(void))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Type = typeof(ErrorViewModel))]
        public IActionResult SetPeriodStartBalance(Guid periodId, decimal balance)
        {
            var errors = brokerValidator.ValidateSetPeriodStartBalance(CurrentUser, periodId, balance);
            if (errors.Any())
                return BadRequest(ErrorResult.GetResult(errors, ErrorCodes.ValidationError));

            var result = trustManagementService.SetPeriodStartBalance(periodId, balance);
            if (!result.IsSuccess)
                return BadRequest(ErrorResult.GetResult(result.Errors));

            return Ok();
        }

        /// <summary>
        /// Get all enabled trade servers
        /// </summary>
        [HttpPost]
        [AllowAnonymous]
        [Route("manager/brokers")]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(BrokersViewModel))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Type = typeof(ErrorViewModel))]
        public IActionResult GetBrokers([FromBody]BrokersFilter filter)
        {
            var data = trustManagementService.GetBrokerTradeServers(filter ?? new BrokersFilter());
            if (!data.IsSuccess)
                return BadRequest(ErrorResult.GetResult(data.Errors));

            return Ok(new BrokersViewModel
                      {
                          Brokers = data.Data.Item1,
                          Total = data.Data.Item2
                      });
        }

        /// <summary>
        /// Accrue investors' profits
        /// </summary>
        [HttpPost]
        [Route("broker/period/accrueProfits")]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(Guid))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Type = typeof(ErrorViewModel))]
        public IActionResult AccrueProfits([FromBody]InvestmentProgramAccrual accrual)
        {
            var errors = brokerValidator.ValidateAccrueProfits(CurrentUser, accrual);
            if (errors.Any())
                return BadRequest(ErrorResult.GetResult(errors, ErrorCodes.ValidationError));

            var result = trustManagementService.AccrueProfits(accrual);
            if (!result.IsSuccess)
                return BadRequest(ErrorResult.GetResult(result));

            return Ok(result);
        }

        /// <summary>
        /// Process investment requests
        /// </summary>
        [HttpPost]
        [Route("broker/period/processInvestmentRequests")]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(Guid))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Type = typeof(ErrorViewModel))]
        public IActionResult ProcessInvestmentRequests(Guid investmentProgramId)
        {
            var errors = brokerValidator.ValidateProcessInvestmentRequests(CurrentUser, investmentProgramId);
            if (errors.Any())
                return BadRequest(ErrorResult.GetResult(errors, ErrorCodes.ValidationError));

            var result = trustManagementService.ProcessInvestmentRequests(investmentProgramId);
            if (!result.IsSuccess)
                return BadRequest(ErrorResult.GetResult(result));

            return Ok(result.Data);
        }

        /// <summary>
        /// New trade event
        /// </summary>
        [HttpPost]
        [Route("broker/trades/new")]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(void))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Type = typeof(ErrorViewModel))]
        public IActionResult NewTrade([FromBody]NewTradeEvent tradeEvent)
        {
            if (!ModelState.IsValid)
                return BadRequest(ErrorResult.GetResult(ModelState));

            var errors = brokerValidator.ValidateNewTrade(CurrentUser, tradeEvent);
            if (errors.Any())
                return BadRequest(ErrorResult.GetResult(errors, ErrorCodes.ValidationError));

            tradesService.SaveNewTrade(tradeEvent);

            return Ok();
        }

        /// <summary>
        /// New open trades event
        /// </summary>
        [HttpPost]
        [Route("broker/trades/openTrades/new")]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(void))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Type = typeof(ErrorViewModel))]
        public IActionResult NewOpenTrades([FromBody]NewOpenTradesEvent trades)
        {
            if (!ModelState.IsValid)
                return BadRequest(ErrorResult.GetResult(ModelState));

            var errors = brokerValidator.ValidateNewOpenTrades(CurrentUser, trades);
            if (errors.Any())
                return BadRequest(ErrorResult.GetResult(errors, ErrorCodes.ValidationError));

            tradesService.SaveNewOpenTrade(trades);

            return Ok();
        }
    }
}
