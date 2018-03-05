using System;
using System.Collections.Generic;

namespace GenesisVision.Core.ViewModels.Wallet
{
    public class WalletInvestmentPrograms
    {
        public IEnumerable<WalletInvestmentProgram> InvestmentPrograms { get; set; }
    }

    public class WalletInvestmentProgram
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
    }
}
