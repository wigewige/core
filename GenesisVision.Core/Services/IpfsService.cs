using GenesisVision.Core.Helpers;
using GenesisVision.Core.Models;
using GenesisVision.Core.Services.Interfaces;

namespace GenesisVision.Core.Services
{
    public class IpfsService : IIpfsService
    {
        //private readonly IpfsClient ipfs;

        public IpfsService()
        {
            //ipfs = new IpfsClient(Constants.IpfsHost);
        }

        public OperationResult<string> GetIpfsText(string hash)
        {
            return InvokeOperations.InvokeOperation(() =>
            {
                return "";
                //var data = ipfs.FileSystem.ReadAllTextAsync(hash).Result;
                //return data;
            });
        }

        public OperationResult<string> WriteIpfsText(string text)
        {
            return InvokeOperations.InvokeOperation(() =>
            {
                return "";

                //var res = ipfs.FileSystem.AddTextAsync(text).Result;
                //return res.Id.Hash.ToString();
            });
        }

        public OperationResult<byte[]> GetIpfsFile(string hash)
        {
            return InvokeOperations.InvokeOperation(() =>
            {
                return new byte[0];

                //using (var stream = ipfs.FileSystem.ReadFileAsync(hash).Result)
                //using (var data = new MemoryStream())
                //{
                //    stream.CopyTo(data);
                //    return data.ToArray();
                //}
            });
        }
    }
}
