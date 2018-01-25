using GenesisVision.DataModel.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace GenesisVision.Core.Controllers
{
    public class BaseController : Controller
    {
        private readonly UserManager<ApplicationUser> userManager;

        private ApplicationUser appUser;
        public ApplicationUser CurrentUser => GetCurrentUser();
        public string CurrentUserName => User?.Identity?.Name;

        public BaseController(UserManager<ApplicationUser> userManager)
        {
            this.userManager = userManager;
        }

        private ApplicationUser GetCurrentUser()
        {
            if (!User.Identity.IsAuthenticated || string.IsNullOrEmpty(User.Identity?.Name))
                return null;

            if (appUser != null)
                return appUser;

            appUser = userManager.FindByNameAsync(User.Identity?.Name).Result;
            return appUser;
        }
    }
}
