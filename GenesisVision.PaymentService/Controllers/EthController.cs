using GenesisVision.Common.Models;
using GenesisVision.Common.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace GenesisVision.PaymentService.Controllers
{
    [Route("api/[controller]")]
    public class EthController : Controller
    {
        private readonly IEthService ethService;

        public EthController(IEthService ethService)
        {
            this.ethService = ethService;
        }

        [HttpGet]
        public EthAccount GenerateAccount()
        {
            return ethService.GenerateAddress();
        }
    }
}
