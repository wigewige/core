using GenesisVision.Core.Data.Enums;
using System;

namespace GenesisVision.Core.Data.Models
{
    public enum IOTransactionType
    {
        Undefined = 0,
        Input = 1,
        Output = 2
    }

    public class IOTransactions
    {
        public Guid Id { get; set; }
        string Currency { get; set; }
        IOTransactionType Type { get; set; }
        IOTransactionStatus Status { get; set; }

        public AspNetUsers User { get; set; }
        public Guid UserId { get; set; }
    }
}
