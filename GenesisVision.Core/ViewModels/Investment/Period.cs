using System;
using System.Collections.Generic;
using GenesisVision.DataModel.Enums;
using GenesisVision.DataModel.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace GenesisVision.Core.ViewModels.Investment
{
    public class Period
    {
        public Guid Id { get; set; }
        public int Number { get; set; }
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public PeriodStatus Status { get; set; }
        public decimal StartBalance { get; set; }
        public decimal ManagerStartBalance { get; set; }
        public decimal ManagerStartShare { get; set; }
        public List<InvestmentProgramRequest> InvestmentRequest { get; set; }
    }
}
