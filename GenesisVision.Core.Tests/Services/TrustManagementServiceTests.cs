using GenesisVision.Core.Services;
using GenesisVision.Core.Services.Interfaces;
using GenesisVision.Core.ViewModels.Investment;
using GenesisVision.DataModel;
using GenesisVision.DataModel.Enums;
using GenesisVision.DataModel.Models;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System;
using System.Linq;

namespace GenesisVision.Core.Tests.Services
{
    [TestFixture]
    public class TrustManagementServiceTests
    {
        private ITrustManagementService trustManagementService;

        private ApplicationDbContext context;

        private InvestmentPrograms investmentPrograms;

        [SetUp]
        public void Init()
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseInMemoryDatabase("databaseTrustManagementService");
            context = new ApplicationDbContext(optionsBuilder.Options);
            
            investmentPrograms = new InvestmentPrograms
                                 {
                                     Id = Guid.NewGuid(),
                                     DateFrom = DateTime.Now.AddYears(-1),
                                     FeeEntrance = 100m,
                                     FeeSuccess = 120m,
                                     FeeManagement = 10m,
                                     Description = "Test",
                                     IsEnabled = true,
                                     Period = 10,
                                     InvestMinAmount = 500,
                                     InvestMaxAmount = 1500
                                 };
            context.Add(investmentPrograms);
            context.SaveChanges();

            trustManagementService = new TrustManagementService(context);
        }

        [Test]
        public void GetClosingPeriodDataCannotClose()
        {
            var period1 = new Periods
                          {
                              InvestmentProgramId = investmentPrograms.Id,
                              Id = Guid.NewGuid(),
                              DateFrom = DateTime.Now.AddDays(-1),
                              DateTo = DateTime.Now.AddDays(9),
                              Number = 1,
                              Status = PeriodStatus.InProccess
                          };
            var period2 = new Periods
                          {
                              InvestmentProgramId = investmentPrograms.Id,
                              Id = Guid.NewGuid(),
                              DateFrom = DateTime.Now.AddDays(9),
                              DateTo = DateTime.Now.AddDays(19),
                              Number = 2,
                              Status = PeriodStatus.Planned
                          };
            context.AddRange(new[] {period1, period2});
            context.SaveChanges();

            var result = trustManagementService.GetClosingPeriodData(investmentPrograms.Id);

            Assert.IsTrue(result.IsSuccess);
            Assert.IsFalse(result.Data.CanCloseCurrentPeriod);
            Assert.IsNotNull(result.Data.CurrentPeriod);
            Assert.AreEqual(result.Data.CurrentPeriod.Id, context.Periods.First(x => x.Number == 1 && x.InvestmentProgramId == investmentPrograms.Id).Id);
            Assert.IsNotNull(result.Data.NextPeriod);
            Assert.AreEqual(result.Data.NextPeriod.Id, context.Periods.First(x => x.Number == 2 && x.InvestmentProgramId == investmentPrograms.Id).Id);

            context.Periods.RemoveRange(context.Periods.Where(x => x.Id == period1.Id || x.Id == period2.Id));
            context.SaveChanges();
        }

        [Test]
        public void GetClosingPeriodDataCanClose()
        {
            var period1 = new Periods
                          {
                              InvestmentProgramId = investmentPrograms.Id,
                              Id = Guid.NewGuid(),
                              DateFrom = DateTime.Now.AddDays(-15),
                              DateTo = DateTime.Now.AddDays(-5),
                              Number = 1,
                              Status = PeriodStatus.InProccess
                          };
            var period2 = new Periods
                          {
                              InvestmentProgramId = investmentPrograms.Id,
                              Id = Guid.NewGuid(),
                              DateFrom = DateTime.Now.AddDays(-5),
                              DateTo = DateTime.Now.AddDays(5),
                              Number = 2,
                              Status = PeriodStatus.Planned
                          };
            context.AddRange(new[] {period1, period2});
            context.SaveChanges();

            var result = trustManagementService.GetClosingPeriodData(investmentPrograms.Id);

            Assert.IsTrue(result.IsSuccess);
            Assert.IsTrue(result.Data.CanCloseCurrentPeriod);
            Assert.IsNotNull(result.Data.CurrentPeriod);
            Assert.AreEqual(result.Data.CurrentPeriod.Id, context.Periods.First(x => x.Number == 1 && x.InvestmentProgramId == investmentPrograms.Id).Id);
            Assert.IsNotNull(result.Data.NextPeriod);
            Assert.AreEqual(result.Data.NextPeriod.Id, context.Periods.First(x => x.Number == 2 && x.InvestmentProgramId == investmentPrograms.Id).Id);

            context.Periods.RemoveRange(context.Periods.Where(x => x.Id == period1.Id || x.Id == period2.Id));
            context.SaveChanges();
        }

