using GenesisVision.Core.Helpers;
using GenesisVision.DataModel.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;

namespace GenesisVision.Core.Controllers
{
    public class BaseController : Controller
    {
        private readonly UserManager<ApplicationUser> userManager;

        public ApplicationUser CurrentUser => GetCurrentUser();
        public string CurrentUserName => User?.Identity?.Name;
        public Guid? CurrentUserId => User?.GetUserId();

        public BaseController(UserManager<ApplicationUser> userManager)
        {
            this.userManager = userManager;
        }

        private ApplicationUser GetCurrentUser()
        {
            if (!User.Identity.IsAuthenticated)
                return null;

            var user = userManager.GetUserAsync(User).Result;
            return user;
        }
    }
}
