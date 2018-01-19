using GenesisVision.Core.Models;
using GenesisVision.Core.Services.Interfaces;
using GenesisVision.Core.Services.Validators.Interfaces;
using GenesisVision.Core.ViewModels.Investment;
using GenesisVision.Core.ViewModels.Manager;
using GenesisVision.DataModel.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;

namespace GenesisVision.Core.Controllers
{
    [Authorize]
    [Route("api")]
    [ApiVersion("1.0")]
    public class ManagerController : BaseController
    {
        private readonly ITrustManagementService trustManagementService;
        private readonly IManagerService managerService;
        private readonly IManagerValidator managerValidator;

        public ManagerController(ITrustManagementService trustManagementService, IManagerService managerService,
            IManagerValidator managerValidator, UserManager<ApplicationUser> userManager)
            : base(userManager)
        {
            this.trustManagementService = trustManagementService;
            this.managerService = managerService;
            this.managerValidator = managerValidator;
        }

        /// <summary>
        /// Create new investment request
        /// </summary>
        [HttpPost]
        [Route("manager/account/newRequest")]
        public IActionResult NewInvestmentRequest([FromBody]NewInvestmentRequest request)
        {
            var errors = managerValidator.ValidateNewInvestmentRequest(CurrentUser, request);
            if (errors.Any())
                return BadRequest(OperationResult.Failed(errors));

            var result = managerService.CreateNewInvestmentRequest(request);
            return Ok(result);
        }

        /// <summary>
        /// Create manager (from broker)
        /// </summary>
        [HttpPost]
        [Route("broker/account/create")]
        public IActionResult CreateManagerAccount([FromBody]NewManager request)
        {
            var errors = managerValidator.ValidateCreateManagerAccount(CurrentUser, request);
            if (errors.Any())
                return BadRequest(OperationResult.Failed(errors));

            var result = managerService.CreateManagerAccount(request);
            return Ok(result);
        }
        
        /// <summary>
        /// Close existing investment program
        /// </summary>
        [HttpGet]
        [Route("manager/investment/close")]
        public IActionResult CloseInvestmentProgram(Guid investmentProgramId)
        {
            var errors = managerValidator.ValidateCloseInvestmentProgram(CurrentUser, investmentProgramId);
            if (errors.Any())
                return BadRequest(OperationResult.Failed(errors));

            var result = trustManagementService.CloseInvestmentProgram(investmentProgramId);
            return Ok(result);
        }
    }
}
