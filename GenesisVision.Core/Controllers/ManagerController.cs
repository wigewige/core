using System;
using GenesisVision.Core.Models;
using GenesisVision.Core.Services.Interfaces;
using GenesisVision.Core.ViewModels.Investment;
using GenesisVision.Core.ViewModels.Manager;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GenesisVision.Core.Controllers
{
    //[Authorize]
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
        /// Create request (from cabinet, broker)
        /// </summary>
        public IActionResult NewManagerAccountRequest([FromBody]NewManagerRequest request)
        {
            var result = InvokeOperations.InvokeOperation(() => managerService.CreateManagerAccountRequest(request));
            return Ok(result);
        }
        
        /// <summary>
        /// Create manager (from broker)
        /// </summary>
        public IActionResult CreateManagerAccount([FromBody]NewManager request)
        {
            var result = InvokeOperations.InvokeOperation(() => managerService.CreateManagerAccount(request));
            return Ok(result);
        }

        /// <summary>
        /// Create investment program
        /// </summary>
        public IActionResult CreateInvestmentProgram([FromBody]CreateInvestment investment)
        {
            var result = InvokeOperations.InvokeOperation(() => trustManagementService.CreateInvestmentProgram(investment));
            return Ok(result);
        }
    }
}
