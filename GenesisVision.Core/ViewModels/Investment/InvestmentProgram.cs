﻿using System;

namespace GenesisVision.Core.ViewModels.Investment
{
    public class InvestmentProgram
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public int Level { get; set; }
        public string Logo { get; set; }
        public decimal Balance { get; set; }
        public int TradesCount { get; set; }
        public int InvestorsCount { get; set; }
        public int PeriodDuration { get; set; }
        public DateTime EndOfPeriod { get; set; }
        public decimal ProfitAvg { get; set; }
        public decimal AvailableInvestment { get; set; }
        public decimal FeeSuccess { get; set; }
        public decimal FeeManagement { get; set; }

        public bool IsPending { get; set; }
    }
}
