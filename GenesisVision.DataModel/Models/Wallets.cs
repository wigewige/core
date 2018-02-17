using System;
using System.Collections.Generic;

namespace GenesisVision.DataModel.Models
{
    public class Wallets
    {
        public Guid UserId { get; set; }
        public ApplicationUser User { get; set; }

        public decimal Amount { get; set; }
        public EthAddresses CurrentAddress { get; set; }
        public Guid CurrentAddressId { get;set; }

        public ICollection<IOTransactions> IOTransactions { get; set; }
        public ICollection<WalletTransactions> WalletTransactions { get; set; }
        public ICollection<EthAddresses> EthAddresses { get; set; }
    }
}
