﻿using System.Collections.Generic;

namespace GenesisVision.Core.ViewModels.Investment
{
    public class InvestmentProgramViewModel
    {
        public InvestmentProgramDetails InvestmentProgram { get; set; }

        public List<InvestmentProgramStatistic> Statistic { get; set; }
    }
}
