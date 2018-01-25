using GenesisVision.Core.Models;
using GenesisVision.Core.Services.Interfaces;
using GenesisVision.Core.ViewModels.Common;
using GenesisVision.Core.ViewModels.Trades;
using GenesisVision.DataModel.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace GenesisVision.Core.Controllers
{
    [Route("api")]
    [ApiVersion("1.0")]
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
        [HttpGet]
        [Route("trades/ipfsHistory")]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(TradesViewModel))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Type = typeof(ErrorViewModel))]
        public IActionResult GetTrades(string ipfsHashId)
        {
            var text = ipfsService.GetIpfsText(ipfsHashId);
            if (!text.IsSuccess)
                return BadRequest(ErrorResult.GetResult(text));

            var trades = tradesServer.ConvertMetaTraderOrdersFromCsv(text.Data);
            if (!trades.IsSuccess)
                return BadRequest(ErrorResult.GetResult(trades));

            return Ok(new TradesViewModel
                      {
                          Trades = trades.Data
                      });
        }
    }
}
