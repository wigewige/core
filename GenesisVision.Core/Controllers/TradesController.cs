using GenesisVision.Core.Data;
using GenesisVision.Core.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GenesisVision.Core.Controllers
{
    //[Authorize]
    public class TradesController : Controller
    {
        private readonly IIpfsService ipfsService;
        private readonly ITradesService tradesServer;

        public TradesController(IIpfsService ipfsService, ITradesService tradesServer)
        {
            this.ipfsService = ipfsService;
            this.tradesServer = tradesServer;
        }

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
