using GenesisVision.Common.Services.Interfaces;
using GenesisVision.Core.Models;
using GenesisVision.Core.ViewModels.Common;
using GenesisVision.Core.ViewModels.Rate;
using GenesisVision.DataModel.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace GenesisVision.Core.Controllers
{
    [Route("api")]
    [ApiVersion("1.0")]
    public class RateController : BaseController
    {
        private readonly IRateService rateService;
        private readonly ILogger<RateController> logger;

        public RateController(UserManager<ApplicationUser> userManager, IRateService rateService, ILogger<RateController> logger)
            : base(userManager)
        {
            this.rateService = rateService;
            this.logger = logger;
        }

        /// <summary>
        /// Get rate
        /// </summary>
        [HttpPost]
        [Route("rate")]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(RateViewModel))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Type = typeof(ErrorViewModel))]
        public IActionResult GetRate([FromBody]RequestRate model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ErrorResult.GetResult(ModelState));

            var rate = rateService.GetRate(model.From, model.To);
            if (!rate.IsSuccess)
                return BadRequest(ErrorResult.GetResult(rate));

            return Ok(new RateViewModel
                      {
                          From = model.From,
                          To = model.To,
                          Rate = rate.Data
                      });
        }
    }
}
