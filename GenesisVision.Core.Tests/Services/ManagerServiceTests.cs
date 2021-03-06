﻿using GenesisVision.Common.Services.Interfaces;
using GenesisVision.Core.Services;
using GenesisVision.Core.Services.Interfaces;
using GenesisVision.Core.ViewModels.Manager;
using GenesisVision.DataModel;
using GenesisVision.DataModel.Enums;
using GenesisVision.DataModel.Models;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using GenesisVision.Common.Models;

namespace GenesisVision.Core.Tests.Services
{
    [TestFixture]
    public class ManagerServiceTests
    {
        private ApplicationDbContext context;

        private IManagerService managerService;
        private Mock<IRateService> rateService;

        [SetUp]
        public void Init()
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseInMemoryDatabase("databaseManagerService");
            context = new ApplicationDbContext(optionsBuilder.Options);

            rateService = new Mock<IRateService>();

            managerService = new ManagerService(context, rateService.Object);
        }


        [Test]
        public void CreateInvestmentProgramRequest()
        {
            rateService.Setup(x => x.GetRate(It.IsAny<Currency>(), It.IsAny<Currency>()))
                       .Returns(() => InvokeOperations.InvokeOperation(() => 1m));

            var user = new ApplicationUser
                       {
                           Id = Guid.NewGuid(),
                           Wallets = new List<Wallets> {new Wallets {Amount = 1000, Currency = Currency.GVT}},
                       };
            var server = new BrokerTradeServers {Id = Guid.NewGuid()};
            context.Add(user);
            context.Add(server);
            context.SaveChanges();

            var createInvestment = new NewInvestmentRequest
                                   {
                                       Description = "#1 New investment program",
                                       FeeManagement = 456,
                                       FeeSuccess = 789,
                                       Period = 5,
                                       UserId = user.Id,
                                       DateFrom = DateTime.UtcNow.AddDays(20).AddHours(15).AddMinutes(10).AddSeconds(5),
                                       DateTo = DateTime.UtcNow.AddDays(100),
                                       InvestMinAmount = 0.9999m,
                                       InvestMaxAmount = 100000.01m,
                                       BrokerTradeServerId = server.Id,
                                       Logo = "logo.jpg",
                                       DepositAmount = 200m,
                                       TokenSymbol = "GVT_TST",
                                       TokenName = "Test Token",
                                       TradePlatformPassword = "testpwd"
                                   };
            var result = managerService.CreateNewInvestmentRequest(createInvestment);
            Assert.IsTrue(result.IsSuccess);
            rateService.Verify(x => x.GetRate(It.IsAny<Currency>(), It.IsAny<Currency>()));

            var investment = context.ManagerRequests
                                    .FirstOrDefault(x => x.Id == result.Data);
            Assert.IsNotNull(investment);
            Assert.AreEqual(createInvestment.Description, investment.Description);
            Assert.AreEqual(createInvestment.DateFrom, investment.DateFrom);
            Assert.AreEqual(createInvestment.DateTo, investment.DateTo);
            Assert.AreEqual(createInvestment.FeeSuccess, investment.FeeSuccess);
            Assert.AreEqual(createInvestment.FeeManagement, investment.FeeManagement);
            Assert.AreEqual(createInvestment.Period, investment.Period);
            Assert.AreEqual(createInvestment.InvestMinAmount, investment.InvestMinAmount);
            Assert.AreEqual(createInvestment.InvestMaxAmount, investment.InvestMaxAmount);
            Assert.AreEqual(createInvestment.Logo, investment.Logo);
            Assert.AreEqual(createInvestment.DepositAmount, investment.DepositAmount);
            Assert.AreEqual(createInvestment.TokenSymbol, investment.TokenSymbol);
            Assert.AreEqual(createInvestment.TokenName, investment.TokenName);
            Assert.AreEqual(createInvestment.TradePlatformPassword, investment.TradePlatformPassword);

            var wallet = context.Wallets.FirstOrDefault(x => x.UserId == user.Id && x.Currency == Currency.GVT);
            Assert.IsNotNull(wallet);
            Assert.AreEqual(800m, wallet.Amount);

            var tx = context.WalletTransactions.FirstOrDefault(x => x.WalletId == wallet.Id);
            Assert.IsNotNull(tx);
            Assert.AreEqual(200m, tx.Amount);
            Assert.AreEqual(WalletTransactionsType.OpenProgram, tx.Type);
        }
    }
}