        [Test]
        public void GetClosingPeriodDataPeriods()
        {
            var period1 = new Periods
                          {
                              InvestmentProgramId = investmentPrograms.Id,
                              Id = Guid.NewGuid(),
                              Number = 1,
                              Status = PeriodStatus.Closed
                          };
            var period2 = new Periods
                          {
                              InvestmentProgramId = investmentPrograms.Id,
                              Id = Guid.NewGuid(),
                              Number = 2,
                              Status = PeriodStatus.InProccess
                          };
            context.AddRange(new[] {period1, period2});
            context.SaveChanges();
            
            var result = trustManagementService.GetClosingPeriodData(investmentPrograms.Id);

            Assert.IsTrue(result.IsSuccess);
            Assert.IsNotNull(result.Data.CurrentPeriod);
            Assert.AreEqual(result.Data.CurrentPeriod.Id, context.Periods.First(x => x.Number == 2 && x.InvestmentProgramId == investmentPrograms.Id).Id);
            Assert.IsNull(result.Data.NextPeriod);

            context.Periods.RemoveRange(context.Periods.Where(x => x.Id == period1.Id || x.Id == period2.Id));
            context.SaveChanges();
        }

        [Test]
        public void CreateInvestmentProgram()
        {
            var managerAccount = new ManagerAccounts {Id = Guid.NewGuid()};
            context.Add(managerAccount);
            context.SaveChanges();

            var createInvestment = new CreateInvestment
                                   {
                                       Description = "#1 New investment program",
                                       FeeEntrance = 123,
                                       FeeManagement = 456,
                                       FeeSuccess = 789,
                                       Period = 5,
                                       ManagersAccountId = managerAccount.Id,
                                       DateFrom = DateTime.Now.AddDays(20).AddHours(15).AddMinutes(10).AddSeconds(5),
                                       DateTo = DateTime.Now.AddDays(100),
                                       InvestMinAmount = 0.9999m,
                                       InvestMaxAmount = 100000.01m
                                   };
            var result = trustManagementService.CreateInvestmentProgram(createInvestment);
            Assert.IsTrue(result.IsSuccess);

            var investment = context.InvestmentPrograms
                                    .Include(x => x.Token)
                                    .FirstOrDefault(x => x.Id == result.Data);
            Assert.IsNotNull(investment);
            Assert.AreEqual(createInvestment.Description, investment.Description);
            Assert.AreEqual(createInvestment.DateFrom, investment.DateFrom);
            Assert.AreEqual(createInvestment.DateTo, investment.DateTo);
            Assert.AreEqual(createInvestment.FeeEntrance, investment.FeeEntrance);
            Assert.AreEqual(createInvestment.FeeSuccess, investment.FeeSuccess);
            Assert.AreEqual(createInvestment.FeeManagement, investment.FeeManagement);
            Assert.AreEqual(createInvestment.Period, investment.Period);
            Assert.AreEqual(createInvestment.InvestMinAmount, investment.InvestMinAmount);
            Assert.AreEqual(createInvestment.InvestMaxAmount, investment.InvestMaxAmount);

            var period = context.Periods.FirstOrDefault(x => x.InvestmentProgramId == investment.Id);
            Assert.IsNotNull(period);
            Assert.AreEqual(PeriodStatus.InProccess, period.Status);
            Assert.AreEqual(1, period.Number);
            Assert.IsTrue(Math.Abs((createInvestment.DateFrom.Value - period.DateFrom).TotalSeconds) < 3);
            Assert.IsTrue(Math.Abs((createInvestment.DateFrom.Value.AddDays(createInvestment.Period) - period.DateTo).TotalSeconds) < 3);

            var token = investment.Token;
            Assert.IsNotNull(token);
        }

