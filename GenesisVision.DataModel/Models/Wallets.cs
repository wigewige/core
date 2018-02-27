using GenesisVision.DataModel.Enums;
using System;
using System.Collections.Generic;

namespace GenesisVision.DataModel.Models
{
    public class Wallets
    {
        public Guid Id { get; set; }
        public decimal Amount { get; set; }
        public Currency Currency { get; set; }

        public Guid UserId { get; set; }
        public ApplicationUser User { get; set; }

        public ICollection<WalletTransactions> WalletTransactions { get; set; }
    }
}
