using GenesisVision.DataModel.Enums;
using System;

namespace GenesisVision.DataModel.Models
{
    public class UserWallets
    {
        public Guid Id { get; set; }
        public WalletType WalletType { get; set; }
        public string Address { get; set; }
    }
}
