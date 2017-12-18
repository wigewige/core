using GenesisVision.Core.Services.Interfaces;
using GenesisVision.Core.ViewModels.Manager;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GenesisVision.Core.Controllers
{
    [Authorize]
    [Route("api/manager")]
    public class ManagerController : Controller
    {
        private readonly ITrustManagementService trustManagementService;
        private readonly IManagerService managerService;

        public ManagerController(ITrustManagementService trustManagementService, IManagerService managerService)
        {
            this.trustManagementService = trustManagementService;
            this.managerService = managerService;
        }
        
        public IActionResult NewManagerAccountRequest([FromBody]NewManagerRequest request)
        {
            var res = managerService.CreateManagerAccountRequest(null);
            return Ok(res);
        }
        
        public IActionResult CreateManagerAccount([FromBody]NewManager request)
        {
            var res = managerService.CreateManagerAccount(request);
            return Ok(res);
        }
    }
}
