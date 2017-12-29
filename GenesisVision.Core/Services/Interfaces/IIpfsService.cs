using GenesisVision.Core.Models;

namespace GenesisVision.Core.Services.Interfaces
{
    public interface IIpfsService
    {
        OperationResult<string> GetIpfsText(string hash);

        OperationResult<string> WriteIpfsText(string text);

        OperationResult<byte[]> GetIpfsFile(string hash);
    }
}
