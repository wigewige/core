using System;

namespace GenesisVision.Core.Data.Models
{
    public enum ManagerRequestType
    {
        FromCabinet = 0,
        FromBroker = 1,
    }

    public class ManagerAccountRequests
    {
        public Guid Id { get; set; }
        public DateTime Date { get; set; }
        public ManagerRequestType Type { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }

        public AspNetUsers User { get; set; }
        public Guid UserId { get; set; }

        public BrokerTradeServers BrokerTradeServers { get; set; }
        public Guid BrokerTradeServersId { get; set; }
    }
}
