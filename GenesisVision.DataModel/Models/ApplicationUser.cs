using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace GenesisVision.DataModel.Models
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        public ICollection<ManagerAccounts> ManagerAccounts { get; set; }

        public ICollection<InvestmentRequests> InvestmentRequests { get; set; }

        public ICollection<ManagerAccountRequests> ManagerAccountRequests { get; set; }

        public ICollection<IOTransactions> IOTransactions { get; set; }

        public InvestorAccounts InvestorAccount { get; set; }
    }
}
