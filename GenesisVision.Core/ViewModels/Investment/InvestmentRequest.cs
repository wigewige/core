using System;
using GenesisVision.DataModel.Enums;
using GenesisVision.DataModel.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace GenesisVision.Core.ViewModels.Investment
{
    public class InvestmentRequest
    {
        public Guid Id { get; set; }
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public InvestmentRequestType Type { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public InvestmentRequestStatus Status { get; set; }
    }
}
