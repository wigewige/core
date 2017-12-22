using GenesisVision.Core.Models;
using GenesisVision.Core.Services.Interfaces;
using GenesisVision.Core.Services.Validators;
using GenesisVision.Core.ViewModels.Broker;
using GenesisVision.Core.ViewModels.Manager;
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
        private readonly IBrokerValidator brokerValidator;

        public BrokerController(IManagerService managerService, IBrokerValidator brokerValidator)
        {
            this.managerService = managerService;
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

                return new BrokerInitData
                       {
                           NewManagerRequest = requests.IsSuccess
                               ? requests.Data
                               : new List<ManagerRequest>()
                       };
            });
            return Ok(result);
        }
    }
}
