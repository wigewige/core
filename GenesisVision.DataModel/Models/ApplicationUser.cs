using GenesisVision.DataModel.Enums;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace GenesisVision.DataModel.Models
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        public ICollection<ManagerAccounts> ManagerAccounts { get; set; }

        public ICollection<InvestmentRequests> InvestmentRequests { get; set; }

        public ICollection<ManagerRequests> ManagerAccountRequests { get; set; }

        public ICollection<IOTransactions> IOTransactions { get; set; }

        public InvestorAccounts InvestorAccount { get; set; }

        public BrokersAccounts BrokersAccount { get; set; }

        public bool IsEnabled { get; set; }

        public UserType Type { get; set; }

        public Profiles Profile { get; set; }

        public Wallets Wallet { get; set; }
    }
}
