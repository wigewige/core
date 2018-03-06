using GenesisVision.DataModel.Enums;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;

namespace GenesisVision.Core.ViewModels.Wallet
{
    public class WalletTransaction
    {
        public Guid Id { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public WalletTransactionsType Type { get; set; }

        public decimal Amount { get; set; }

        public DateTime Date { get; set; }

        public Guid WalletId { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public Currency Currency { get; set; }
        
        public InvestmentProgramTxInfo InvestmentProgram { get; set; }
        public InvestmentProgramRequestTxInfo InvestmentProgramRequest { get; set; }
        public PaymentTxInfo PaymentTx { get; set; }
    }

    public class InvestmentProgramRequestTxInfo
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public InvestmentRequestType Type { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public InvestmentRequestStatus Status { get; set; }
    }

    public class InvestmentProgramTxInfo
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
    }

    public class PaymentTxInfo
    {
        public Guid Id { get; set; }
        public string Hash { get; set; }
        public string Address { get; set; }
    }
}
