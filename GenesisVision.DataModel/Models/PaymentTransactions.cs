﻿using GenesisVision.DataModel.Enums;
using System;

namespace GenesisVision.DataModel.Models
{
    public class PaymentTransactions
    {
        public Guid Id { get; set; }
        public string Hash { get; set; }
        public decimal Amount { get; set; }
        public decimal Fee { get; set; }
        public PaymentTransactionType Type { get; set; }
        public PaymentTransactionStatus Status { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime? LastUpdated { get; set; }
        public DateTime? PaymentTxDate { get; set; }
        public string ExtraData { get; set; }
        public string PayoutTxHash { get; set; }
        public decimal? PayoutMinerFee { get; set; }
        public decimal? PayoutServiceFee { get; set; }
        public PaymentTransactionStatus PayoutStatus { get; set; }
        public string DestAddress { get; set; }
        public BlockchainAddresses BlockchainAddress { get; set; }
        public Guid BlockchainAddressId { get; set; }

        public WalletTransactions WalletTransaction { get; set; }
        public Guid WalletTransactionId { get; set; }
    }
}
