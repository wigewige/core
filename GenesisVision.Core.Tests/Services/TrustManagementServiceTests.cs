using GenesisVision.Core.Models;
using GenesisVision.Core.Services;
using GenesisVision.Core.Services.Interfaces;
using GenesisVision.Core.ViewModels.Broker;
using GenesisVision.Core.ViewModels.Investment;
using GenesisVision.DataModel;
using GenesisVision.DataModel.Enums;
using GenesisVision.DataModel.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using NSubstitute;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GenesisVision.Core.Tests.Services
{
    [TestFixture]
    public class TrustManagementServiceTests
    {
        private ITrustManagementService trustManagementService;
        private Mock<ISmartContractService> smartContractService;
        private Mock<IIpfsService> ipfsService;
        private Mock<IStatisticService> statisticService;
        private Mock<IRateService> rateService;

        private ApplicationDbContext context;

        private InvestmentPrograms investmentProgram;
        private ManagerAccounts managerAccount;
        private ApplicationUser user;
        private BrokersAccounts broker;
        private BrokerTradeServers brokerTradeServer;

        [SetUp]
        public void Init()
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseInMemoryDatabase("databaseTrustManagementService");
            context = new ApplicationDbContext(optionsBuilder.Options);

            user = new ApplicationUser
                   {
                       Id = Guid.NewGuid(),
                       IsEnabled = true,
                       Wallets = new List<Wallets> {new Wallets {Amount = 100000, Currency = Currency.GVT}},
                   };
            managerAccount = new ManagerAccounts
                             {
                                 Id = Guid.NewGuid(),
                                 IsConfirmed = true,
                                 UserId = user.Id
                             };
            investmentProgram = new InvestmentPrograms
                                {
                                    Id = Guid.NewGuid(),
                                    DateFrom = DateTime.Now.AddYears(-1),
                                    FeeEntrance = 100m,
                                    FeeSuccess = 120m,
                                    FeeManagement = 10m,
                                    Description = "Test inv",
                                    IsEnabled = true,
                                    Period = 10,
                                    InvestMinAmount = 500,
                                    InvestMaxAmount = 1500,
                                    ManagerAccountId = managerAccount.Id,
                                    Token = new ManagerTokens
                                            {
                                                TokenName = "Token",
                                                TokenSymbol = "TST"
                                            }
                                };
            broker = new BrokersAccounts
                     {
                         Id = Guid.NewGuid(),
                         UserId = user.Id,
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
            context.Add(user);
            context.Add(managerAccount);
            context.Add(investmentProgram);
            context.Add(broker);
            context.Add(brokerTradeServer);
            context.SaveChanges();

            smartContractService = new Mock<ISmartContractService>();
            ipfsService = new Mock<IIpfsService>();
            statisticService = new Mock<IStatisticService>();
            rateService = new Mock<IRateService>();

            trustManagementService = new TrustManagementService(context,
                ipfsService.Object,
                smartContractService.Object,
                statisticService.Object,
                rateService.Object,
                Substitute.For<ILogger<ITrustManagementService>>());
        }

        [Test]
        public void GetClosingPeriodDataCannotClose()
        {
            var period1 = new Periods
                          {
                              InvestmentProgramId = investmentProgram.Id,
                              Id = Guid.NewGuid(),
                              DateFrom = DateTime.Now.AddDays(-1),
                              DateTo = DateTime.Now.AddDays(9),
                              Number = 1,
                              Status = PeriodStatus.InProccess
                          };
            var period2 = new Periods
                          {
                              InvestmentProgramId = investmentProgram.Id,
                              Id = Guid.NewGuid(),
                              DateFrom = DateTime.Now.AddDays(9),
                              DateTo = DateTime.Now.AddDays(19),
                              Number = 2,
                              Status = PeriodStatus.Planned
                          };
            context.AddRange(new[] {period1, period2});
            context.SaveChanges();

            var result = trustManagementService.GetClosingPeriodData(investmentProgram.Id);

            Assert.IsTrue(result.IsSuccess);
            Assert.IsFalse(result.Data.CanCloseCurrentPeriod);
            Assert.IsNotNull(result.Data.CurrentPeriod);
            Assert.AreEqual(result.Data.CurrentPeriod.Id, context.Periods.First(x => x.Number == 1 && x.InvestmentProgramId == investmentProgram.Id).Id);
            //Assert.IsNotNull(result.Data.NextPeriod);
            //Assert.AreEqual(result.Data.NextPeriod.Id, context.Periods.First(x => x.Number == 2 && x.InvestmentProgramId == investmentProgram.Id).Id);

            context.Periods.RemoveRange(context.Periods.Where(x => x.Id == period1.Id || x.Id == period2.Id));
            context.SaveChanges();
        }

        [Test]
        public void GetClosingPeriodDataCanClose()
        {
            var period1 = new Periods
                          {
                              InvestmentProgramId = investmentProgram.Id,
                              Id = Guid.NewGuid(),
                              DateFrom = DateTime.Now.AddDays(-15),
                              DateTo = DateTime.Now.AddDays(-5),
                              Number = 1,
                              Status = PeriodStatus.InProccess
                          };
            var period2 = new Periods
                          {
                              InvestmentProgramId = investmentProgram.Id,
                              Id = Guid.NewGuid(),
                              DateFrom = DateTime.Now.AddDays(-5),
                              DateTo = DateTime.Now.AddDays(5),
                              Number = 2,
                              Status = PeriodStatus.Planned
                          };
            context.AddRange(new[] {period1, period2});
            context.SaveChanges();

            var result = trustManagementService.GetClosingPeriodData(investmentProgram.Id);

            Assert.IsTrue(result.IsSuccess);
            Assert.IsTrue(result.Data.CanCloseCurrentPeriod);
            Assert.IsNotNull(result.Data.CurrentPeriod);
            Assert.AreEqual(result.Data.CurrentPeriod.Id, context.Periods.First(x => x.Number == 1 && x.InvestmentProgramId == investmentProgram.Id).Id);
            //Assert.IsNotNull(result.Data.NextPeriod);
            //Assert.AreEqual(result.Data.NextPeriod.Id, context.Periods.First(x => x.Number == 2 && x.InvestmentProgramId == investmentProgram.Id).Id);

            context.Periods.RemoveRange(context.Periods.Where(x => x.Id == period1.Id || x.Id == period2.Id));
            context.SaveChanges();
        }

        [Test]
        public void GetClosingPeriodDataPeriods()
        {
            var period1 = new Periods
                          {
                              InvestmentProgramId = investmentProgram.Id,
                              Id = Guid.NewGuid(),
                              Number = 1,
                              Status = PeriodStatus.Closed
                          };
            var period2 = new Periods
                          {
                              InvestmentProgramId = investmentProgram.Id,
                              Id = Guid.NewGuid(),
                              Number = 2,
                              Status = PeriodStatus.InProccess
                          };
            context.AddRange(new[] {period1, period2});
            context.SaveChanges();

            var result = trustManagementService.GetClosingPeriodData(investmentProgram.Id);

            Assert.IsTrue(result.IsSuccess);
            Assert.IsNotNull(result.Data.CurrentPeriod);
            Assert.AreEqual(result.Data.CurrentPeriod.Id, context.Periods.First(x => x.Number == 2 && x.InvestmentProgramId == investmentProgram.Id).Id);
            //Assert.IsNull(result.Data.NextPeriod);

            context.Periods.RemoveRange(context.Periods.Where(x => x.Id == period1.Id || x.Id == period2.Id));
            context.SaveChanges();
        }

        [Test]
        public void CreateInvestmentProgram()
        {
            var createInvestment = new ManagerRequests
                                   {
                                       Id = Guid.NewGuid(),
                                       UserId = user.Id,
                                       TokenName = "Token123",
                                       TokenSymbol = "T123",
                                       Description = "#1 New investment program",
                                       FeeEntrance = 123,
                                       FeeManagement = 456,
                                       FeeSuccess = 789,
                                       Period = 5,
                                       DateFrom = DateTime.Now.AddDays(20).AddHours(15).AddMinutes(10).AddSeconds(5),
                                       DateTo = DateTime.Now.AddDays(100),
                                       InvestMinAmount = 0.9999m,
                                       InvestMaxAmount = 100000.01m,
                                       Status = ManagerRequestStatus.Created,
                                       Type = ManagerRequestType.FromCabinet,
                                       Date = DateTime.Now,
                                       BrokerTradeServerId = brokerTradeServer.Id,
                                       DepositAmount = 5000,
                                       TradePlatformCurrency = Currency.USD,
                                       TradePlatformPassword = "PWD"
                                   };
            context.Add(createInvestment);
            context.SaveChanges();

            ipfsService.Setup(x => x.WriteIpfsText(It.IsAny<string>()))
                       .Returns(OperationResult<string>.Ok("hash1234567890"));
            smartContractService.Setup(x => x.RegisterManager(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
                                    It.IsAny<string>(), It.IsAny<string>(), It.IsAny<decimal>(), It.IsAny<decimal>()))
                                .Returns(OperationResult.Ok);

            var result = trustManagementService.CreateInvestmentProgram(new NewManager {Login = "321654", RequestId = createInvestment.Id});
            Assert.IsTrue(result.IsSuccess);

            smartContractService.Verify(x => x.RegisterManager(It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<decimal>(), It.IsAny<decimal>()));
            ipfsService.Verify(x => x.WriteIpfsText(It.IsAny<string>()));

            var manager = context.ManagersAccounts.FirstOrDefault(x => x.Id == result.Data);
            Assert.IsNotNull(manager);
            Assert.AreEqual("321654", manager.Login);
            Assert.AreEqual("hash1234567890", manager.IpfsHash);
            Assert.IsTrue(manager.IsConfirmed);

            var req = context.ManagerRequests.First(x => x.Id == createInvestment.Id);
            Assert.IsNull(req.TradePlatformPassword);

            var investment = context.InvestmentPrograms
                                    .Include(x => x.Token)
                                    .FirstOrDefault(x => x.ManagerAccountId == manager.Id);
            Assert.IsNotNull(investment);
            Assert.AreEqual(createInvestment.Description, investment.Description);
            Assert.AreEqual(createInvestment.DateFrom, investment.DateFrom);
            Assert.AreEqual(createInvestment.DateTo, investment.DateTo);
            Assert.AreEqual(createInvestment.FeeEntrance, investment.FeeEntrance);
            Assert.AreEqual(createInvestment.FeeSuccess, investment.FeeSuccess);
            Assert.AreEqual(createInvestment.FeeManagement, investment.FeeManagement);
            Assert.AreEqual(createInvestment.Period, investment.Period);
            Assert.AreEqual(createInvestment.InvestMinAmount, investment.InvestMinAmount);
            Assert.AreEqual(createInvestment.TokenSymbol, investment.Token.TokenSymbol);
            Assert.AreEqual(createInvestment.TokenName, investment.Token.TokenName);

            Assert.AreEqual(2, context.Periods.Count(x => x.InvestmentProgramId == investment.Id));

            var currentPeriod = context.Periods.FirstOrDefault(x => x.InvestmentProgramId == investment.Id && x.Status == PeriodStatus.InProccess);
            Assert.IsNotNull(currentPeriod);
            Assert.AreEqual(1, currentPeriod.Number);
            Assert.IsTrue(Math.Abs((createInvestment.DateFrom - currentPeriod.DateFrom).TotalSeconds) < 3);
            Assert.IsTrue(Math.Abs((createInvestment.DateFrom.AddDays(createInvestment.Period) - currentPeriod.DateTo).TotalSeconds) < 3);

            var plannedPeriod = context.Periods.FirstOrDefault(x => x.InvestmentProgramId == investment.Id && x.Status == PeriodStatus.Planned);
            Assert.IsNotNull(plannedPeriod);
            Assert.AreEqual(2, plannedPeriod.Number);
            Assert.IsTrue(Math.Abs((createInvestment.DateFrom.AddDays(createInvestment.Period) - plannedPeriod.DateFrom).TotalSeconds) < 3);
            Assert.IsTrue(Math.Abs((createInvestment.DateFrom.AddDays(createInvestment.Period * 2) - plannedPeriod.DateTo).TotalSeconds) < 3);

            var token = investment.Token;
            Assert.IsNotNull(token);
        }

        [Test]
        public void CreateInvestmentProgram2()
        {
            var createInvestment = new ManagerRequests
                                   {
                                       Id = Guid.NewGuid(),
                                       UserId = user.Id,
                                       TokenName = "Token1234",
                                       TokenSymbol = "T1234",
                                       Description = "#2 New investment program",
                                       FeeEntrance = 123,
                                       FeeManagement = 456,
                                       FeeSuccess = 789,
                                       Period = 25,
                                       DateFrom = DateTime.Now,
                                       DateTo = null,
                                       Status = ManagerRequestStatus.Created,
                                       Type = ManagerRequestType.FromCabinet,
                                       Date = DateTime.Now,
                                       BrokerTradeServerId = brokerTradeServer.Id,
                                       DepositAmount = 1000,
                                       TradePlatformCurrency = Currency.USD,
                                       TradePlatformPassword = "PWD"
                                   };
            context.Add(createInvestment);
            context.SaveChanges();

            ipfsService.Setup(x => x.WriteIpfsText(It.IsAny<string>()))
                       .Returns(OperationResult<string>.Ok("hash1234567890"));
            smartContractService.Setup(x => x.RegisterManager(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
                                    It.IsAny<string>(), It.IsAny<string>(), It.IsAny<decimal>(), It.IsAny<decimal>()))
                                .Returns(OperationResult.Ok);

            var result = trustManagementService.CreateInvestmentProgram(new NewManager {Login = "741852", RequestId = createInvestment.Id});
            Assert.IsTrue(result.IsSuccess);

            var investment = context.InvestmentPrograms.FirstOrDefault(x => x.ManagerAccountId == result.Data);
            Assert.IsNotNull(investment);
            Assert.AreEqual(createInvestment.Description, investment.Description);
            Assert.IsTrue(Math.Abs((DateTime.Now - investment.DateFrom).TotalSeconds) < 3);
            Assert.IsNull(investment.DateTo);

            Assert.AreEqual(2, context.Periods.Count(x => x.InvestmentProgramId == investment.Id));

            var period = context.Periods.FirstOrDefault(x => x.InvestmentProgramId == investment.Id && x.Status == PeriodStatus.InProccess);
            Assert.IsNotNull(period);
            Assert.IsTrue(Math.Abs((DateTime.Now - period.DateFrom).TotalSeconds) < 3);
            Assert.IsTrue(Math.Abs((DateTime.Now.AddDays(createInvestment.Period) - period.DateTo).TotalSeconds) < 3);
        }

        [Test]
        public void Invest()
        {
            var period = new Periods
                         {
                             Id = Guid.NewGuid(),
                             InvestmentProgramId = investmentProgram.Id,
                             Status = PeriodStatus.InProccess,
                             Number = 1
                         };
            var investorUser = new ApplicationUser
                               {
                                   Id = Guid.NewGuid(),
                                   Type = UserType.Investor,
                                   IsEnabled = true,
                                   InvestorAccount = new InvestorAccounts(),
                                   Wallets = new List<Wallets> {new Wallets {Amount = 7000,Currency = Currency.GVT}},
                               };
            context.Add(period);
            context.Add(investorUser);
            context.SaveChanges();

            var invest = new Invest
                         {
                             UserId = investorUser.Id,
                             InvestmentProgramId = investmentProgram.Id,
                             Amount = 2500
                         };
            var result = trustManagementService.Invest(invest);
            Assert.IsTrue(result.IsSuccess);

            var lastPeriod = context.Periods
                                    .Where(x => x.InvestmentProgramId == investmentProgram.Id)
                                    .OrderByDescending(x => x.Number)
                                    .FirstOrDefault();
            Assert.IsNotNull(lastPeriod);

            var investRequest = context.InvestmentRequests
                                       .First(x => x.UserId == investorUser.Id &&
                                                   x.InvestmentProgramtId == investmentProgram.Id);
            Assert.IsNotNull(investRequest);

            Assert.AreEqual(investRequest.PeriodId, lastPeriod.Id);
            Assert.AreEqual(invest.Amount, investRequest.Amount);
            Assert.AreEqual(InvestmentRequestStatus.New, investRequest.Status);
            Assert.AreEqual(InvestmentRequestType.Invest, investRequest.Type);
            Assert.IsTrue(Math.Abs((DateTime.Now - investRequest.Date).TotalSeconds) < 3);

            var tx = context.WalletTransactions
                            .Include(x => x.Wallet)
                            .FirstOrDefault(x => x.Wallet.UserId == investorUser.Id);
            Assert.IsNotNull(tx);
            Assert.AreEqual(invest.Amount, tx.Amount);
            Assert.AreEqual(WalletTransactionsType.InvestToProgram, tx.Type);
            Assert.IsTrue(Math.Abs((DateTime.Now - tx.Date).TotalSeconds) < 3);
            Assert.AreEqual(4500, tx.Wallet.Amount);
        }

        [Test]
        public void GetBrokerInvestmentsInitData()
        {
            var manager1 = new ManagerAccounts
                           {
                               Id = Guid.NewGuid(),
                               BrokerTradeServerId = brokerTradeServer.Id
                           };
            var inv1 = new InvestmentPrograms
                       {
                           Id = Guid.NewGuid(),
                           IsEnabled = true,
                           DateFrom = DateTime.Now.AddDays(-1),
                           DateTo = DateTime.Now.AddDays(1),
                           Period = 5,
                           ManagerAccountId = manager1.Id,
                           Description = "#1",
                           Token = new ManagerTokens
                                   {
                                       TokenName = "TokenName1",
                                       TokenSymbol = "TokenSymbol1"
                                   }
                       };
            var period1 = new Periods
                          {
                              Id = Guid.NewGuid(),
                              InvestmentProgramId = inv1.Id,
                              Status = PeriodStatus.InProccess,
                              Number = 5
                          };
            var period2 = new Periods
                          {
                              Id = Guid.NewGuid(),
                              InvestmentProgramId = inv1.Id,
                              Status = PeriodStatus.Planned,
                              Number = 6
                          };
            var manager2 = new ManagerAccounts
                           {
                               Id = Guid.NewGuid(),
                               BrokerTradeServerId = brokerTradeServer.Id
                           };
            var inv2 = new InvestmentPrograms
                       {
                           Id = Guid.NewGuid(),
                           IsEnabled = true,
                           DateFrom = DateTime.Now.AddDays(-1),
                           DateTo = DateTime.Now.AddDays(1),
                           Period = 7,
                           ManagerAccountId = manager2.Id,
                           Description = "#2",
                           Token = new ManagerTokens
                                   {
                                       TokenName = "TokenName2",
                                       TokenSymbol = "TokenSymbol2",
                                       TokenAddress = "TokenAddress2",
                                   }
                       };
            context.Add(manager1);
            context.Add(inv1);
            context.Add(period1);
            context.Add(period2);
            context.Add(manager2);
            context.Add(inv2);
            context.SaveChanges();

            var result = trustManagementService.GetBrokerInvestmentsInitData(brokerTradeServer.Id);
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(2, result.Data.Count);
            //Assert.AreEqual(inv1.Description, result.Data.First(x => x.Investment.Id == inv1.Id).Investment.Description);
            //Assert.AreEqual(inv1.DateFrom, result.Data.First(x => x.Investment.Id == inv1.Id).Investment.DateFrom);
            //Assert.IsNotNull(result.Data.First(x => x.Investment.Id == inv1.Id).Investment.LastPeriod);
            //Assert.AreEqual(period2.Id, result.Data.First(x => x.Investment.Id == inv1.Id).Investment.LastPeriod.Id);
            //Assert.AreEqual(period2.Number, result.Data.First(x => x.Investment.Id == inv1.Id).Investment.LastPeriod.Number);
            //Assert.AreEqual(period2.Status, result.Data.First(x => x.Investment.Id == inv1.Id).Investment.LastPeriod.Status);
            //Assert.AreEqual(inv2.Period, result.Data.First(x => x.Investment.Id == inv2.Id).Investment.Period);
            //Assert.AreEqual(inv2.DateTo, result.Data.First(x => x.Investment.Id == inv2.Id).Investment.DateTo);
            //Assert.AreEqual(inv1.Token.TokenName, result.Data.First(x => x.Investment.Id == inv1.Id).Token.TokenName);
            //Assert.AreEqual(inv2.Token.TokenSymbol, result.Data.First(x => x.Investment.Id == inv2.Id).Token.TokenSymbol);
            //Assert.AreEqual(inv2.Token.TokenAddress, result.Data.First(x => x.Investment.Id == inv2.Id).Token.TokenAddress);
            //Assert.AreEqual(inv2.ManagerAccount.Login, result.Data.First(x => x.Investment.Id == inv2.Id).Account.Login);
            //Assert.AreEqual(inv2.ManagerAccount.RegistrationDate, result.Data.First(x => x.Investment.Id == inv2.Id).Account.RegistrationDate);
            //Assert.AreEqual(inv1.ManagerAccount.IpfsHash, result.Data.First(x => x.Investment.Id == inv1.Id).Account.IpfsHash);
        }

        [Test]
        public void GetBrokerInvestmentsInitDataExpiredOrFuture()
        {
            var manager1 = new ManagerAccounts
                           {
                               Id = Guid.NewGuid(),
                               BrokerTradeServerId = brokerTradeServer.Id
                           };
            var inv1 = new InvestmentPrograms
                       {
                           Id = Guid.NewGuid(),
                           IsEnabled = true,
                           DateFrom = DateTime.Now.AddDays(-10),
                           DateTo = DateTime.Now.AddDays(-3),
                           ManagerAccountId = manager1.Id,
                           Token = new ManagerTokens
                                   {
                                       Id = Guid.NewGuid(),
                                       TokenName = "TokenName1",
                                       TokenSymbol = "TokenSymbol1"
                                   }
                       };
            var manager2 = new ManagerAccounts
                           {
                               Id = Guid.NewGuid(),
                               BrokerTradeServerId = brokerTradeServer.Id
                           };
            var inv2 = new InvestmentPrograms
                       {
                           Id = Guid.NewGuid(),
                           IsEnabled = true,
                           DateFrom = DateTime.Now.AddDays(10),
                           DateTo = DateTime.Now.AddDays(17),
                           ManagerAccountId = manager2.Id,
                           Token = new ManagerTokens
                                   {
                                       Id = Guid.NewGuid(),
                                       TokenName = "TokenName2",
                                       TokenSymbol = "TokenSymbol2"
                                   }
                       };
            context.Add(manager1);
            context.Add(manager2);
            context.Add(inv1);
            context.Add(inv2);
            context.SaveChanges();

            var result = trustManagementService.GetBrokerInvestmentsInitData(brokerTradeServer.Id);
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(0, result.Data.Count);

            inv1.DateTo = DateTime.Now.AddMinutes(1);
            inv2.DateFrom = DateTime.Now.AddMinutes(-1);
            context.SaveChanges();

            result = trustManagementService.GetBrokerInvestmentsInitData(brokerTradeServer.Id);
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(2, result.Data.Count);
        }

        [Test]
        public void GetBrokerInvestmentsInitDataDisabled()
        {
            var manager1 = new ManagerAccounts
                           {
                               Id = Guid.NewGuid(),
                               BrokerTradeServerId = brokerTradeServer.Id
                           };
            var inv1 = new InvestmentPrograms
                       {
                           Id = Guid.NewGuid(),
                           IsEnabled = true,
                           DateFrom = DateTime.Now.AddDays(-10),
                           ManagerAccountId = manager1.Id,
                           Token = new ManagerTokens
                                   {
                                       TokenName = "TokenName1",
                                       TokenSymbol = "TokenSymbol1"
                                   }
                       };
            var manager2 = new ManagerAccounts
                           {
                               Id = Guid.NewGuid(),
                               BrokerTradeServerId = brokerTradeServer.Id
                           };
            var inv2 = new InvestmentPrograms
                       {
                           Id = Guid.NewGuid(),
                           IsEnabled = false,
                           DateFrom = DateTime.Now.AddDays(-10),
                           ManagerAccountId = manager2.Id,
                           Token = new ManagerTokens
                                   {
                                       TokenName = "TokenName2",
                                       TokenSymbol = "TokenSymbol2"
                                   }
                       };
            context.Add(manager1);
            context.Add(manager2);
            context.Add(inv1);
            context.Add(inv2);
            context.SaveChanges();

            var result = trustManagementService.GetBrokerInvestmentsInitData(brokerTradeServer.Id);
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(1, result.Data.Count);
            //Assert.AreEqual(inv1.Id, result.Data.First().Investment.Id);

            inv2.IsEnabled = true;
            context.SaveChanges();

            result = trustManagementService.GetBrokerInvestmentsInitData(brokerTradeServer.Id);
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(2, result.Data.Count);
        }
    }
}
