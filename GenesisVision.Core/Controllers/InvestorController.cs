using GenesisVision.Core.Models;
using GenesisVision.Core.Services.Interfaces;
using GenesisVision.Core.Services.Validators.Interfaces;
using GenesisVision.Core.ViewModels.Investment;
using GenesisVision.DataModel.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace GenesisVision.Core.Controllers
{
    [Authorize]
    [Route("api")]
    [ApiVersion("1.0")]
    public class InvestorController : BaseController
    {
        private readonly ITrustManagementService trustManagementService;
        private readonly IInvestorValidator investorValidator;

        public InvestorController(ITrustManagementService trustManagementService, IInvestorValidator investorValidator, UserManager<ApplicationUser> userManager)
            : base(userManager)
        {
            this.trustManagementService = trustManagementService;
            this.investorValidator = investorValidator;
        }

        /// <summary>
        /// Invest in manager
        /// </summary>
        [Route("investor/investment/invest")]
        public IActionResult Invest([FromBody]Invest model)
        {
            var errors = investorValidator.ValidateInvest(CurrentUser, model);
            if (errors.Any())
                return BadRequest(OperationResult.Failed(errors));

            var res = trustManagementService.Invest(model);
            return Ok(res);
        }

        /// <summary>
        /// Get investments by filter
        /// </summary>
        [Route("investor/investment/search")]
        public IActionResult GetInvestments([FromBody]InvestmentsFilter filter)
        {
            var res = trustManagementService.GetInvestments(filter);
            return Ok(res);
        }
    }
}
