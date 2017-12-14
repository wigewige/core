using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GenesisVision.Core.Controllers
{
    [Authorize]
    [Route("api/broker")]
    public class BrokerController : Controller
    {
        public BrokerController()
        {
        }

        public IActionResult RegisterManager()
        {
            return Ok();
        }

        public IActionResult GetManagers()
        {
            return Ok(null);
        }
    }
}
