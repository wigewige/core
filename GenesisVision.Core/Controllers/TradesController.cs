using GenesisVision.Core.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GenesisVision.Core.Controllers
{
    [Authorize]
    [Route("api/trades")]
    public class TradesController : Controller
    {
        private readonly ApplicationDbContext context;

        public TradesController(ApplicationDbContext context)
        {
            this.context = context;
        }

        public IActionResult GetTrades(string managerId)
        {
            return Ok(null);
        }
    }
}
