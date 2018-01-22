using GenesisVision.Core.Models;
using GenesisVision.Core.Services.Interfaces;
using GenesisVision.Core.Services.Validators.Interfaces;
using GenesisVision.Core.ViewModels.Broker;
using GenesisVision.Core.ViewModels.Other;
using GenesisVision.DataModel.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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
        private readonly IBrokerValidator brokerValidator;

        public BrokerController(IManagerService managerService, ITrustManagementService trustManagementService,
            IBrokerValidator brokerValidator, UserManager<ApplicationUser> userManager)
            : base(userManager)
        {
            this.managerService = managerService;
            this.trustManagementService = trustManagementService;
            this.brokerValidator = brokerValidator;
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
        [HttpGet]
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
        [HttpGet]
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
            var data = trustManagementService.GetBrokerTradeServers(filter);
            if (!data.IsSuccess)
                return BadRequest(ErrorResult.GetResult(data.Errors));

            return Ok(new BrokersViewModel
                      {
                          Brokers = data.Data.Item1,
                          Total = data.Data.Item2
                      });
        }
    }
}
