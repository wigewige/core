﻿using System;
using System.Collections.Generic;

namespace GenesisVision.DataModel.Models
{
    public class InvestorAccounts
    {
        public Guid UserId { get; set; }
        public ApplicationUser User { get; set; }

        public ICollection<InvestorTokens> InvestorTokens { get; set; }

        public ICollection<InvestmentRequests> InvestmentRequests { get; set; }
    }
}
