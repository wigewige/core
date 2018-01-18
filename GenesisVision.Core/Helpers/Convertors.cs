using GenesisVision.DataModel.Models;
using GenesisVision.Core.ViewModels.Broker;
using GenesisVision.Core.ViewModels.Investment;
using GenesisVision.Core.ViewModels.Manager;
using System.Collections.Generic;
using System.Linq;
using GenesisVision.Core.ViewModels.Account;

namespace GenesisVision.Core.Helpers
{
    public static class Convertors
    {
        public static Investment ToInvestment(this InvestmentPrograms inv)
        {
            return new Investment
                   {
                       Id = inv.Id,
                       InvestMinAmount = inv.InvestMinAmount,
                       InvestMaxAmount = inv.InvestMaxAmount,
                       Description = inv.Description,
                       IsEnabled = inv.IsEnabled,
                       FeeEntrance = inv.FeeEntrance,
                       FeeSuccess = inv.FeeSuccess,
                       FeeManagement = inv.FeeManagement,
                       DateFrom = inv.DateFrom,
                       DateTo = inv.DateTo,
                       Period = inv.Period,
                       ManagerId = inv.ManagersAccountId,
                       LastPeriod = inv.Periods?.OrderByDescending(x => x.Number).FirstOrDefault()?.ToPeriod()
                   };
        }

        public static Period ToPeriod(this Periods p)
        {
            return new Period
                   {
                       Id = p.Id,
                       DateFrom = p.DateFrom,
                       DateTo = p.DateTo,
                       Status = p.Status,
                       Number = p.Number,
                       StartBalance = p.StartBalance,
                       InvestmentRequest = p.InvestmentRequests?.Select(ToInvestmentRequest).ToList() ??
                                           new List<InvestmentRequest>()
                   };
        }

        public static ManagerRequest ToManagerRequest(this ManagerRequests x)
        {
            return new ManagerRequest
                   {
                       Currency = x.Currency,
                       Description = x.Description,
                       Name = x.Name,
                       UserId = x.Id,
                       RequestId = x.Id
                   };
        }

        public static InvestmentRequest ToInvestmentRequest(this InvestmentRequests inv)
        {
            return new InvestmentRequest
                   {
                       Id = inv.Id,
                       Date = inv.Date,
                       Status = inv.Status,
                       Type = inv.Type,
                       Amount = inv.Amount
                   };
        }

        public static ManagerAccount ToManagerAccount(this ManagerAccounts manager)
        {
            return new ManagerAccount
                   {
                       Id = manager.Id,
                       Name = manager.Name,
                       Avatar = manager.Avatar,
                       Login = manager.Login,
                       Description = manager.Description,
                       Currency = manager.Currency,
                       IpfsHash = manager.IpfsHash,
                       IsEnabled = manager.IsEnabled,
                       BrokerTradeServer = manager.BrokerTradeServer?.ToBrokerTradeServers(),
                       Broker = manager.BrokerTradeServer?.Broker?.ToBroker()
                   };
        }

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

        public static ProfileShortViewModel ToProfileShort(this ApplicationUser user)
        {
            return new ProfileShortViewModel
                   {
                       Email = user.Email,
                       Balance = 0
                   };
        }

        public static ProfileFullViewModel ToProfileFull(this ApplicationUser user)
        {
            var model = new ProfileFullViewModel
                   {
                       Email = user.Email,
                       Balance = 0
                   };
            if (user.Profile != null)
            {
                model.Avatar = user.Profile.Avatar;
                model.Address = user.Profile.Address;
                model.Birthday = user.Profile.Birthday;
                model.City = user.Profile.City;
                model.Country = user.Profile.Country;
                model.DocumentNumber = user.Profile.DocumentNumber;
                model.DocumentType = user.Profile.DocumentType;
                model.FirstName = user.Profile.FirstName;
                model.Gender = user.Profile.Gender;
                model.LastName = user.Profile.LastName;
                model.MiddleName = user.Profile.MiddleName;
                model.Phone = user.Profile.Phone;
            }
            return model;
        }
    }
}