        [Test]
        public void CreateInvestmentProgram2()
        {
            var managerAccount = new ManagerAccounts {Id = Guid.NewGuid()};
            context.Add(managerAccount);
            context.SaveChanges();

            var createInvestment = new CreateInvestment
                                   {
                                       Description = "#2 New investment program",
                                       Period = 25,
                                       ManagersAccountId = managerAccount.Id,
                                       DateFrom = null,
                                       DateTo = null
                                   };

            var result = trustManagementService.CreateInvestmentProgram(createInvestment);
            Assert.IsTrue(result.IsSuccess);

            var investment = context.InvestmentPrograms.FirstOrDefault(x => x.Id == result.Data);
            Assert.IsNotNull(investment);
            Assert.AreEqual(createInvestment.Description, investment.Description);
            Assert.IsTrue(Math.Abs((DateTime.Now - investment.DateFrom).TotalSeconds) < 3);
            Assert.IsNull(investment.DateTo);

            var period = context.Periods.FirstOrDefault(x => x.InvestmentProgramId == investment.Id);
            Assert.IsNotNull(period);
            Assert.IsTrue(Math.Abs((DateTime.Now - period.DateFrom).TotalSeconds) < 3);
            Assert.IsTrue(Math.Abs((DateTime.Now.AddDays(createInvestment.Period) - period.DateTo).TotalSeconds) < 3);
        }

        [Test]
        public void Invest()
        {
            var managerAccount = new ManagerAccounts {Id = Guid.NewGuid()};
            var appUser = new ApplicationUser {Id = Guid.NewGuid()};
            var investor = new InvestorAccounts {Id = Guid.NewGuid(), UserId = appUser.Id};
            context.Add(managerAccount);
            context.Add(appUser);
            context.Add(investor);
            context.SaveChanges();

            var createInvestment = new CreateInvestment
                                   {
                                       Description = "#2 New investment program",
                                       Period = 25,
                                       ManagersAccountId = managerAccount.Id
                                   };
            var investmentId = trustManagementService.CreateInvestmentProgram(createInvestment);
            Assert.IsTrue(investmentId.IsSuccess);

            var invest = new Invest
                         {
                             UserId = appUser.Id,
                             InvestmentProgramId = investmentId.Data,
                             Amount = 2500
                         };
            var result = trustManagementService.Invest(invest);
            Assert.IsTrue(result.IsSuccess);

            var lastPeriod = context.Periods
                                    .Where(x => x.InvestmentProgramId == investmentId.Data)
                                    .OrderByDescending(x => x.Number)
                                    .FirstOrDefault();
            Assert.IsNotNull(lastPeriod);

            var investRequest = context.InvestmentRequests
                                       .First(x => x.UserId == appUser.Id &&
                                                   x.InvestmentProgramtId == investmentId.Data);
            Assert.IsNotNull(investRequest);

            Assert.AreEqual(investRequest.PeriodId, lastPeriod.Id);
            Assert.AreEqual(invest.Amount, investRequest.Amount);
            Assert.AreEqual(InvestmentRequestStatus.New, investRequest.Status);
            Assert.AreEqual(InvestmentRequestType.Invest, investRequest.Type);
            Assert.IsTrue(Math.Abs((DateTime.Now - investRequest.Date).TotalSeconds) < 3);
        }

