using GenesisVision.Core.Helpers;
using GenesisVision.Core.Helpers.TokenHelper;
using GenesisVision.Core.Models;
using GenesisVision.Core.Services.Interfaces;
using GenesisVision.Core.ViewModels.Account;
using GenesisVision.Core.ViewModels.Other;
using GenesisVision.DataModel.Enums;
using GenesisVision.DataModel.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Threading.Tasks;

namespace GenesisVision.Core.Controllers
{
    [Route("api")]
    [ApiVersion("1.0")]
    public class AccountController : BaseController
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IEmailSender emailSender;
        private readonly IUserService userService;

        public AccountController(UserManager<ApplicationUser> userManager, IEmailSender emailSender, IUserService userService)
            : base(userManager)
        {
            this.userManager = userManager;
            this.emailSender = emailSender;
            this.userService = userService;
        }

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
                return BadRequest(ErrorResult.GetResult($"Wrong username/password"));

            if (await userManager.CheckPasswordAsync(user, model.Password))
            {
                if (!await userManager.IsEmailConfirmedAsync(user))
                    return BadRequest(ErrorResult.GetResult("Email does not confirmed"));

                if (!user.IsEnabled)
                    return BadRequest(ErrorResult.GetResult("User is disabled"));

                var token = JwtManager.GenerateToken(user);
                return Ok(token.Value);
            }

            return BadRequest(ErrorResult.GetResult("Wrong username/password"));
        }

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

        [HttpGet]
        [Authorize]
        [Route("manager/profile")]
        [Route("investor/profile")]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(ProfileShortViewModel))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Type = typeof(ErrorViewModel))]
        public IActionResult ProfileShort()
        {
            var user = userService.GetUserProfileShort(CurrentUser.Id);
            if (!user.IsSuccess)
                return BadRequest(ErrorResult.GetResult(user));

            return Ok(user.Data);
        }

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

        [HttpPost]
        [Authorize]
        [Route("manager/profile/update")]
        [Route("investor/profile/update")]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(void))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Type = typeof(ErrorViewModel))]
        public IActionResult UpdateProfile([FromBody]ProfileFullViewModel model)
        {
            var res = userService.UpdateUserProfile(CurrentUser.Id, model);
            if (!res.IsSuccess)
                return BadRequest(ErrorResult.GetResult(res));

            return Ok();
        }

        [HttpPost]
        [Route("manager/auth/signUp")]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(void))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Type = typeof(ErrorViewModel))]
        public async Task<IActionResult> RegisterManager([FromBody]RegisterManagerViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ErrorResult.GetResult(ModelState));

            var user = new ApplicationUser
                       {
                           UserName = model.Email,
                           Email = model.Email,
                           IsEnabled = true,
                           Type = UserType.Manager,
                           Profile = new Profiles(),
                           Wallet = new Wallets()
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

        [HttpPost]
        [Route("investor/auth/signUp")]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(void))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Type = typeof(ErrorViewModel))]
        public async Task<IActionResult> RegisterInvestor([FromBody]RegisterInvestorViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ErrorResult.GetResult(ModelState));

            var user = new ApplicationUser
                       {
                           UserName = model.Email,
                           Email = model.Email,
                           IsEnabled = true,
                           Type = UserType.Investor,
                           Profile = new Profiles(),
                           Wallet = new Wallets(),
                           InvestorAccount = new InvestorAccounts()
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
