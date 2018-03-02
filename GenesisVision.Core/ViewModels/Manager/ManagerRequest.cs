using GenesisVision.DataModel.Enums;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;

namespace GenesisVision.Core.ViewModels.Manager
{
    public class ManagerRequest
    {
        public Guid UserId { get; set; }
        public Guid RequestId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public Currency Currency { get; set; }
        public string Password { get; set; }
        public decimal DepositInUsd { get; set; }
    }
}
