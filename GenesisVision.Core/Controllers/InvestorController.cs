﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GenesisVision.Core.Controllers
{
    [Authorize]
    [Route("api/investor")]
    public class InvestorController : Controller
    {
        public InvestorController()
        {
        }

        public IActionResult Invest()
        {
            return Ok();
        }
    }
}
