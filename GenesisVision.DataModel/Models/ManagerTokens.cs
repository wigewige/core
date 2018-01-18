using System;

namespace GenesisVision.DataModel.Models
{
    public class ManagerTokens
    {
        public Guid Id { get; set; }
        public string TokenSymbol { get; set; }
        public string TokenName { get; set; }
        public string TokenAddress { get; set; }

        public InvestmentPrograms InvestmentProgram { get; set; }
    }
}
