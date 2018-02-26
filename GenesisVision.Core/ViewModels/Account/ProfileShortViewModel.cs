using System;
using System.Collections.Generic;

namespace GenesisVision.Core.ViewModels.Account
{
    public class ProfileShortViewModel
    {
        public Guid Id { get; set; }

        public string Email { get; set; }

        public IEnumerable<WalletViewModel> Wallets { get; set; }
    }
}
