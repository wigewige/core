using GenesisVision.Core.Helpers;
using GenesisVision.Core.Helpers.TokenHelper;
using GenesisVision.Core.Services.Interfaces;
using GenesisVision.Core.ViewModels.Account;
using GenesisVision.DataModel.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace GenesisVision.Core.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/account")]
    public class AccountController : BaseController
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IEmailSender emailSender;

        public AccountController(UserManager<ApplicationUser> userManager, IEmailSender emailSender)
            : base(userManager)
        {
            this.userManager = userManager;
            this.emailSender = emailSender;
        }

        [HttpPost]
        [Route("authorize")]
        public async Task<IActionResult> Authorize([FromBody]LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.FindByNameAsync(model.Username);
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

        [Authorize]
        [Route("updateAuthToken")]
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

        [Authorize]
        [Route("details")]
        public IActionResult Details()
        {
            var user = CurrentUser;
            var model = new AccountViewModel
                        {
                            UserName = user.UserName,
                            Email = user.Email,
                            Balance = 0m
                        };
            return Ok(model);
        }

        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody]RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser {UserName = model.Username, Email = model.Email, IsEnabled = true};
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

        [Route("confirmEmail")]
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
