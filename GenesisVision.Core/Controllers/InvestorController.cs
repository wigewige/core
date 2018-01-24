﻿using GenesisVision.Core.Models;
using GenesisVision.Core.Services.Interfaces;
using GenesisVision.Core.Services.Validators.Interfaces;
using GenesisVision.Core.ViewModels.Account;
using GenesisVision.Core.ViewModels.Investment;
using GenesisVision.Core.ViewModels.Other;
using GenesisVision.DataModel.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Linq;

namespace GenesisVision.Core.Controllers
{
    [Authorize]
    [Route("api")]
    [ApiVersion("1.0")]
    public class InvestorController : BaseController
    {
        private readonly ITrustManagementService trustManagementService;
        private readonly IInvestorValidator investorValidator;
        private readonly IUserService userService;

        public InvestorController(ITrustManagementService trustManagementService, IUserService userService, IInvestorValidator investorValidator, UserManager<ApplicationUser> userManager)
            : base(userManager)
        {
            this.trustManagementService = trustManagementService;
            this.investorValidator = investorValidator;
            this.userService = userService;
        }

        /// <summary>
        /// Invest in manager
        /// </summary>
        [HttpPost]
        [Route("investor/investments/invest")]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(ProfileShortViewModel))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Type = typeof(ErrorViewModel))]
        public IActionResult Invest([FromBody]InvestViewModel model)
        {
            var investModel = new Invest
                              {
                                  UserId = CurrentUserId.Value,
                                  Amount = model.Amount,
                                  InvestmentProgramId = model.InvestmentProgramId
                              };

            var errors = investorValidator.ValidateInvest(CurrentUser, investModel);
            if (errors.Any())
                return BadRequest(ErrorResult.GetResult(errors, ErrorCodes.ValidationError));

            var res = trustManagementService.Invest(investModel);
            if (!res.IsSuccess)
                return BadRequest(ErrorResult.GetResult(res.Errors));

            var user = userService.GetUserProfileShort(CurrentUserId.Value);
            return Ok(user.Data);
        }

        /// <summary>
        /// Withdraw from investment program
        /// </summary>
        [HttpPost]
        [Route("investor/investments/withdraw")]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(void))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Type = typeof(ErrorViewModel))]
        public IActionResult RequestForWithdraw([FromBody]InvestViewModel model)
        {
            var investModel = new Invest
                              {
                                  UserId = CurrentUserId.Value,
                                  Amount = model.Amount,
                                  InvestmentProgramId = model.InvestmentProgramId
                              };

            var errors = investorValidator.ValidateWithdraw(CurrentUser, investModel);
            if (errors.Any())
                return BadRequest(ErrorResult.GetResult(errors, ErrorCodes.ValidationError));

            var res = trustManagementService.RequestForWithdraw(investModel);
            if (!res.IsSuccess)
                return BadRequest(ErrorResult.GetResult(res.Errors));

            return Ok();
        }

        /// <summary>
        /// Get investments by filter
        /// </summary>
        [HttpPost]
        [AllowAnonymous]
        [Route("investor/investments")]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(InvestmentProgramsViewModel))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Type = typeof(ErrorViewModel))]
        public IActionResult GetInvestments([FromBody]InvestmentsFilter filter)
        {
            var data = trustManagementService.GetInvestments(filter);
            if (!data.IsSuccess)
                return BadRequest(ErrorResult.GetResult(data));

            return Ok(new InvestmentProgramsViewModel
                      {
                          Investments = data.Data.Item1,
                          Total = data.Data.Item2
                      });
        }


        /// <summary>
        /// Get investor dashboard
        /// </summary>
        [HttpGet]
        [Route("investor/dashboard")]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(InvestorDashboard))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Type = typeof(ErrorViewModel))]
        public IActionResult InvestorDashboard()
        {
            var data = trustManagementService.GetInvestorDashboard(CurrentUserId.Value);
            if (!data.IsSuccess)
                return BadRequest(ErrorResult.GetResult(data));

            return Ok(data.Data);
        }
    }
}
