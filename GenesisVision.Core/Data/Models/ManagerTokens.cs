using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GenesisVision.Core.Data.Models
{
    public class ManagerTokens
    {
        public Guid Id { get; set; }

        public string TokenAddress { get; set; }

        public string TokenSymbol { get; set; }
    }
}
