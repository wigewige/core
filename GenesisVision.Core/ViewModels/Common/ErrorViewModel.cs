using System.Collections.Generic;
using GenesisVision.Core.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace GenesisVision.Core.ViewModels.Common
{
    public class ErrorViewModel
    {
        public IEnumerable<ErrorMessage> Errors { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public ErrorCodes Code { get; set; }
    }
}
