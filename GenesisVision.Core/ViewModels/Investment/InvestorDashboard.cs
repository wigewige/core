using System.Collections.Generic;

namespace GenesisVision.Core.ViewModels.Investment
{
    public class InvestorDashboard
    {
        public List<InvestmentProgramDashboard> InvestmentPrograms { get; set; }

        public int Total { get; set; }
    }
}
