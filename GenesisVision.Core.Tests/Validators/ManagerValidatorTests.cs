﻿using GenesisVision.Core.Helpers;
using GenesisVision.Core.Services.Validators;
using GenesisVision.Core.Services.Validators.Interfaces;
using GenesisVision.Core.ViewModels.Manager;
using GenesisVision.DataModel;
using GenesisVision.DataModel.Enums;
using GenesisVision.DataModel.Models;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System;
using System.Collections.Generic;
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
        private ManagerAccounts managerAccount;
        
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
                                  Type = UserType.Manager,
                                  Wallets = new List<Wallets> {new Wallets {Amount = 10000, Currency = Currency.GVT}},
                              };
            broker = new BrokersAccounts
                     {
                         Id = Guid.NewGuid(),
                         UserId = applicationUser.Id,
                         Description = string.Empty,
                         IsEnabled = true,
                         Name = "Broker #1",
                         RegistrationDate = DateTime.UtcNow
                     };
            brokerTradeServer = new BrokerTradeServers
                                {
                                    Id = Guid.NewGuid(),
                                    Name = "Server #1",
                                    IsEnabled = true,
                                    Host = string.Empty,
                                    RegistrationDate = DateTime.UtcNow,
                                    Type = BrokerTradeServerType.MetaTrader4,
                                    BrokerId = broker.Id
                                };
            managerAccount = new ManagerAccounts
                             {
                                 Id = Guid.NewGuid(),
                                 BrokerTradeServerId = brokerTradeServer.Id,
                                 Currency = Currency.USD,
                                 Login = "111111",
                                 RegistrationDate = DateTime.UtcNow,
                                 UserId = applicationUser.Id,
                                 IsConfirmed = true
                             };
            
            context.Add(applicationUser);
            context.Add(broker);
            context.Add(brokerTradeServer);
            context.Add(managerAccount);
            context.SaveChanges();

            managerValidator = new ManagerValidator(context);
        }

        [Test]
        public void ValidateNewManagerAccountRequestCheckTradeServer()
        {
            const string errorMsg = "Does not find trade server";

            var res1 = managerValidator.ValidateNewInvestmentRequest(applicationUser,
                new NewInvestmentRequest {UserId = applicationUser.Id, BrokerTradeServerId = Guid.NewGuid()});
            Assert.IsTrue(res1.Any(x => x.Contains(errorMsg)));

            var res2 = managerValidator.ValidateNewInvestmentRequest(applicationUser,
                new NewInvestmentRequest {UserId = applicationUser.Id, BrokerTradeServerId = brokerTradeServer.Id});
            Assert.IsTrue(!res2.Any(x => x.Contains(errorMsg)));
        }

        [Test]
        public void ValidateInvestSuccess()
        {
            var createInv = new NewInvestmentRequest
                            {
                                UserId = applicationUser.Id,
                                Description = "Test_test",
                                DateFrom = DateTime.UtcNow.AddDays(1),
                                DateTo = DateTime.UtcNow.AddDays(36),
                                InvestMaxAmount = 99999,
                                InvestMinAmount = 100,
                                FeeSuccess = 10,
                                FeeManagement = 20,
                                Period = 35,
                                BrokerTradeServerId = brokerTradeServer.Id,
                                Logo = "logo.jpg",
                                DepositAmount = 200,
                                TokenSymbol = "GVT_TST",
                                TokenName = "Test symbol",
                                TradePlatformPassword = "testpwd"
                            };

            var result = managerValidator.ValidateNewInvestmentRequest(applicationUser, createInv);
            Assert.IsEmpty(result);
        }

        [Test]
        public void ValidateInvestWrongInvestments()
        {
            var createInv = new NewInvestmentRequest {UserId = Guid.NewGuid()};
            var result = managerValidator.ValidateNewInvestmentRequest(applicationUser, createInv);
            Assert.IsTrue(result.Any(x => x.Contains(ValidationMessages.AccessDenied)));
        }

        [Test]
        public void ValidateInvestWrongDates()
        {
            var createInv = new NewInvestmentRequest
                            {
                                UserId = applicationUser.Id,
                                Description = "Test_test",
                                DateFrom = DateTime.UtcNow.AddDays(1),
                                DateTo = DateTime.UtcNow.AddDays(10),
                                InvestMaxAmount = 99999,
                                InvestMinAmount = 100,
                                FeeSuccess = 10,
                                FeeManagement = 20,
                                Period = 35
                            };

            createInv.DateFrom = createInv.DateTo = DateTime.UtcNow.Date;
            var result1 = managerValidator.ValidateNewInvestmentRequest(applicationUser, createInv);
            Assert.True(result1.Any(x => x == "DateFrom must be greater than today"));
            Assert.True(result1.Any(x => x == "DateTo must be greater DateFrom"));

            createInv.DateFrom = DateTime.UtcNow.AddDays(10);
            createInv.DateTo = DateTime.UtcNow.Date;
            var result2 = managerValidator.ValidateNewInvestmentRequest(applicationUser, createInv);
            Assert.IsTrue(result2.Any(x => x == "DateTo must be greater DateFrom"));

            createInv.DateFrom = createInv.DateTo = DateTime.UtcNow.AddDays(10);
            var result3 = managerValidator.ValidateNewInvestmentRequest(applicationUser, createInv);
            Assert.IsTrue(result3.Any(x => x == "DateTo must be greater DateFrom"));

            createInv.DateFrom = DateTime.UtcNow.Date.AddDays(10);
            createInv.DateTo = DateTime.UtcNow.Date.AddDays(10).AddHours(1);
            var result4 = managerValidator.ValidateNewInvestmentRequest(applicationUser, createInv);
            Assert.True(result4.Any(x => x == "Minimum duration is 1 day"));
        }

        [Test]
        public void ValidateInvestWrongPeriod()
        {
            var createInv = new NewInvestmentRequest
                            {
                                UserId = applicationUser.Id,
                                Period = -1
                            };
            var result = managerValidator.ValidateNewInvestmentRequest(applicationUser, createInv);
            Assert.IsTrue(result.Any(x => x.Contains("Period must be greater than zero")));
        }

        [Test]
        public void ValidateInvestWrongFee()
        {
            var createInv = new NewInvestmentRequest
                            {
                                UserId = applicationUser.Id,
                                Period = 10,
                                FeeSuccess = -10,
                                FeeManagement = -20,
                                BrokerTradeServerId = brokerTradeServer.Id
                            };
            var result = managerValidator.ValidateNewInvestmentRequest(applicationUser, createInv);
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
    }
}
