using GenesisVision.Core.ViewModels.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;

namespace GenesisVision.Core.ViewModels.Wallet
{
    public enum WalletTxType
    {
        All = 0,
        Internal = 1,
        External = 2
    }

    public class TransactionsFilter : PagingFilter
    {
        public Guid? InvestmentProgramId { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public WalletTxType? Type { get; set; }
    }
}
