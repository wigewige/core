using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using GenesisVision.PaymentService.Models;
using GenesisVision.PaymentService.Services.Interfaces;

namespace GenesisVision.PaymentService.Controllers
{
    [Route("api/[controller]")]
    public class EthController : Controller
    {
        private IEthService ethService;

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
