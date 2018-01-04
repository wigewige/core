using GenesisVision.PaymentService.Models;
using GenesisVision.PaymentService.Services.Interfaces;
using Nethereum.Hex.HexConvertors.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GenesisVision.PaymentService.Services
{
    public class EthService: IEthService
    {
        public EthAccount GenerateAddress()
        {
            var ecKey = Nethereum.Signer.EthECKey.GenerateKey();

            return new EthAccount
            {
                PrivateKey = ecKey.GetPrivateKey(),
                PublicAddress = ecKey.GetPublicAddress()
            };
        }
    }
}
