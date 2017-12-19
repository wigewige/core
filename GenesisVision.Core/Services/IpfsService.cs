using GenesisVision.Core.Data;
using GenesisVision.Core.Models;
using GenesisVision.Core.Services.Interfaces;
using Ipfs.Api;
using System;
using System.IO;

namespace GenesisVision.Core.Services
{
    public class IpfsService : IIpfsService
    {
        private readonly IpfsClient ipfs;

        public IpfsService()
        {
            ipfs = new IpfsClient(Constants.IpfsHost);
        }

        public OperationResult<string> GetIpfsText(string hash)
        {
            try
            {
                var data = ipfs.FileSystem.ReadAllTextAsync(hash).Result;
                return OperationResult<string>.Ok(data);
            }
            catch (Exception e)
            {
                return OperationResult<string>.Failed(e.Message);
            }
        }

        public OperationResult<byte[]> GetIpfsFile(string hash)
        {
            try
            {
                using (var stream = ipfs.FileSystem.ReadFileAsync(hash).Result)
                using (var data = new MemoryStream())
                {
                    stream.CopyTo(data);
                    return OperationResult<byte[]>.Ok(data.ToArray());
                }
            }
            catch (Exception e)
            {
                return OperationResult<byte[]>.Failed(e.Message);
            }
        }
    }
}
