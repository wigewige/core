﻿using GenesisVision.Core.Services.Interfaces;
using GenesisVision.Core.ViewModels.Investment;
using GenesisVision.Core.ViewModels.Manager;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GenesisVision.Core.Controllers
{
    [Authorize]
    [Route("api/manager")]
    public class ManagerController : Controller
    {
        private readonly ITrustManagementService trustManagementService;
        private readonly IManagerService managerService;

        public ManagerController(ITrustManagementService trustManagementService, IManagerService managerService)
        {
            this.trustManagementService = trustManagementService;
            this.managerService = managerService;
        }
        
        /// <summary>
        /// Create request
        /// </summary>
        public IActionResult NewManagerAccountRequest([FromBody]NewManagerRequest request)
        {
            var res = managerService.CreateManagerAccountRequest(request);
            return Ok(res);
        }
        
        /// <summary>
        /// Create manager
        /// </summary>
        public IActionResult CreateManagerAccount([FromBody]NewManager request)
        {
            var res = managerService.CreateManagerAccount(request);
            return Ok(res);
        }

        public IActionResult CreateInvestmentProgram([FromBody]CreateInvestment investment)
        {
            var res = trustManagementService.CreateInvestmentProgram(investment);
            return Ok(res);
        }
    }
}
