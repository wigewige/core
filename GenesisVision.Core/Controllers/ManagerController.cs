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
        /// Create request (from cabinet, broker)
        /// </summary>
        [HttpPost]
        [Route("manager/account/newRequest")]
        //[Route("broker/account/newRequest")]
        public IActionResult NewManagerAccountRequest([FromBody]NewManagerRequest request)
        {
            var errors = managerValidator.ValidateNewManagerAccountRequest(CurrentUser, request);
            if (errors.Any())
                return BadRequest(OperationResult.Failed(errors));

            var result = managerService.CreateManagerAccountRequest(request);
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
        /// Update manager account
        /// </summary>
        [HttpPost]
        [Route("manager/account/update")]
        public IActionResult UpdateManagerAccount([FromBody]UpdateManagerAccount account)
        {
            var errors = managerValidator.ValidateUpdateManagerAccount(CurrentUser, account);
            if (errors.Any())
                return BadRequest(OperationResult.Failed(errors));

            var details = managerService.UpdateManagerAccount(account);
            return Ok(details);
        }

        /// <summary>
        /// Create investment program
        /// </summary>
        [HttpPost]
        [Route("manager/investment/create")]
        public IActionResult CreateInvestmentProgram([FromBody]CreateInvestment investment)
        {
            var errors = managerValidator.ValidateCreateInvestmentProgram(CurrentUser, investment);
            if (errors.Any())
                return BadRequest(OperationResult.Failed(errors));

            var result = trustManagementService.CreateInvestmentProgram(investment);
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

        /// <summary>
        /// Get manager details
        /// </summary>
        [HttpGet]
        [AllowAnonymous]
        [Route("manager/account")]
        public IActionResult Details(Guid managerId)
        {
            var errors = managerValidator.ValidateGetManagerDetails(CurrentUser, managerId);
            if (errors.Any())
                return BadRequest(OperationResult.Failed(errors));

            var details = managerService.GetManagerDetails(managerId);
            return Ok(details);
        }

        /// <summary>
        /// Get all managers account by user
        /// </summary>
        [HttpGet]
        [AllowAnonymous]
        [Route("manager/account/user")]
        public IActionResult GetUserManagersAccounts(Guid? userId)
        {
            if (!userId.HasValue && CurrentUserId.HasValue)
                userId = CurrentUserId;

            if (!userId.HasValue)
                return BadRequest();

            var managers = managerService.GetUserManagersAccounts(userId.Value);
            return Ok(managers);
        }

        /// <summary>
        /// Get managers by filter
        /// </summary>
        [HttpPost]
        [AllowAnonymous]
        [Route("manager/account/search")]
        public IActionResult GetManagers([FromBody]ManagersFilter filter)
        {
            var res = managerService.GetManagersDetails(filter);
            return Ok(res);
        }
    }
}
