using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using GenesisVision.Core.Helpers;
using GenesisVision.Core.Helpers.TokenHelper;
using GenesisVision.Core.ViewModels.Account;
using GenesisVision.DataModel.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace GenesisVision.Core.Controllers
{
    public class AccountController : BaseController
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
            : base(userManager)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
        }

        [HttpPost]
        public async Task<IActionResult> Authorize([FromBody]LoginViewModel model)
        {
            if (string.IsNullOrEmpty(model?.Email) || string.IsNullOrEmpty(model?.Password))
                return BadRequest($"Empty email/password");

            var user = await userManager.FindByEmailAsync(model.Email);
            if (user == null)
                return BadRequest($"User with email '{model.Email}' not found");

            var result = await signInManager.PasswordSignInAsync(user, model.Password, model.RememberMe, false);
            if (result.Succeeded)
            {
                var token = JwtManager.GenerateToken(user);
                return Ok(token.Value);
            }

            return BadRequest();
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
                    
                    // todo
                    //var callbackUrl = Url.Action("ConfirmEmail", "Account", new {userId = user.Id, code = code}, protocol: HttpContext.Request.Scheme);
                    //emailSender.SendEmail(model.Email, callbackUrl);

                    var confirm = await userManager.ConfirmEmailAsync(user, code);

                    return Ok();
                }
            }
            
            return BadRequest();
        }
    }
}
