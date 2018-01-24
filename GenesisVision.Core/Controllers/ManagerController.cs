﻿using GenesisVision.Core.Models;
using GenesisVision.Core.Services.Interfaces;
using GenesisVision.Core.Services.Validators.Interfaces;
using GenesisVision.Core.ViewModels.Investment;
using GenesisVision.Core.ViewModels.Manager;
using GenesisVision.Core.ViewModels.Other;
using GenesisVision.DataModel.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(Guid))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Type = typeof(ErrorViewModel))]
        public IActionResult NewInvestmentRequest([FromBody]NewInvestmentRequest request)
        {
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
        [HttpGet]
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
        /// Get investment program with statistic by id
        /// </summary>
        [HttpGet]
        [AllowAnonymous]
        [Route("manager/investment")]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(InvestmentProgramViewModel))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Type = typeof(ErrorViewModel))]
        public IActionResult GetInvestmentProgram(Guid investmentProgramId)
        {
            var investment = trustManagementService.GetInvestment(investmentProgramId);
            if (!investment.IsSuccess)
                return BadRequest(ErrorResult.GetResult(investment));

            var statistic = statisticService.GetInvestmentProgramStatistic(investmentProgramId);
            if (!statistic.IsSuccess)
                return BadRequest(ErrorResult.GetResult(statistic));

            return Ok(new InvestmentProgramViewModel
                      {
                          InvestmentProgram = investment.Data,
                          Statistic = statistic.Data
                      });
        }
    }
}
