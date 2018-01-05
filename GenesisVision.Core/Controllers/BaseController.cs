using GenesisVision.DataModel.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;

namespace GenesisVision.Core.Controllers
{
    public class BaseController : Controller
    {
        private readonly UserManager<ApplicationUser> userManager;

        public ApplicationUser CurrentUser => GetCurrentUser();
        public string CurrentUserName => User?.Identity?.Name;
        public Guid? CurrentUserId => GetCurrentUserId();

        public BaseController(UserManager<ApplicationUser> userManager)
        {
            this.userManager = userManager;
        }

        private ApplicationUser GetCurrentUser()
        {
            if (!User.Identity.IsAuthenticated || string.IsNullOrEmpty(User.Identity?.Name))
                return null;

            var user = userManager.FindByNameAsync(User.Identity?.Name).Result;
            return user;
        }

        private Guid? GetCurrentUserId()
        {
            if (!User.Identity.IsAuthenticated || string.IsNullOrEmpty(User.Identity?.Name))
                return null;

            var id = Guid.Parse(User.Claims.First(x => x.Type == "id").Value);
            return id;
        }
    }
}
