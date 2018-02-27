using System;
using System.Collections.Generic;

namespace GenesisVision.DataModel.Models
{
    public class ManagerTokens
    {
        public Guid Id { get; set; }
        public string TokenSymbol { get; set; }
        public string TokenName { get; set; }
        public string TokenAddress { get; set; }
        public decimal InitialPrice { get; set; }
        public decimal FreeTokens { get; set; }

        public InvestmentPrograms InvestmentProgram { get; set; }

        public ICollection<Portfolios> Portfolios { get; set; }
    }
}
