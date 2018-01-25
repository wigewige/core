using System;
using GenesisVision.Core.ViewModels.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace GenesisVision.Core.ViewModels.Investment
{
    public enum Sorting
    {
        ByRatingAsc = 0,
        ByRatingDesc = 1,
        ByProfitAsc = 2,
        ByProfitDesc = 3,
        ByOrdersAsc = 4,
        ByOrdersDesc = 5
    }

    public class InvestmentsFilter : PagingFilter
    {
        public Guid? ManagerId { get; set; }
        public Guid? BrokerId { get; set; }
        public Guid? BrokerTradeServerId { get; set; }
        public decimal? InvestMaxAmountFrom { get; set; }
        public decimal? InvestMaxAmountTo { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public Sorting? Sorting { get; set; }
    }
}
