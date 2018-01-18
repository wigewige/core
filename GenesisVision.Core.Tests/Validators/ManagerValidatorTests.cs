using GenesisVision.Core.Services.Validators;
using GenesisVision.Core.Services.Validators.Interfaces;
using GenesisVision.Core.ViewModels.Investment;
using GenesisVision.Core.ViewModels.Manager;
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
    public class ManagerValidatorTests
    {
        private IManagerValidator managerValidator;

        private ApplicationDbContext context;

        private ApplicationUser applicationUser;
        private BrokersAccounts broker;
        private BrokerTradeServers brokerTradeServer;
        private ManagerAccounts managerAccountWithProgram;
        private ManagerAccounts managerAccount;
        private InvestmentPrograms investmentPrograms;
        
        [SetUp]
        public void Init()
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseInMemoryDatabase("databaseManagerValidator");
            context = new ApplicationDbContext(optionsBuilder.Options);

            applicationUser = new ApplicationUser
                              {
                                  Id = Guid.NewGuid(),
                                  IsEnabled = true,
                                  Type = UserType.Manager
                              };
            broker = new BrokersAccounts
                     {
                         Id = Guid.NewGuid(),
                         UserId = applicationUser.Id,
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
            managerAccount = new ManagerAccounts
                             {
                                 Id = Guid.NewGuid(),
                                 BrokerTradeServerId = brokerTradeServer.Id,
                                 Description = string.Empty,
                                 IsEnabled = true,
                                 Name = "Manager",
                                 Avatar = string.Empty,
                                 Currency = "USD",
                                 Login = "111111",
                                 Rating = 0m,
                                 RegistrationDate = DateTime.Now,
                                 UserId = applicationUser.Id
            };
            managerAccountWithProgram = new ManagerAccounts
                                        {
                                            Id = Guid.NewGuid(),
                                            BrokerTradeServerId = brokerTradeServer.Id,
                                            Description = string.Empty,
                                            IsEnabled = true,
                                            Name = "Manager",
                                            Avatar = string.Empty,
                                            Currency = "USD",
                                            Login = "111111",
                                            Rating = 0m,
                                            RegistrationDate = DateTime.Now,
                                            UserId = applicationUser.Id
            };
            investmentPrograms = new InvestmentPrograms
                                 {
                                     Id = Guid.NewGuid(),
                                     ManagersAccountId = managerAccountWithProgram.Id,
                                     DateFrom = DateTime.Now.AddDays(-10),
                                     DateTo = DateTime.Now.AddDays(10),
                                     FeeEntrance = 100m,
                                     FeeSuccess = 120m,
                                     FeeManagement = 10m,
                                     Description = "Test inv",
                                     IsEnabled = true,
                                     Period = 35,
                                     InvestMinAmount = 500,
                                     InvestMaxAmount = 1500
                                 };
            
            context.Add(applicationUser);
            context.Add(broker);
            context.Add(brokerTradeServer);
            context.Add(managerAccountWithProgram);
            context.Add(managerAccount);
            context.Add(investmentPrograms);
            context.SaveChanges();

            managerValidator = new ManagerValidator(context);
        }

        [Test]
        public void ValidateNewManagerAccountRequestCheckTradeServer()
        {
            const string errorMsg = "Does not find trade server";

            var res1 = managerValidator.ValidateNewInvestmentRequest(applicationUser,
                new NewInvestmentRequest {BrokerTradeServerId = Guid.NewGuid()});
            Assert.IsTrue(res1.Any(x => x.Contains(errorMsg)));

            var res2 = managerValidator.ValidateNewInvestmentRequest(applicationUser,
                new NewInvestmentRequest {BrokerTradeServerId = brokerTradeServer.Id});
            Assert.IsTrue(!res2.Any(x => x.Contains(errorMsg)));
        }

        [Test]
        public void ValidateNewManagerAccountRequestCheckUser()
        {
            const string errorMsg = "Does not find user";

            var res1 = managerValidator.ValidateNewInvestmentRequest(applicationUser,
                new NewInvestmentRequest
                {
                    BrokerTradeServerId = brokerTradeServer.Id,
                    UserId = applicationUser.Id
                });
            Assert.IsTrue(res1.All(x => x != errorMsg));

            var res2 = managerValidator.ValidateNewInvestmentRequest(applicationUser,
                new NewInvestmentRequest
                {
                    BrokerTradeServerId = brokerTradeServer.Id,
                    UserId = Guid.NewGuid()
                });
            Assert.IsTrue(res2.Any(x => x == errorMsg));

            var res3 = managerValidator.ValidateNewInvestmentRequest(applicationUser,
                new NewInvestmentRequest
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

            var res1 = managerValidator.ValidateNewInvestmentRequest(applicationUser,
                new NewInvestmentRequest
                {
                    BrokerTradeServerId = brokerTradeServer.Id,
                    UserId = applicationUser.Id,
                    Name = "Manager #1"
                });
            Assert.IsTrue(res1.All(x => x != errorMsg));

            var res2 = managerValidator.ValidateNewInvestmentRequest(applicationUser,
                new NewInvestmentRequest
                {
                    BrokerTradeServerId = brokerTradeServer.Id,
                    UserId = applicationUser.Id,
                    Name = ""
                });
            Assert.IsTrue(res2.Any(x => x == errorMsg));
        }

        [Test]
        public void ValidateInvestSuccess()
        {
            var createInv = new CreateInvestment
                            {
                                ManagersAccountId = managerAccount.Id,
                                Description = "Test_test",
                                DateFrom = DateTime.Now.AddDays(1),
                                DateTo = DateTime.Now.AddDays(36),
                                InvestMaxAmount = 99999,
                                InvestMinAmount = 100,
                                FeeSuccess = 10,
                                FeeManagement = 20,
                                FeeEntrance = 30,
                                Period = 35
                            };

            var result = managerValidator.ValidateCreateInvestmentProgram(applicationUser, createInv);
            Assert.IsEmpty(result);
        }

        [Test]
        public void ValidateInvestWrongInvestments()
        {
            var createInv = new CreateInvestment {ManagersAccountId = Guid.NewGuid()};
            var result = managerValidator.ValidateCreateInvestmentProgram(applicationUser, createInv);
            Assert.IsTrue(result.Any(x => x.Contains("Does not find manager account")));
        }

        [Test]
        public void ValidateInvestAlreadyExist()
        {
            var createInv = new CreateInvestment {ManagersAccountId = managerAccountWithProgram.Id};
            var result = managerValidator.ValidateCreateInvestmentProgram(applicationUser, createInv);
            Assert.IsTrue(result.Any(x => x.Contains("Manager has active investment program")));
        }

        [Test]
        public void ValidateInvestWrongDates()
        {
            var createInv = new CreateInvestment
                            {
                                ManagersAccountId = managerAccount.Id,
                                Description = "Test_test",
                                DateFrom = DateTime.Now.AddDays(1),
                                DateTo = DateTime.Now.AddDays(10),
                                InvestMaxAmount = 99999,
                                InvestMinAmount = 100,
                                FeeSuccess = 10,
                                FeeManagement = 20,
                                FeeEntrance = 30,
                                Period = 35
                            };

            createInv.DateFrom = createInv.DateTo = DateTime.Now.Date;
            var result1 = managerValidator.ValidateCreateInvestmentProgram(applicationUser, createInv);
            Assert.True(result1.Any(x => x == "DateFrom must be greater than today"));
            Assert.True(result1.Any(x => x == "DateTo must be greater DateFrom"));
            
            createInv.DateFrom = DateTime.Now.AddDays(10);
            createInv.DateTo = DateTime.Now.Date;
            var result2 = managerValidator.ValidateCreateInvestmentProgram(applicationUser, createInv);
            Assert.IsTrue(result2.Any(x => x == "DateTo must be greater DateFrom"));
            
            createInv.DateFrom = createInv.DateTo = DateTime.Now.AddDays(10);
            var result3 = managerValidator.ValidateCreateInvestmentProgram(applicationUser, createInv);
            Assert.IsTrue(result3.Any(x => x == "DateTo must be greater DateFrom"));

            createInv.DateFrom = DateTime.Now.Date.AddDays(10);
            createInv.DateTo = DateTime.Now.Date.AddDays(10).AddHours(1);
            var result4 = managerValidator.ValidateCreateInvestmentProgram(applicationUser, createInv);
            Assert.True(result4.Any(x => x == "Minimum duration is 1 day"));
        }

        [Test]
        public void ValidateInvestWrongPeriod()
        {
            var createInv = new CreateInvestment
                            {
                                ManagersAccountId = managerAccount.Id,
                                Period = -1
                            };
            var result = managerValidator.ValidateCreateInvestmentProgram(applicationUser, createInv);
            Assert.IsTrue(result.Any(x => x.Contains("Period must be greater than zero")));
        }

        [Test]
        public void ValidateInvestWrongFee()
        {
            var createInv = new CreateInvestment
                            {
                                ManagersAccountId = managerAccount.Id,
                                Period = 10,
                                FeeEntrance = -1,
                                FeeSuccess = -10,
                                FeeManagement = -20
                            };
            var result = managerValidator.ValidateCreateInvestmentProgram(applicationUser, createInv);
            Assert.IsTrue(result.Any(x => x.Contains("FeeEntrance must be greater or equal zero")));
            Assert.IsTrue(result.Any(x => x.Contains("FeeSuccess must be greater or equal zero")));
            Assert.IsTrue(result.Any(x => x.Contains("FeeManagement must be greater or equal zero")));
        }

        [Test]
        public void ValidateGetManagerDetails()
        {
            var result1 = managerValidator.ValidateGetManagerDetails(applicationUser, managerAccount.Id);
            Assert.IsEmpty(result1);
            
            var result2 = managerValidator.ValidateGetManagerDetails(applicationUser, Guid.NewGuid());
            Assert.IsNotEmpty(result2);
        }

        [Test]
        public void ValidateCreateManagerAccount()
        {
            var res1 = managerValidator.ValidateCreateManagerAccount(applicationUser, new NewManager {Login = "xxxxx", RequestId = Guid.NewGuid()});
            Assert.IsTrue(res1.Any(x => x.Contains("Does not find request")));

            var requestId = Guid.NewGuid();
            context.Add(new ManagerRequests {Id = requestId, UserId = applicationUser.Id, Status = ManagerRequestStatus.Declined, BrokerTradeServerId = brokerTradeServer.Id});
            context.SaveChanges();

            var res2 = managerValidator.ValidateCreateManagerAccount(applicationUser, new NewManager {Login = "xxxxx", RequestId = requestId});
            Assert.IsTrue(res2.Any(x => x.Contains("Could not proccess request")));

            requestId = Guid.NewGuid();
            context.Add(new ManagerRequests {Id = requestId, UserId = applicationUser.Id, Status = ManagerRequestStatus.Processed, BrokerTradeServerId = brokerTradeServer.Id});
            context.SaveChanges();

            var res3 = managerValidator.ValidateCreateManagerAccount(applicationUser, new NewManager {Login = "xxxxx", RequestId = requestId});
            Assert.IsTrue(res3.Any(x => x.Contains("Could not proccess request")));

            requestId = Guid.NewGuid();
            context.Add(new ManagerRequests {Id = requestId, UserId = applicationUser.Id, Status = ManagerRequestStatus.Created, BrokerTradeServerId = brokerTradeServer.Id});
            context.SaveChanges();

            var res4 = managerValidator.ValidateCreateManagerAccount(applicationUser, new NewManager {Login = "xxxxx", RequestId = requestId});
            Assert.IsEmpty(res4);
        }
    }
}
