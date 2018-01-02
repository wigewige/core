using System;
using System.Collections.Generic;

namespace GenesisVision.DataModel.Models
{
    public class AspNetUsers
    {
        public Guid Id { get; set; }

        public ICollection<ManagerAccounts> ManagerAccounts { get; set; }

        public ICollection<InvestmentRequests> InvestmentRequests { get; set; }

        public ICollection<ManagerAccountRequests> ManagerAccountRequests { get; set; }

        public ICollection<IOTransactions> IOTransactions { get; set; }

        public InvestorAccounts InvestorAccount { get; set; }

        public ApplicationUser ApplicationUser { get; set; }
    }
}
