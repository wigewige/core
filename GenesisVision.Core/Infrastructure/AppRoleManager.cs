using GenesisVision.DataModel.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace GenesisVision.Core.Infrastructure
{
    public class AppRoleManager : RoleManager<ApplicationUser>
    {
        public AppRoleManager(IRoleStore<ApplicationUser> store,
            IEnumerable<IRoleValidator<ApplicationUser>> roleValidators, ILookupNormalizer keyNormalizer,
            IdentityErrorDescriber errors, ILogger<RoleManager<ApplicationUser>> logger)
            : base(store, roleValidators, keyNormalizer, errors, logger)
        {
        }
    }
}
