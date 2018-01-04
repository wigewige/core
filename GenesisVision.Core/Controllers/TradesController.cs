using GenesisVision.Core.Services.Interfaces;
using GenesisVision.DataModel.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace GenesisVision.Core.Controllers
{
    //[Authorize]
    public class TradesController : BaseController
    {
        private readonly IIpfsService ipfsService;
        private readonly ITradesService tradesServer;

        public TradesController(IIpfsService ipfsService, ITradesService tradesServer, UserManager<ApplicationUser> userManager)
            : base(userManager)
        {
            this.ipfsService = ipfsService;
            this.tradesServer = tradesServer;
        }

        /// <summary>
        /// Get trades by IPFS hash id
        /// </summary>
        public IActionResult GetTrades(string ipfsHashId)
        {
            var text = ipfsService.GetIpfsText(ipfsHashId);
            if (!text.IsSuccess)
                return BadRequest();

            var trades = tradesServer.ConvertMetaTraderOrdersFromCsv(text.Data);
            if (!trades.IsSuccess)
                return BadRequest();

            return Ok(trades);
        }
    }
}
