using GenesisVision.Core.Data;
using GenesisVision.Core.Data.Models;
using GenesisVision.Core.Services.Validators;
using GenesisVision.Core.Services.Validators.Interfaces;
using GenesisVision.Core.ViewModels.Manager;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;

namespace GenesisVision.Core.Tests
{
    [TestFixture]
    public class ManagerValidatorTests
    {
        private IManagerValidator managerValidator;

        private IPrincipal user;
        private Brokers broker;
        private BrokerTradeServers brokerTradeServer;
        private AspNetUsers aspNetUser;

        [SetUp]
        public void Init()
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseInMemoryDatabase("database");
            var context = new ApplicationDbContext(optionsBuilder.Options);

            aspNetUser = new AspNetUsers
                         {
                             Id = Guid.NewGuid(),
                             AccessFailedCount = 0,
                             Email = "test@test.com",
                             EmailConfirmed = true,
                         };
            broker = new Brokers
                     {
                         Id = Guid.NewGuid(),
                         Description = string.Empty,
                         IsEnabled = true,
                         Name = "Broker #1",
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
            context.Add(aspNetUser);
            context.Add(broker);
            context.Add(brokerTradeServer);
            context.SaveChanges();

            user = new ClaimsPrincipal();

            managerValidator = new ManagerValidator(context);
        }

        [Test]
        public void ValidateNewManagerAccountRequestCheckTradeServer()
        {
            const string errorMsg = "Does not find trade server";

            var res1 = managerValidator.ValidateNewManagerAccountRequest(user,
                new NewManagerRequest {BrokerTradeServerId = Guid.NewGuid()});
            Assert.IsTrue(res1.Any(x => x.Contains(errorMsg)));

            var res2 = managerValidator.ValidateNewManagerAccountRequest(user,
                new NewManagerRequest {BrokerTradeServerId = brokerTradeServer.Id});
            Assert.IsTrue(!res2.Any(x => x.Contains(errorMsg)));
        }

        [Test]
        public void ValidateNewManagerAccountRequestCheckUser()
        {
            const string errorMsg = "Does not find user";

            var res1 = managerValidator.ValidateNewManagerAccountRequest(user,
                new NewManagerRequest
                {
                    BrokerTradeServerId = brokerTradeServer.Id,
                    UserId = aspNetUser.Id
                });
            Assert.IsTrue(res1.All(x => x != errorMsg));

            var res2 = managerValidator.ValidateNewManagerAccountRequest(user,
                new NewManagerRequest
                {
                    BrokerTradeServerId = brokerTradeServer.Id,
                    UserId = Guid.NewGuid()
                });
            Assert.IsTrue(res2.Any(x => x == errorMsg));

            var res3 = managerValidator.ValidateNewManagerAccountRequest(user,
                new NewManagerRequest
                {
                    BrokerTradeServerId = brokerTradeServer.Id,
                    UserId = null
                });
            Assert.IsTrue(res3.All(x => x != errorMsg));
        }

        [Test]
        public void ValidateNewManagerAccountRequestCheckName()
        {
            const string errorMsg = "'Name' is empty";

            var res1 = managerValidator.ValidateNewManagerAccountRequest(user,
                new NewManagerRequest
                {
                    BrokerTradeServerId = brokerTradeServer.Id,
                    UserId = aspNetUser.Id,
                    Name = "Manager #1"
                });
            Assert.IsTrue(res1.All(x => x != errorMsg));

            var res2 = managerValidator.ValidateNewManagerAccountRequest(user,
                new NewManagerRequest
                {
                    BrokerTradeServerId = brokerTradeServer.Id,
                    UserId = aspNetUser.Id,
                    Name = ""
                });
            Assert.IsTrue(res2.Any(x => x == errorMsg));
        }
    }
}
