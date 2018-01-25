using System;
using GenesisVision.DataModel.Enums;
using GenesisVision.DataModel.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace GenesisVision.Core.ViewModels.Broker
{
    public class BrokerTradeServer
    {
        public Guid Id { get; set; }
        public Guid BrokerId { get; set; }
        public string Name { get; set; }
        public string Host { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public BrokerTradeServerType Type { get; set; }
        public Broker Broker { get; set; }
        public DateTime RegistrationDate { get; set; }
    }
}
