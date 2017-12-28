using System;

namespace GenesisVision.Core.Data.Models
{
    public class ManagerTokens
    {
        public Guid Id { get; set; }
        public string TokenAddress { get; set; }
        public string TokenSymbol { get; set; }

        public InvestmentPrograms InvestmentProgram { get; set; }
    }
}
