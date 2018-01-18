using System;

namespace GenesisVision.DataModel.Models
{
    public class Wallets
    {
        public Guid UserId { get; set; }
        public ApplicationUser User { get; set; }

        public decimal Amount { get; set; }
        public string Address { get; set; }
    }
}
