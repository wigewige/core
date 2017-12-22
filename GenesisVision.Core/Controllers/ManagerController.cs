using System;
using System.Linq;
using GenesisVision.Core.Models;
using GenesisVision.Core.Services.Interfaces;
using GenesisVision.Core.Services.Validators;
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
        private readonly IManagerValidator managerValidator;

        public ManagerController(ITrustManagementService trustManagementService, IManagerService managerService, IManagerValidator managerValidator)
        {
            this.trustManagementService = trustManagementService;
            this.managerService = managerService;
            this.managerValidator = managerValidator;
        }
        
        /// <summary>
        /// Create request (from cabinet, broker)
        /// </summary>
        public IActionResult NewManagerAccountRequest([FromBody]NewManagerRequest request)
        {
            var errors = managerValidator.ValidateNewManagerAccountRequest(User, request);
            if (errors.Any())
                return BadRequest(OperationResult.Failed(errors));

            var result = managerService.CreateManagerAccountRequest(request);
            return Ok(result);
        }
        
        /// <summary>
        /// Create manager (from broker)
        /// </summary>
        public IActionResult CreateManagerAccount([FromBody]NewManager request)
        {
            var errors = managerValidator.ValidateCreateManagerAccount(User, request);
            if (errors.Any())
                return BadRequest(OperationResult.Failed(errors));

            var result = managerService.CreateManagerAccount(request);
            return Ok(result);
        }

        /// <summary>
        /// Create investment program
        /// </summary>
        public IActionResult CreateInvestmentProgram([FromBody]CreateInvestment investment)
        {
            var errors = managerValidator.ValidateCreateInvestmentProgram(User, investment);
            if (errors.Any())
                return BadRequest(OperationResult.Failed(errors));

            var result = trustManagementService.CreateInvestmentProgram(investment);
            return Ok(result);
        }
    }
}
