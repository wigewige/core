using GenesisVision.Core.ViewModels.Common;
using GenesisVision.DataModel.Enums;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.ComponentModel.DataAnnotations;

namespace GenesisVision.Core.ViewModels.Investment
{
    public class InvestmentProgramRequestsFilter : PagingFilter
    {
        [Required]
        public Guid InvestmentProgramId { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public InvestmentRequestStatus? Status { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public InvestmentRequestType? Type { get; set; }
    }
}
