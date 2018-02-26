using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GenesisVision.Core.ViewModels.Wallet
{
    public class WalletWithdrawRequestModel
    {
        public string Currency { get; set; }
        public decimal Amount { get; set; }
        public string BlockchainAddress { get; set; }
    }
}
