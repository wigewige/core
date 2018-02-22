using GenesisVision.Core.Models;
using GenesisVision.Core.Services.Interfaces;
using GenesisVision.Core.Services.Validators.Interfaces;
using GenesisVision.Core.ViewModels.Common;
using GenesisVision.Core.ViewModels.Investment;
using GenesisVision.Core.ViewModels.Manager;
using GenesisVision.DataModel.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.SwaggerGen;
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
        private readonly ILogger<ManagerController> logger;

        public ManagerController(ITrustManagementService trustManagementService, IStatisticService statisticService, IManagerService managerService,
            IManagerValidator managerValidator, UserManager<ApplicationUser> userManager, ILogger<ManagerController> logger)
            : base(userManager)
        {
            this.trustManagementService = trustManagementService;
            this.statisticService = statisticService;
            this.managerService = managerService;
            this.managerValidator = managerValidator;
            this.logger = logger;
        }

        /// <summary>
        /// Create new investment request
        /// </summary>
        [HttpPost]
        [Route("manager/account/newInvestmentRequest")]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(Guid))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Type = typeof(ErrorViewModel))]
        public IActionResult NewInvestmentRequest([FromBody]NewInvestmentRequest request)
        {
            request.UserId = CurrentUser.Id;

            if (!ModelState.IsValid)
                return BadRequest(ErrorResult.GetResult(ModelState));

            var errors = managerValidator.ValidateNewInvestmentRequest(CurrentUser, request);
            if (errors.Any())
                return BadRequest(ErrorResult.GetResult(errors, ErrorCodes.ValidationError));

            var result = managerService.CreateNewInvestmentRequest(request);
            if (!result.IsSuccess)
                return BadRequest(ErrorResult.GetResult(result));
            
            return Ok(result.Data);
        }
        
        /// <summary>
        /// Close existing investment program
        /// </summary>
        [HttpPost]
        [Route("manager/investment/close")]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(void))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Type = typeof(ErrorViewModel))]
        public IActionResult CloseInvestmentProgram(Guid investmentProgramId)
        {
            var errors = managerValidator.ValidateCloseInvestmentProgram(CurrentUser, investmentProgramId);
            if (errors.Any())
                return BadRequest(ErrorResult.GetResult(errors, ErrorCodes.ValidationError));

            var result = trustManagementService.CloseInvestmentProgram(investmentProgramId);
            if (!result.IsSuccess)
                return BadRequest(ErrorResult.GetResult(result));

            return Ok();
        }
        
        /// <summary>
        /// Manager deposit in his own investment program
        /// </summary>
        [HttpPost]
        [Route("manager/investment/invest")]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(void))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Type = typeof(ErrorViewModel))]
        public IActionResult Invest([FromBody]Invest model)
        {
            model.UserId = CurrentUser.Id;

            var errors = managerValidator.ValidateInvest(CurrentUser, model);
            if (errors.Any())
                return BadRequest(ErrorResult.GetResult(errors, ErrorCodes.ValidationError));

            var res = trustManagementService.Invest(model);
            if (!res.IsSuccess)
                return BadRequest(ErrorResult.GetResult(res.Errors));

            return Ok();
        }

        /// <summary>
        /// Manager withdrawal from his own investment program
        /// </summary>
        [HttpPost]
        [Route("manager/investment/withdraw")]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(void))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Type = typeof(ErrorViewModel))]
        public IActionResult RequestForWithdraw([FromBody]Invest model)
        {
            model.UserId = CurrentUser.Id;

            var errors = managerValidator.ValidateWithdraw(CurrentUser, model);
            if (errors.Any())
                return BadRequest(ErrorResult.GetResult(errors, ErrorCodes.ValidationError));

            var res = trustManagementService.RequestForWithdraw(model);
            if (!res.IsSuccess)
                return BadRequest(ErrorResult.GetResult(res.Errors));

            return Ok();
        }

        /// <summary>
        /// Cancel investment request
        /// </summary>
        [HttpPost]
        [Route("manager/investment/cancelInvestmentRequest")]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(void))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Type = typeof(ErrorViewModel))]
        public IActionResult CancelInvestmentRequest(Guid requestId)
        {
            var errors = managerValidator.ValidateCancelInvestmentRequest(CurrentUser, requestId);
            if (errors.Any())
                return BadRequest(ErrorResult.GetResult(errors, ErrorCodes.ValidationError));

            var res = trustManagementService.CancelInvestmentRequest(requestId);
            if (!res.IsSuccess)
                return BadRequest(ErrorResult.GetResult(res.Errors));

            return Ok();
        }
    }
}
