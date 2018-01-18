using GenesisVision.DataModel.Enums;
using System;

namespace GenesisVision.DataModel.Models
{

    public class IOTransactions
    {
        public Guid Id { get; set; }
        public string Currency { get; set; }
        public IOTransactionType Type { get; set; }
        public IOTransactionStatus Status { get; set; }

        public ApplicationUser User { get; set; }
        public Guid UserId { get; set; }

        public Wallets UserWallet { get; set; }
        public Guid? UserWalletId { get; set; }

        public DateTime CreationDate { get; set; }
        public DateTime ConfirmationDate { get; set; }
    }
}
