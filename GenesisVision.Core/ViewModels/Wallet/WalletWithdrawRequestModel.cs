using GenesisVision.DataModel.Enums;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.ComponentModel.DataAnnotations;

namespace GenesisVision.Core.ViewModels.Wallet
{
    public class WalletWithdrawRequestModel
    {
        [Required]
        [JsonConverter(typeof(StringEnumConverter))]
        public WalletCurrency Currency { get; set; }

        [Required]
        public decimal Amount { get; set; }

        [Required]
        public string BlockchainAddress { get; set; }
    }
}