        [Test]
        public void GetBrokerInvestmentsInitData()
        {
            var brokerTradeServer = new BrokerTradeServers{Id = Guid.NewGuid()};
            var managerAccount = new ManagerAccounts {Id = Guid.NewGuid(), BrokerTradeServerId = brokerTradeServer.Id};
            var inv1 = new InvestmentPrograms
                       {
                           Id = Guid.NewGuid(),
                           IsEnabled = true,
                           DateFrom = DateTime.Now.AddDays(-1),
                           DateTo = DateTime.Now.AddDays(1),
                           Period = 5,
                           ManagersAccountId = managerAccount.Id,
                           Description = "#1"
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
            var inv2 = new InvestmentPrograms
                       {
                           Id = Guid.NewGuid(),
                           IsEnabled = true,
                           DateFrom = DateTime.Now.AddDays(-1),
                           DateTo = DateTime.Now.AddDays(1),
                           Period = 7,
                           ManagersAccountId = managerAccount.Id,
                           Description = "#2"
                       };
            context.Add(brokerTradeServer);
            context.Add(managerAccount);
            context.Add(inv1);
            context.Add(period1);
            context.Add(period2);
            context.Add(inv2);
            context.SaveChanges();

            var result = trustManagementService.GetBrokerInvestmentsInitData(brokerTradeServer.Id);
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(2, result.Data.Count);
            Assert.AreEqual(inv1.Description, result.Data.First(x => x.Id == inv1.Id).Description);
            Assert.AreEqual(inv1.DateFrom, result.Data.First(x => x.Id == inv1.Id).DateFrom);
            Assert.IsNotNull(result.Data.First(x => x.Id == inv1.Id).LastPeriod);
            Assert.AreEqual(period2.Id, result.Data.First(x => x.Id == inv1.Id).LastPeriod.Id);
            Assert.AreEqual(period2.Number, result.Data.First(x => x.Id == inv1.Id).LastPeriod.Number);
            Assert.AreEqual(period2.Status, result.Data.First(x => x.Id == inv1.Id).LastPeriod.Status);
            Assert.AreEqual(inv2.Period, result.Data.First(x => x.Id == inv2.Id).Period);
            Assert.AreEqual(inv2.DateTo, result.Data.First(x => x.Id == inv2.Id).DateTo);
        }

        [Test]
        public void GetBrokerInvestmentsInitDataExpiredOrFuture()
        {
            var brokerTradeServer = new BrokerTradeServers {Id = Guid.NewGuid()};
            var managerAccount = new ManagerAccounts {Id = Guid.NewGuid(), BrokerTradeServerId = brokerTradeServer.Id};
            var inv1 = new InvestmentPrograms
                       {
                           Id = Guid.NewGuid(),
                           IsEnabled = true,
                           DateFrom = DateTime.Now.AddDays(-10),
                           DateTo = DateTime.Now.AddDays(-3),
                           ManagersAccountId = managerAccount.Id,
                       };
            var inv2 = new InvestmentPrograms
                       {
                           Id = Guid.NewGuid(),
                           IsEnabled = true,
                           DateFrom = DateTime.Now.AddDays(10),
                           DateTo = DateTime.Now.AddDays(17),
                           ManagersAccountId = managerAccount.Id,
                       };
            context.Add(brokerTradeServer);
            context.Add(managerAccount);
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
            var brokerTradeServer = new BrokerTradeServers {Id = Guid.NewGuid()};
            var managerAccount = new ManagerAccounts {Id = Guid.NewGuid(), BrokerTradeServerId = brokerTradeServer.Id};
            var inv1 = new InvestmentPrograms
                       {
                           Id = Guid.NewGuid(),
                           IsEnabled = true,
                           DateFrom = DateTime.Now.AddDays(-10),
                           ManagersAccountId = managerAccount.Id
                       };
            var inv2 = new InvestmentPrograms
                       {
                           Id = Guid.NewGuid(),
                           IsEnabled = false,
                           DateFrom = DateTime.Now.AddDays(-10),
                           ManagersAccountId = managerAccount.Id,
                       };
            context.Add(brokerTradeServer);
            context.Add(managerAccount);
            context.Add(inv1);
            context.Add(inv2);
            context.SaveChanges();

            var result = trustManagementService.GetBrokerInvestmentsInitData(brokerTradeServer.Id);
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(1, result.Data.Count);
            Assert.AreEqual(inv1.Id, result.Data.First().Id);

            inv2.IsEnabled = true;
            context.SaveChanges();

            result = trustManagementService.GetBrokerInvestmentsInitData(brokerTradeServer.Id);
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(2, result.Data.Count);
        }
    }
}
