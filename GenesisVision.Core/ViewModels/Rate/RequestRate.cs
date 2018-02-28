using GenesisVision.DataModel.Enums;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.ComponentModel.DataAnnotations;

namespace GenesisVision.Core.ViewModels.Rate
{
    public class RequestRate
    {
        [Required]
        [JsonConverter(typeof(StringEnumConverter))]
        public Currency From { get; set; }

        [Required]
        [JsonConverter(typeof(StringEnumConverter))]
        public Currency To { get; set; }
    }
}
