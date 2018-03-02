using GenesisVision.Core.Models;
using GenesisVision.Core.Services.Interfaces;
using GenesisVision.Core.ViewModels.Common;
using GenesisVision.Core.ViewModels.Files;
using GenesisVision.DataModel.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;

namespace GenesisVision.Core.Controllers
{
    [Route("api")]
    [ApiVersion("1.0")]
    public class FilesController : BaseController
    {
        private readonly IFileService fileService;
        private readonly ILogger<FilesController> logger;

        public FilesController(UserManager<ApplicationUser> userManager, IFileService fileService, ILogger<FilesController> logger) 
            : base(userManager)
        {
            this.fileService = fileService;
            this.logger = logger;
        }

        /// <summary>
        /// Upload file
        /// </summary>
        [HttpPost]
        [Route("files/upload")]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(UploadResult))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Type = typeof(ErrorViewModel))]
        public IActionResult UploadFile(IFormFile uploadedFile)
        {
            var result = fileService.Upload(uploadedFile, CurrentUser?.Id);
            if (!result.IsSuccess)
                return BadRequest(ErrorResult.GetResult(result));

            return Ok(result.Data);
        }

        /// <summary>
        /// Download file
        /// </summary>
        [HttpGet]
        [Route("files")]
        [Route("files/{id}")]
        public FileResult Get(Guid id)
        {
            var result = fileService.GetFile(id);
            if (!result.IsSuccess)
                return File(new byte[0], "");

            return File(result.Data.Item2, result.Data.Item1.ContentType, result.Data.Item1.FileName);
        }
    }
}
