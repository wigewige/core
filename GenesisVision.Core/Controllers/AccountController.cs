using GenesisVision.Core.Helpers;
using GenesisVision.Core.Helpers.TokenHelper;
using GenesisVision.Core.Services.Interfaces;
using GenesisVision.Core.ViewModels.Account;
using GenesisVision.DataModel.Enums;
using GenesisVision.DataModel.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using GenesisVision.Core.Helpers.Convertors;

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
        public async Task<IActionResult> Authorize([FromBody]LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.FindByNameAsync(model.Email);
                if (user == null)
                    return BadRequest($"Wrong username/password");
                
                if (await userManager.CheckPasswordAsync(user, model.Password))
                {
                    if (!await userManager.IsEmailConfirmedAsync(user))
                        return BadRequest("Email does not confirmed");

                    if (!user.IsEnabled)
                        return BadRequest("User is disabled");

                    var token = JwtManager.GenerateToken(user);
                    return Ok(token.Value);
                }
            }
            else
            {
                var errors = ModelState.SelectMany(x => x.Value.Errors.Select(e => e.ErrorMessage));
                return BadRequest(errors);
            }

            return BadRequest($"Wrong username/password");
        }

        [HttpGet]
        [Authorize]
        [Route("manager/auth/updateToken")]
        [Route("investor/auth/updateToken")]
        [Route("broker/auth/updateToken")]
        public IActionResult UpdateAuthToken()
        {
            var user = CurrentUser;
            if (user.IsEnabled)
            {
                var token = JwtManager.GenerateToken(user);
                return Ok(token.Value);
            }

            return BadRequest(ValidationMessages.AccessDenied);
        }

        [HttpGet]
        [Authorize]
        [Route("manager/profile")]
        [Route("investor/profile")]
        public IActionResult ProfileShort()
        {
            var user = userService.GetUserProfileShort(CurrentUserId.Value);
            return Ok(user);
        }

        [HttpGet]
        [Authorize]
        [Route("manager/profile/full")]
        [Route("investor/profile/full")]
        public IActionResult ProfileFull()
        {
            var user = userService.GetUserProfileFull(CurrentUserId.Value);
            return Ok(user);
        }

        [HttpPost]
        [Authorize]
        [Route("manager/profile/update")]
        [Route("investor/profile/update")]
        public IActionResult UpdateProfile([FromBody]ProfileFullViewModel model)
        {
            userService.UpdateUserProfile(CurrentUserId.Value, model);
            return Ok();
        }

        [HttpPost]
        [Route("manager/auth/signUp")]
        public async Task<IActionResult> RegisterManager([FromBody]RegisterManagerViewModel model)
        {
            if (ModelState.IsValid)
            {
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
                    return BadRequest(result.Errors.Select(x => x.Description));

                var code = await userManager.GenerateEmailConfirmationTokenAsync(user);

                var callbackUrl = Url.Action("ConfirmEmail", "Account", new {userId = user.Id, code}, HttpContext.Request.Scheme);
                var text = $"Confirmation url: {callbackUrl}";
                emailSender.SendEmailAsync(model.Email, "Registration", text, text);

                return Ok();
            }

            var errors = ModelState.SelectMany(x => x.Value.Errors.Select(e => e.ErrorMessage));
            return BadRequest(errors);
        }

        [HttpPost]
        [Route("investor/auth/signUp")]
        public async Task<IActionResult> RegisterInvestor([FromBody]RegisterInvestorViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                           {
                               UserName = model.Email,
                               Email = model.Email,
                               IsEnabled = true,
                               Type = UserType.Investor,
                               Profile = new Profiles(),
                               Wallet = new Wallets()
                           };
                var result = await userManager.CreateAsync(user, model.Password);
                if (!result.Succeeded)
                    return BadRequest(result.Errors.Select(x => x.Description));

                var code = await userManager.GenerateEmailConfirmationTokenAsync(user);

                var callbackUrl = Url.Action("ConfirmEmail", "Account", new {userId = user.Id, code}, HttpContext.Request.Scheme);
                var text = $"Confirmation url: {callbackUrl}";
                emailSender.SendEmailAsync(model.Email, "Registration", text, text);
                    
                return Ok();
            }

            var errors = ModelState.SelectMany(x => x.Value.Errors.Select(e => e.ErrorMessage));
            return BadRequest(errors);
        }

        [HttpGet]
        [Route("manager/auth/confirmEmail")]
        [Route("investor/auth/confirmEmail")]
        public async Task<IActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
                return BadRequest($"Empty userId/code");

            var user = await userManager.FindByIdAsync(userId);
            var result = await userManager.ConfirmEmailAsync(user, code);
            if (result.Succeeded)
                return Ok();

            return BadRequest();
        }
    }
}
