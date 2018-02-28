using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using GenesisVision.Core.Helpers;
using GenesisVision.Core.Models;
using GenesisVision.Core.Services.Interfaces;
using GenesisVision.Core.ViewModels.Files;
using GenesisVision.DataModel;
using GenesisVision.DataModel.Models;
using Microsoft.AspNetCore.Http;

namespace GenesisVision.Core.Services
{
    public class FileService : IFileService
    {
        private readonly ApplicationDbContext context;

        public FileService(ApplicationDbContext context)
        {
            this.context = context;
        }

        public OperationResult<UploadResult> Upload(IFormFile uploadedFile, Guid? userId)
        {
            return InvokeOperations.InvokeOperation(() =>
            {
                var fileName = Guid.NewGuid() + (uploadedFile.FileName.Contains(".")
                                   ? uploadedFile.FileName.Substring(uploadedFile.FileName.LastIndexOf(".", StringComparison.Ordinal))
                                   : "");
                using (var stream = new FileStream(Path.Combine(Constants.UploadPath, fileName), FileMode.Create))
                {
                    uploadedFile.CopyTo(stream);
                }

                var file = new Files
                           {
                               Id = Guid.NewGuid(),
                               UploadDate = DateTime.Now,
                               UserId = userId,
                               Path = fileName,
                               FileName = uploadedFile.FileName,
                               ContentType = uploadedFile.ContentType
                           };
                context.Add(file);
                context.SaveChanges();

                return new UploadResult {Id = file.Id};
            });
        }

        public OperationResult<(Files, byte[])> GetFile(Guid id)
        {
            return InvokeOperations.InvokeOperation(() =>
            {
                var file = context.Files.FirstOrDefault(x => x.Id == id);
                if (file == null)
                    throw new FileNotFoundException("File not found");

                var bytes = File.ReadAllBytes(Path.Combine(Constants.UploadPath, file.Path));
                return (file, bytes);
            });
        }
    }
}
