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
                                       DateTo = DateTime.Now.AddDays(100)
                                   };
            var result = trustManagementService.CreateInvestmentProgram(createInvestment);
            Assert.IsTrue(result.IsSuccess);

            var investment = context.InvestmentPrograms
                                    .Include(x => x.Tokens)
                                    .FirstOrDefault(x => x.Id == result.Data);
            Assert.IsNotNull(investment);
            Assert.AreEqual(createInvestment.Description, investment.Description);
            Assert.AreEqual(createInvestment.DateFrom, investment.DateFrom);
            Assert.AreEqual(createInvestment.DateTo, investment.DateTo);
            Assert.AreEqual(createInvestment.FeeEntrance, investment.FeeEntrance);
            Assert.AreEqual(createInvestment.FeeSuccess, investment.FeeSuccess);
            Assert.AreEqual(createInvestment.FeeManagement, investment.FeeManagement);
            Assert.AreEqual(createInvestment.Period, investment.Period);

            var period = context.Periods.FirstOrDefault(x => x.InvestmentProgramId == investment.Id);
            Assert.IsNotNull(period);
            Assert.AreEqual(PeriodStatus.InProccess, period.Status);
            Assert.AreEqual(1, period.Number);
            Assert.IsTrue(Math.Abs((createInvestment.DateFrom.Value - period.DateFrom).TotalSeconds) < 3);
            Assert.IsTrue(Math.Abs((createInvestment.DateFrom.Value.AddDays(createInvestment.Period) - period.DateTo).TotalSeconds) < 3);

            var token = investment.Tokens;
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
    }
}
