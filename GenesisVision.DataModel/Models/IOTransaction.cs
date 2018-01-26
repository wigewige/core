using GenesisVision.DataModel.Enums;
using System;

namespace GenesisVision.DataModel.Models
{

    public class IOTransactions
    {
        public Guid Id { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public IOTransactionType Type { get; set; }
        public IOTransactionStatus Status { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime ConfirmationDate { get; set; }

        public ApplicationUser User { get; set; }
        public Guid UserId { get; set; }

        public Wallets Wallet { get; set; }
        public Guid WalletId { get; set; }

        public WalletTransactions WalletTransaction { get; set; }
        public Guid WalletTransactionId { get; set; }
    }
}
