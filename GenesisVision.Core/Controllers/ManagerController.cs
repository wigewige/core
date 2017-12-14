using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GenesisVision.Core.Controllers
{
    [Authorize]
    [Route("api/manager")]
    public class ManagerController : Controller
    {
        public ManagerController()
        {
        }

        public IActionResult CreateAccount()
        {
            return Ok();
        }
    }
}
