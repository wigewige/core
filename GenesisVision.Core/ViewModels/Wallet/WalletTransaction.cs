using GenesisVision.DataModel.Enums;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;

namespace GenesisVision.Core.ViewModels.Wallet
{
    public class WalletTransaction
    {
        public Guid Id { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public WalletTransactionsType Type { get; set; }

        public decimal Amount { get; set; }

        public DateTime Date { get; set; }

        public Guid WalletId { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public WalletCurrency WalletCurrency { get; set; }
    }
}
