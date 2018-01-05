using System;
using GenesisVision.DataModel.Enums;

namespace GenesisVision.DataModel.Models
{
    public class ManagerAccountRequests
    {
        public Guid Id { get; set; }
        public DateTime Date { get; set; }
        public ManagerRequestType Type { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Avatar { get; set; }
        public string Currency { get; set; }
        public string TokenName { get; set; }
        public string TokenSymbol { get; set; }

        public ManagerRequestStatus Status { get; set; }

        public ApplicationUser User { get; set; }
        public Guid UserId { get; set; }

        public BrokerTradeServers BrokerTradeServers { get; set; }
        public Guid BrokerTradeServerId { get; set; }
    }
}
