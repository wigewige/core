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
    [ApiVersion("1.0")]
    [Route("api/manager")]
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
        [Route("newManagerAccountRequest")]
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
        [Route("createManagerAccount")]
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
        [Route("updateManagerAccount")]
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
        [Route("createInvestmentProgram")]
        public IActionResult CreateInvestmentProgram([FromBody]CreateInvestment investment)
        {
            var errors = managerValidator.ValidateCreateInvestmentProgram(CurrentUser, investment);
            if (errors.Any())
                return BadRequest(OperationResult.Failed(errors));

            var result = trustManagementService.CreateInvestmentProgram(investment);
            return Ok(result);
        }

        /// <summary>
        /// Get manager details
        /// </summary>
        [AllowAnonymous]
        [Route("details")]
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
        [AllowAnonymous]
        [Route("userManagersAccounts")]
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
        [AllowAnonymous]
        [Route("getManagers")]
        public IActionResult GetManagers([FromBody]ManagersFilter filter)
        {
            var res = managerService.GetManagersDetails(filter);
            return Ok(res);
        }
    }
}
