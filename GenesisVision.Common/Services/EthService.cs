using GenesisVision.Common.Models;
using GenesisVision.Common.Services.Interfaces;
using Nethereum.Signer;

namespace GenesisVision.Common.Services
{
    public class EthService : IEthService
    {
        public EthAccount GenerateAddress()
        {
            var ecKey = EthECKey.GenerateKey();

            return new EthAccount
                   {
                       PrivateKey = ecKey.GetPrivateKey(),
                       PublicAddress = ecKey.GetPublicAddress()
                   };
        }
    }
}
