﻿using GenesisVision.Core.Helpers;
using GenesisVision.Core.Helpers.TokenHelper;
using GenesisVision.Core.Models;
using GenesisVision.Core.Services.Interfaces;
using GenesisVision.Core.ViewModels.Account;
using GenesisVision.Core.ViewModels.Common;
using GenesisVision.DataModel.Enums;
using GenesisVision.DataModel.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GenesisVision.Common.Services.Interfaces;

namespace GenesisVision.Core.Controllers
{
    [Route("api")]
    [ApiVersion("1.0")]
    public class AccountController : BaseController
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IEmailSender emailSender;
        private readonly IUserService userService;
        private readonly IEthService ethService;
        private readonly ILogger<AccountController> logger;

        public AccountController(UserManager<ApplicationUser> userManager, IEmailSender emailSender, IUserService userService, IEthService ethService, ILogger<AccountController> logger)
            : base(userManager)
        {
            this.userManager = userManager;
            this.emailSender = emailSender;
            this.userService = userService;
            this.ethService = ethService;
            this.logger = logger;
        }

        /// <summary>
        /// Authorize
        /// </summary>
        [HttpPost]
        [Route("manager/auth/signIn")]
        [Route("investor/auth/signIn")]
        [Route("broker/auth/signIn")]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(string))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Type = typeof(ErrorViewModel))]
        public async Task<IActionResult> Authorize([FromBody]LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ErrorResult.GetResult(ModelState));

            var user = await userManager.FindByNameAsync(model.Email);
            if (user == null)
                return BadRequest(ErrorResult.GetResult($"Wrong email/password"));

            if (await userManager.CheckPasswordAsync(user, model.Password))
            {
                if (!await userManager.IsEmailConfirmedAsync(user))
                    return BadRequest(ErrorResult.GetResult("Email does not confirmed"));

                if (!user.IsEnabled)
                    return BadRequest(ErrorResult.GetResult("User is disabled"));

                var token = JwtManager.GenerateToken(user);
                return Ok(token.Value);
            }

            return BadRequest(ErrorResult.GetResult("Wrong email/password"));
        }

        /// <summary>
        /// Update auth token
        /// </summary>
        [HttpGet]
        [Authorize]
        [Route("manager/auth/updateToken")]
        [Route("investor/auth/updateToken")]
        [Route("broker/auth/updateToken")]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(string))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Type = typeof(ErrorViewModel))]
        public IActionResult UpdateAuthToken()
        {
            if (CurrentUser.IsEnabled && CurrentUser.EmailConfirmed)
            {
                var token = JwtManager.GenerateToken(CurrentUser);
                return Ok(token.Value);
            }

            return BadRequest(ErrorResult.GetResult(ValidationMessages.AccessDenied));
        }
        
        /// <summary>
        /// Get public profile
        /// </summary>
        [HttpGet]
        [Route("manager/profile/public")]
        [Route("investor/profile/public")]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(ProfilePublicViewModel))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Type = typeof(ErrorViewModel))]
        public IActionResult ProfileShort(Guid userId)
        {
            var user = userService.GetUserPublicProfile(userId);
            if (!user.IsSuccess)
                return BadRequest(ErrorResult.GetResult(user));

            return Ok(user.Data);
        }

        /// <summary>
        /// Get full profile
        /// </summary>
        [HttpGet]
        [Authorize]
        [Route("manager/profile/full")]
        [Route("investor/profile/full")]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(ProfileFullViewModel))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Type = typeof(ErrorViewModel))]
        public IActionResult ProfileFull()
        {
            var user = userService.GetUserProfileFull(CurrentUser.Id);
            if (!user.IsSuccess)
                return BadRequest(ErrorResult.GetResult(user));

            return Ok(user.Data);
        }

        /// <summary>
        /// Update profile
        /// </summary>
        [HttpPost]
        [Authorize]
        [Route("manager/profile/update")]
        [Route("investor/profile/update")]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(void))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Type = typeof(ErrorViewModel))]
        public IActionResult UpdateProfile([FromBody]UpdateProfileViewModel model)
        {
            var res = userService.UpdateUserProfile(CurrentUser.Id, model);
            if (!res.IsSuccess)
                return BadRequest(ErrorResult.GetResult(res));

            return Ok();
        }

        /// <summary>
        /// Register new manager
        /// </summary>
        [HttpPost]
        [Route("manager/auth/signUp")]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(void))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Type = typeof(ErrorViewModel))]
        public async Task<IActionResult> RegisterManager([FromBody]RegisterManagerViewModel model)
        {
            logger.LogInformation($"Register manager: {model?.Email}");

            if (!ModelState.IsValid)
                return BadRequest(ErrorResult.GetResult(ModelState));

            var address = ethService.GenerateAddress();
            var user = new ApplicationUser
                       {
                           UserName = model.Email,
                           Email = model.Email,
                           IsEnabled = true,
                           Type = UserType.Manager,
                           Profile = new Profiles(),
                           Wallets = new List<Wallets> {new Wallets {Currency = Currency.GVT}},
                           BlockchainAddresses = new List<BlockchainAddresses>
                                                 {
                                                     new BlockchainAddresses
                                                     {
                                                         Address = address.PublicAddress,
                                                         Currency = Currency.GVT,
                                                         IsDefault = true
                                                     }
                                                 }
                       };
            var result = await userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
                return BadRequest(ErrorResult.GetResult(result));

            var code = await userManager.GenerateEmailConfirmationTokenAsync(user);

            if (Constants.IsDevelopment)
            {
                await userManager.ConfirmEmailAsync(user, code);
                return Ok();
            }
            else
            {
                var callbackUrl = Url.Action("ConfirmEmail", "Account", new {userId = user.Id, code}, HttpContext.Request.Scheme);
                var text = $"Confirmation url: {callbackUrl}";
                emailSender.SendEmailAsync(model.Email, "Registration", text, text);
                return Ok();
            }
        }

        /// <summary>
        /// Register new investor
        /// </summary>
        [HttpPost]
        [Route("investor/auth/signUp")]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(void))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Type = typeof(ErrorViewModel))]
        public async Task<IActionResult> RegisterInvestor([FromBody]RegisterInvestorViewModel model)
        {
            logger.LogInformation($"Register investor: {model?.Email}");

            if (!ModelState.IsValid)
                return BadRequest(ErrorResult.GetResult(ModelState));

            var address = ethService.GenerateAddress();
            var user = new ApplicationUser
                       {
                           UserName = model.Email,
                           Email = model.Email,
                           IsEnabled = true,
                           Type = UserType.Investor,
                           Profile = new Profiles(),
                           Wallets = new List<Wallets> {new Wallets {Currency = Currency.GVT}},
                           InvestorAccount = new InvestorAccounts(),
                           BlockchainAddresses = new List<BlockchainAddresses>
                                                 {
                                                     new BlockchainAddresses
                                                     {
                                                         Address = address.PublicAddress,
                                                         Currency = Currency.GVT,
                                                         IsDefault = true
                                                     }
                                                 }
                       };
            var result = await userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
                return BadRequest(ErrorResult.GetResult(result));

            var code = await userManager.GenerateEmailConfirmationTokenAsync(user);

            if (Constants.IsDevelopment)
            {
                await userManager.ConfirmEmailAsync(user, code);
                return Ok();
            }
            else
            {
                var callbackUrl = Url.Action("ConfirmEmail", "Account", new {userId = user.Id, code}, HttpContext.Request.Scheme);
                var text = $"Confirmation url: {callbackUrl}";
                emailSender.SendEmailAsync(model.Email, "Registration", text, text);
                return Ok();
            }
        }

        /// <summary>
        /// Confirm email after registration
        /// </summary>
        [HttpGet]
        [Route("manager/auth/confirmEmail")]
        [Route("investor/auth/confirmEmail")]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(void))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Type = typeof(ErrorViewModel))]
        public async Task<IActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
                return BadRequest(ErrorResult.GetResult("Empty userId/code"));

            var user = await userManager.FindByIdAsync(userId);
            var result = await userManager.ConfirmEmailAsync(user, code);
            if (result.Succeeded)
                return BadRequest(ErrorResult.GetResult(result));

            return Ok();
        }
    }
}
