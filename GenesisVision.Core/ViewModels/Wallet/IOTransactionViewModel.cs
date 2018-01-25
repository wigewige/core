using GenesisVision.DataModel.Enums;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace GenesisVision.Core.ViewModels.Wallet
{
    public class IOTransactionViewModel
    {
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public IOTransactionType Type { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public IOTransactionStatus Status { get; set; }
    }
}
