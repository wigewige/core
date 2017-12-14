using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GenesisVision.Core.Controllers
{
    [Authorize]
    [Route("api/trades")]
    public class TradesController : Controller
    {
        public TradesController()
        {
        }

        public IActionResult GetTrades(string managerId)
        {
            return Ok(null);
        }
    }
}
