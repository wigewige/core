using System;
using System.Linq;
using GenesisVision.DataModel;
using GenesisVision.DataModel.Models;
using GenesisVision.Core.Services;
using GenesisVision.Core.Services.Interfaces;
using GenesisVision.DataModel.Enums;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

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
            context.Add(new Periods
                        {
                            InvestmentProgramId = investmentPrograms.Id,
                            Id = Guid.NewGuid(),
                            DateFrom = DateTime.Now.AddDays(-1),
                            DateTo = DateTime.Now.AddDays(9),
                            Number = 1,
                            Status = PeriodStatus.InProccess
                        });
            context.Add(new Periods
                        {
                            InvestmentProgramId = investmentPrograms.Id,
                            Id = Guid.NewGuid(),
                            DateFrom = DateTime.Now.AddDays(9),
                            DateTo = DateTime.Now.AddDays(19),
                            Number = 2,
                            Status = PeriodStatus.Planned
                        });
            context.SaveChanges();

            var result = trustManagementService.GetClosingPeriodData(investmentPrograms.Id);

            Assert.IsTrue(result.IsSuccess);
            Assert.IsFalse(result.Data.CanCloseCurrentPeriod);
            Assert.IsNotNull(result.Data.CurrentPeriod);
            Assert.AreEqual(result.Data.CurrentPeriod.Id, context.Periods.First(x => x.Number == 1).Id);
            Assert.IsNotNull(result.Data.NextPeriod);
            Assert.AreEqual(result.Data.NextPeriod.Id, context.Periods.First(x => x.Number == 2).Id);

            context.Periods.RemoveRange(context.Periods);
            context.SaveChanges();
        }

        [Test]
        public void GetClosingPeriodDataCanClose()
        {
            context.Add(new Periods
                        {
                            InvestmentProgramId = investmentPrograms.Id,
                            Id = Guid.NewGuid(),
                            DateFrom = DateTime.Now.AddDays(-15),
                            DateTo = DateTime.Now.AddDays(-5),
                            Number = 1,
                            Status = PeriodStatus.InProccess
                        });
            context.Add(new Periods
                        {
                            InvestmentProgramId = investmentPrograms.Id,
                            Id = Guid.NewGuid(),
                            DateFrom = DateTime.Now.AddDays(-5),
                            DateTo = DateTime.Now.AddDays(5),
                            Number = 2,
                            Status = PeriodStatus.Planned
                        });
            context.SaveChanges();

            var result = trustManagementService.GetClosingPeriodData(investmentPrograms.Id);

            Assert.IsTrue(result.IsSuccess);
            Assert.IsTrue(result.Data.CanCloseCurrentPeriod);
            Assert.IsNotNull(result.Data.CurrentPeriod);
            Assert.AreEqual(result.Data.CurrentPeriod.Id, context.Periods.First(x => x.Number == 1).Id);
            Assert.IsNotNull(result.Data.NextPeriod);
            Assert.AreEqual(result.Data.NextPeriod.Id, context.Periods.First(x => x.Number == 2).Id);

            context.Periods.RemoveRange(context.Periods);
            context.SaveChanges();
        }

        [Test]
        public void GetClosingPeriodDataPeriods()
        {
            context.Add(new Periods
                        {
                            InvestmentProgramId = investmentPrograms.Id,
                            Id = Guid.NewGuid(),
                            Number = 1,
                            Status = PeriodStatus.Closed
                        });
            context.Add(new Periods
                        {
                            InvestmentProgramId = investmentPrograms.Id,
                            Id = Guid.NewGuid(),
                            Number = 2,
                            Status = PeriodStatus.InProccess
                        });
            context.SaveChanges();
            
            var result = trustManagementService.GetClosingPeriodData(investmentPrograms.Id);

            Assert.IsTrue(result.IsSuccess);
            Assert.IsNotNull(result.Data.CurrentPeriod);
            Assert.AreEqual(result.Data.CurrentPeriod.Id, context.Periods.First(x => x.Number == 2).Id);
            Assert.IsNull(result.Data.NextPeriod);

            context.Periods.RemoveRange(context.Periods);
            context.SaveChanges();
        }
    }
}
