using System.Collections.Generic;
using GenesisVision.Core.Models;

namespace GenesisVision.Core.ViewModels.Other
{
    public class ErrorViewModel
    {
        public IEnumerable<ErrorMessage> Errors { get; set; }

        public ErrorCodes Code { get; set; }
    }
}
