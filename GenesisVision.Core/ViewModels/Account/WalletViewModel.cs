using GenesisVision.DataModel.Enums;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;

namespace GenesisVision.Core.ViewModels.Account
{
    public class WalletViewModel
    {
        public Guid Id { get; set; }

        public decimal Amount { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public WalletCurrency Currency { get; set; }
    }
}
