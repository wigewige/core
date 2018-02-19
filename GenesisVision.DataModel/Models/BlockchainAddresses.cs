﻿using System;

namespace GenesisVision.DataModel.Models
{
    public class BlockchainAddresses
    {
        public Guid Id { get; set; }

        public Wallets Wallet { get; set; }
        public Guid WalletId { get; set; }

        public string Currency { get; set; }


        public string Address { get; set; }
    }
}