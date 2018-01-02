﻿using System;
using GenesisVision.DataModel.Enums;
using GenesisVision.DataModel.Models;

namespace GenesisVision.Core.ViewModels.Investment
{
    public class InvestmentRequest
    {
        public Guid Id { get; set; }
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
        public InvestmentRequestType Type { get; set; }
        public InvestmentRequestStatus Status { get; set; }
    }
}
