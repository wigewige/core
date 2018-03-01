using GenesisVision.Common.Models;
using GenesisVision.Core.ViewModels.Files;
using GenesisVision.DataModel.Models;
using Microsoft.AspNetCore.Http;
using System;

namespace GenesisVision.Core.Services.Interfaces
{
    public interface IFileService
    {
        OperationResult<UploadResult> Upload(IFormFile uploadedFile, Guid? userId);

        OperationResult<(Files, byte[])> GetFile(Guid id);
    }
}
