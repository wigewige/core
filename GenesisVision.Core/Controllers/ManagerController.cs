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
        private readonly IStatisticService statisticService;
        private readonly IManagerService managerService;
        private readonly IManagerValidator managerValidator;

        public ManagerController(ITrustManagementService trustManagementService, IStatisticService statisticService, IManagerService managerService,
            IManagerValidator managerValidator, UserManager<ApplicationUser> userManager)
            : base(userManager)
        {
            this.trustManagementService = trustManagementService;
            this.statisticService = statisticService;
            this.managerService = managerService;
            this.managerValidator = managerValidator;
        }

        /// <summary>
        /// Create new investment request
        /// </summary>
        [HttpPost]
        [Route("manager/account/newInvestmentRequest")]
        public IActionResult NewInvestmentRequest([FromBody]NewInvestmentRequest request)
        {
            var errors = managerValidator.ValidateNewInvestmentRequest(CurrentUser, request);
            if (errors.Any())
                return BadRequest(OperationResult.Failed(errors));

            var result = managerService.CreateNewInvestmentRequest(request);
            if (!result.IsSuccess)
                return BadRequest(result.Errors);
            
            return Ok(result.Data);
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
            if (!result.IsSuccess)
                return BadRequest(result.Errors);

            return Ok(result.Data);
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
            if (!result.IsSuccess)
                return BadRequest(result.Errors);

            return Ok();
        }

        /// <summary>
        /// Get investment program with statistic by id
        /// </summary>
        [HttpGet]
        [AllowAnonymous]
        [Route("manager/investment")]
        public IActionResult GetInvestmentProgram(Guid investmentProgramId)
        {
            var investment = trustManagementService.GetInvestment(investmentProgramId);
            if (!investment.IsSuccess)
                return BadRequest(investment.Errors);

            var statistic = statisticService.GetInvestmentProgramStatistic(investmentProgramId);
            if (!statistic.IsSuccess)
                return BadRequest(statistic.Errors);

            return Ok(new
                      {
                          InvestmentProgram = investment.Data,
                          Statistic = statistic.Data
                      });
        }
    }
}
