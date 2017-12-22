using System.Linq;
using GenesisVision.Core.Models;
using GenesisVision.Core.Services.Interfaces;
using GenesisVision.Core.Services.Validators.Interfaces;
using GenesisVision.Core.ViewModels.Investment;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GenesisVision.Core.Controllers
{
    //[Authorize]
    public class InvestorController : Controller
    {
        private readonly ITrustManagementService trustManagementService;
        private readonly IInvestorValidator investorValidator;

        public InvestorController(ITrustManagementService trustManagementService, IInvestorValidator investorValidator)
        {
            this.trustManagementService = trustManagementService;
            this.investorValidator = investorValidator;
        }

        /// <summary>
        /// Invest in manager
        /// </summary>
        public IActionResult Invest([FromBody]Invest model)
        {
            var errors = investorValidator.ValidateInvest(User, model);
            if (errors.Any())
                return BadRequest(OperationResult.Failed(errors));

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
