using GenesisVision.Core.Helpers.TokenHelper;
using GenesisVision.Core.Services.Interfaces;
using GenesisVision.Core.ViewModels.Account;
using GenesisVision.DataModel.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace GenesisVision.Core.Controllers
{
    public class AccountController : BaseController
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly IEmailSender emailSender;

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IEmailSender emailSender)
            : base(userManager)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.emailSender = emailSender;
        }

        [HttpPost]
        public async Task<IActionResult> Authorize([FromBody]LoginViewModel model)
        {
            if (string.IsNullOrEmpty(model?.Email) || string.IsNullOrEmpty(model?.Password))
                return BadRequest($"Empty email/password");

            var user = await userManager.FindByEmailAsync(model.Email);
            if (user == null)
                return BadRequest($"Wrong email/password");

            if (!await userManager.IsEmailConfirmedAsync(user))
                return BadRequest("Email does not confirmed");

            var result = await signInManager.PasswordSignInAsync(user, model.Password, model.RememberMe, false);
            if (result.Succeeded)
            {
                var token = JwtManager.GenerateToken(user);
                return Ok(token.Value);
            }

            return BadRequest($"Wrong email/password");
        }

        [HttpPost]
        public async Task<IActionResult> Register([FromBody]RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser {UserName = model.Username, Email = model.Email};
                var result = await userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    var code = await userManager.GenerateEmailConfirmationTokenAsync(user);
                    
                    var callbackUrl = Url.Action("ConfirmEmail", "Account", new {userId = user.Id, code}, HttpContext.Request.Scheme);
                    var text = $"Confirmation url: {callbackUrl}";
                    emailSender.SendEmailAsync(model.Email, "Registration", text, text);
                    
                    return Ok();
                }
            }

            return BadRequest();
        }
        
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
