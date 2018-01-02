using GenesisVision.Core.Models;
using GenesisVision.Core.ViewModels.Broker;
using GenesisVision.Core.ViewModels.Manager;
using GenesisVision.Core.ViewModels.Investment;
using GenesisVision.Core.Services.Validators.Interfaces;
using GenesisVision.Core.Services.Interfaces;
using GenesisVision.Core.Services.Validators;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GenesisVision.Core.Controllers
{
    //[Authorize]
    public class BrokerController : Controller
    {
        private readonly IManagerService managerService;
        private readonly ITrustManagementService trustManagementService;
        private readonly IBrokerValidator brokerValidator;

        public BrokerController(IManagerService managerService, ITrustManagementService trustManagementService, IBrokerValidator brokerValidator)
        {
            this.managerService = managerService;
            this.trustManagementService = trustManagementService;
            this.brokerValidator = brokerValidator;
        }

        /// <summary>
        /// Get broker initial data
        /// </summary>
        public IActionResult GetBrokerInitData(Guid brokerTradeServerId)
        {
            var errors = brokerValidator.ValidateGetBrokerInitData(User, brokerTradeServerId);
            if (errors.Any())
                return BadRequest(OperationResult.Failed(errors));

            var result = InvokeOperations.InvokeOperation(() =>
            {
                var requests = managerService.GetNewRequests(brokerTradeServerId);
                var investments = trustManagementService.GetBrokerInvestmentsInitData(brokerTradeServerId);

                return new BrokerInitData
                       {
                           NewManagerRequest = requests.IsSuccess
                               ? requests.Data
                               : new List<ManagerRequest>(),
                           Investments = investments.IsSuccess
                               ? investments.Data
                               : new List<Investment>()
                       };
            });
            return Ok(result);
        }

        /// <summary>
        /// Get data for closing investment period
        /// </summary>
        public IActionResult GetClosingPeriodData(Guid investmentProgramId)
        {
            var errors = brokerValidator.ValidateGetClosingPeriodData(User, investmentProgramId);
            if (errors.Any())
                return BadRequest(OperationResult.Failed(errors));

            var result = trustManagementService.GetClosingPeriodData(investmentProgramId);

            return Ok(result);
        }

        /// <summary>
        /// Close investment period
        /// </summary>
        public IActionResult ClosePeriod(Guid investmentProgramId)
        {
            var errors = brokerValidator.ValidateClosePeriod(User, investmentProgramId);
            if (errors.Any())
                return BadRequest(OperationResult.Failed(errors));

            var result = trustManagementService.ClosePeriod(investmentProgramId);

            return Ok(result);
        }

        /// <summary>
        /// Set investment period start balance
        /// </summary>
        public IActionResult SetPeriodStartBalance(Guid periodId, decimal balance)
        {
            var errors = brokerValidator.ValidateSetPeriodStartBalance(User, periodId, balance);
            if (errors.Any())
                return BadRequest(OperationResult.Failed(errors));

            var result = trustManagementService.SetPeriodStartBalance(periodId, balance);

            return Ok(result);
        }
    }
}
