using GenesisVision.Core.Services.Interfaces;
using GenesisVision.Core.ViewModels.Investment;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GenesisVision.Core.Controllers
{
    //[Authorize]
    public class InvestorController : Controller
    {
        private readonly ITrustManagementService trustManagementService;

        public InvestorController(ITrustManagementService trustManagementService)
        {
            this.trustManagementService = trustManagementService;
        }

        /// <summary>
        /// Invest in manager
        /// </summary>
        public IActionResult Invest([FromBody]Invest model)
        {
            var res = trustManagementService.Invest(model);
            return Ok(res);
        }

        /// <summary>
        /// Get investments by filter
        /// </summary>
        public IActionResult GetInvestments([FromBody]InvestmentsFilter filter)
        {
            var res = trustManagementService.GetInvestments(filter);
            return Ok(res);
        }
    }
}
