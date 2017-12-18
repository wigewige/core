using System;

namespace GenesisVision.Core.ViewModels.Investment
{
    public class CreateInvestment
    {
        public Guid ManagersAccountId { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public string Description { get; set; }
        public decimal FeeEntrance { get; set; }
        public decimal FeeManagement { get; set; }
        public decimal FeeSuccess { get; set; }
        public decimal InvestMinAmount { get; set; }
        public decimal? InvestMaxAmount { get; set; }
        public int Period { get; set; }
    }
}
