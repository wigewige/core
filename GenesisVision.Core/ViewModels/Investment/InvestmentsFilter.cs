using System;
using GenesisVision.Core.ViewModels.Other;

namespace GenesisVision.Core.ViewModels.Investment
{
    public class InvestmentsFilter : PagingFilter
    {
        public Guid? ManagerId { get; set; }
        public Guid? BrokerId { get; set; }
        public Guid? BrokerTradeServerId { get; set; }
        public decimal? InvestMaxAmountFrom { get; set; }
        public decimal? InvestMaxAmountTo { get; set; }
    }
}
