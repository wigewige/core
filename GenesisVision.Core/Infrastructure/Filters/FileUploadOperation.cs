using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace GenesisVision.Core.Infrastructure.Filters
{
    public class FileUploadOperation : IOperationFilter
    {
        public void Apply(Operation operation, OperationFilterContext context)
        {
            if (operation.OperationId.ToLower() == "apifilesuploadpost")
            {
                operation.Parameters.Clear();
                operation.Parameters.Add(new NonBodyParameter
                                         {
                                             Name = "uploadedFile",
                                             In = "formData",
                                             Description = "Upload File",
                                             Required = true,
                                             Type = "file"
                                         });
                operation.Consumes.Add("multipart/form-data");
            }
        }
    }
}
