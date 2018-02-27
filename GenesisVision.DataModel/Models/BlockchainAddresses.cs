using GenesisVision.DataModel.Enums;
using System;
using System.Collections.Generic;

namespace GenesisVision.DataModel.Models
{
    public class BlockchainAddresses
    {
        public Guid Id { get; set; }
        public Currency Currency { get; set; }
        public string Address { get; set; }
        public bool IsDefault { get; set; }

        public ApplicationUser User { get; set; }
        public Guid UserId { get; set; }

        public ICollection<PaymentTransactions> PaymentTransactions { get; set; }
    }
}
