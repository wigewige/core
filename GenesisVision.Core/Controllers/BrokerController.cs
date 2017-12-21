using GenesisVision.Core.Services.Interfaces;
using GenesisVision.Core.ViewModels.Broker;
using GenesisVision.Core.ViewModels.Manager;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using GenesisVision.Core.Models;

namespace GenesisVision.Core.Controllers
{
    //[Authorize]
    public class BrokerController : Controller
    {
        private readonly IManagerService managerService;

        public BrokerController(IManagerService managerService)
        {
            this.managerService = managerService;
        }

        public IActionResult GetBrokerInitData(Guid brokerTradeServerId)
        {
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
