using GenesisVision.Core.Helpers;
using GenesisVision.Core.Services.Validators;
using GenesisVision.Core.Services.Validators.Interfaces;
using GenesisVision.Core.ViewModels.Broker;
using GenesisVision.DataModel;
using GenesisVision.DataModel.Enums;
using GenesisVision.DataModel.Models;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System;
using System.Linq;

namespace GenesisVision.Core.Tests.Validators
{
    [TestFixture]
    public class BrokerValidatorTests
    {
        private IBrokerValidator brokerValidator;

        private ApplicationDbContext context;

        private ApplicationUser user;
        private BrokersAccounts broker;
        private BrokerTradeServers brokerTradeServer;

        [SetUp]
        public void Init()
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseInMemoryDatabase("databaseBrokerValidator");
            context = new ApplicationDbContext(optionsBuilder.Options);

            user = new ApplicationUser
                   {
                       Id = Guid.NewGuid(),
                       IsEnabled = true,
                       Type = UserType.Broker
                   };
            broker = new BrokersAccounts
                     {
                         Id = Guid.NewGuid(),
                         UserId = user.Id,
                         Description = string.Empty,
                         IsEnabled = true,
                         Name = "Broker #1",
                         Logo = "logo.png",
                         RegistrationDate = DateTime.Now
                     };
            brokerTradeServer = new BrokerTradeServers
                                {
                                    Id = Guid.NewGuid(),
                                    Name = "Server #1",
                                    IsEnabled = true,
                                    Host = string.Empty,
                                    RegistrationDate = DateTime.Now,
                                    Type = BrokerTradeServerType.MetaTrader4,
                                    BrokerId = broker.Id
                                };
            context.Add(broker);
            context.Add(brokerTradeServer);
            context.SaveChanges();


            brokerValidator = new BrokerValidator(context);
        }

        [Test]
        public void ValidateGetBrokerInitDataServerExists()
        {
            var res1 = brokerValidator.ValidateGetBrokerInitData(user, brokerTradeServer.Id);
            Assert.IsEmpty(res1);

            var res2 = brokerValidator.ValidateGetBrokerInitData(user, Guid.NewGuid());
            Assert.IsTrue(res2.Any(x => x.Contains("does not exist")));
        }

        [Test]
        public void ValidateGetBrokerInitDataServerEnable()
        {
            context.BrokersAccounts.First(x => x.Id == broker.Id).IsEnabled = false;
            context.SaveChanges();

            var res1 = brokerValidator.ValidateGetBrokerInitData(user, brokerTradeServer.Id);
            Assert.IsTrue(res1.Any(x => x.Contains(ValidationMessages.AccessDenied)));

            context.BrokersAccounts.First(x => x.Id == broker.Id).IsEnabled = true;
            context.BrokerTradeServers.First(x => x.Id == brokerTradeServer.Id).IsEnabled = false;
            context.SaveChanges();

            var res2 = brokerValidator.ValidateGetBrokerInitData(user, brokerTradeServer.Id);
            Assert.IsTrue(res2.Any(x => x.Contains(ValidationMessages.AccessDenied)));
        }

        [Test]
        public void ValidateCreateManagerAccount()
        {
            var res1 = brokerValidator.ValidateCreateManagerAccount(user, new NewManager {Login = "xxxxx", RequestId = Guid.NewGuid()});
            Assert.IsTrue(res1.Any(x => x.Contains("Does not find request")));

            var requestId = Guid.NewGuid();
            context.Add(new ManagerRequests {Id = requestId, UserId = user.Id, Status = ManagerRequestStatus.Declined, BrokerTradeServerId = brokerTradeServer.Id});
            context.SaveChanges();

            var res2 = brokerValidator.ValidateCreateManagerAccount(user, new NewManager {Login = "xxxxx", RequestId = requestId});
            Assert.IsTrue(res2.Any(x => x.Contains("Could not proccess request")));

            requestId = Guid.NewGuid();
            context.Add(new ManagerRequests {Id = requestId, UserId = user.Id, Status = ManagerRequestStatus.Processed, BrokerTradeServerId = brokerTradeServer.Id});
            context.SaveChanges();

            var res3 = brokerValidator.ValidateCreateManagerAccount(user, new NewManager {Login = "xxxxx", RequestId = requestId});
            Assert.IsTrue(res3.Any(x => x.Contains("Could not proccess request")));

            requestId = Guid.NewGuid();
            context.Add(new ManagerRequests {Id = requestId, UserId = user.Id, Status = ManagerRequestStatus.Created, BrokerTradeServerId = brokerTradeServer.Id});
            context.SaveChanges();

            var res4 = brokerValidator.ValidateCreateManagerAccount(user, new NewManager {Login = "xxxxx", RequestId = requestId});
            Assert.IsEmpty(res4);
        }
    }
}
