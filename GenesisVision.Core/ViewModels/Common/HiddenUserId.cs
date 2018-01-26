using Newtonsoft.Json;
using System;

namespace GenesisVision.Core.ViewModels.Common
{
    public class HiddenUserId
    {
        [JsonIgnore]
        public Guid UserId { get; set; }
    }
}
