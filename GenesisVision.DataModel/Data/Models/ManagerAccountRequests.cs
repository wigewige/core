using System;

namespace GenesisVision.DataModel.Models
{
    public enum ManagerRequestType
    {
        FromCabinet = 0,
        FromBroker = 1,
    }

    public enum ManagerRequestStatus
    {
        Created = 0,
        Processed = 1,
        Declined = 2
    }

    public class ManagerAccountRequests
    {
        public Guid Id { get; set; }
        public DateTime Date { get; set; }
        public ManagerRequestType Type { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Avatar { get; set; }
        public string Currency { get; set; }
        public ManagerRequestStatus Status { get; set; }

        public AspNetUsers User { get; set; }
        public Guid UserId { get; set; }

        public BrokerTradeServers BrokerTradeServers { get; set; }
        public Guid BrokerTradeServerId { get; set; }
    }
}
