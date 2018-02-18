using System;
using System.Collections.Generic;

namespace GenesisVision.DataModel.Models
{
    public class Wallets
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public ApplicationUser User { get; set; }

        public decimal Amount { get; set; }
        public BlockchainAddresses CurrentAddress { get; set; }
        public Guid CurrentAddressId { get;set; }

        public ICollection<IOTransactions> IOTransactions { get; set; }
        public ICollection<WalletTransactions> WalletTransactions { get; set; }
        public ICollection<BlockchainAddresses> BlockchainAddresses { get; set; }
    }
}
