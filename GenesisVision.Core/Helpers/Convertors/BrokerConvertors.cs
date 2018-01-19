using GenesisVision.Core.ViewModels.Broker;
using GenesisVision.DataModel.Models;

namespace GenesisVision.Core.Helpers.Convertors
{
    public static partial class Convertors
    {
        public static BrokerTradeServer ToBrokerTradeServers(this BrokerTradeServers server)
        {
            return new BrokerTradeServer
                   {
                       Id = server.Id,
                       Name = server.Name,
                       Type = server.Type,
                       Host = server.Host,
                       RegistrationDate = server.RegistrationDate,
                       BrokerId = server.BrokerId,
                       Broker = server.Broker?.ToBroker()
                   };
        }

        public static Broker ToBroker(this BrokersAccounts broker)
        {
            return new Broker
                   {
                       Id = broker.Id,
                       Description = broker.Description,
                       Name = broker.Name,
                       Logo = broker.Logo,
                       RegistrationDate = broker.RegistrationDate
                   };
        }
    }
}
