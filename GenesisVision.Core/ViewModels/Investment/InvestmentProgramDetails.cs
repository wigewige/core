﻿using System;
using System.Collections.Generic;
using GenesisVision.Core.ViewModels.Account;
using GenesisVision.DataModel.Enums;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace GenesisVision.Core.ViewModels.Investment
{
    public class InvestmentProgramDetails
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int Level { get; set; }
        public string Login { get; set; }
        public string Logo { get; set; }
        public decimal Balance { get; set; }
        public decimal OwnBalance { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public Currency Currency { get; set; }
        public decimal InvestedTokens { get; set; }
        public int TradesCount { get; set; }
        public int InvestorsCount { get; set; }
        public int PeriodDuration { get; set; }
        public DateTime EndOfPeriod { get; set; }
        public decimal ProfitAvg { get; set; }
        public decimal ProfitTotal { get; set; }
        public decimal AvailableInvestment { get; set; }
        public decimal FeeSuccess { get; set; }
        public decimal FeeManagement { get; set; }
        public string IpfsHash { get; set; }
        public string TradeIpfsHash { get; set; }
        public bool IsEnabled { get; set; }
        public List<Chart> Chart { get; set; }

        public ProfilePublicViewModel Manager { get; set; }

        public bool HasNewRequests { get; set; }
        public bool IsHistoryEnable { get; set; }
        public bool IsInvestEnable { get; set; }
        public bool IsWithdrawEnable { get; set; }
        public bool IsOwnProgram { get; set; }
    }
}
