using GenesisVision.DataModel;
using GenesisVision.DataModel.Models;
using GenesisVision.Core.Services.Validators;
using GenesisVision.Core.Services.Validators.Interfaces;
using GenesisVision.Core.ViewModels.Investment;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using GenesisVision.DataModel.Enums;

namespace GenesisVision.Core.Tests.Validators
{
    [TestFixture]
    public class InvestorValidatorTests
    {
        private IInvestorValidator investorValidator;

        private ApplicationDbContext context;
        private InvestmentPrograms investment;

        private IPrincipal user;

        [SetUp]
        public void Init()
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseInMemoryDatabase("databaseInvestorValidator");
            context = new ApplicationDbContext(optionsBuilder.Options);

            investment = new InvestmentPrograms
                         {
                             Id = Guid.NewGuid(),
                             DateFrom = DateTime.Now,
                             DateTo = DateTime.Now.AddMonths(1),
                             IsEnabled = true,
                             Period = 10
                         };
            var period = new Periods
                         {
                             Id = Guid.NewGuid(),
                             DateFrom = DateTime.Now,
                             DateTo = DateTime.Now.AddDays(10),
                             Number = 1,
                             Status = PeriodStatus.InProccess,
                             InvestmentProgramId = investment.Id
                         };
            context.Add(investment);
            context.Add(period);
            context.SaveChanges();

            user = new ClaimsPrincipal();

            investorValidator = new InvestorValidator(context);
        }

        [Test]
        public void ValidateInvest()
        {
            var res1 = investorValidator.ValidateInvest(user, new Invest {InvestmentProgramId = Guid.NewGuid()});
            Assert.IsTrue(res1.Any(x => x.Contains("Does not find investment program")));
            Assert.AreEqual(1, res1.Count);

            const string error = "There are no new period";
            context.Periods.First().Status = PeriodStatus.InProccess;
            context.SaveChanges();
            var res2 = investorValidator.ValidateInvest(user, new Invest {InvestmentProgramId = investment.Id});
            Assert.IsTrue(res2.Any(x => x.Contains(error)));
            Assert.AreEqual(1, res1.Count);

            context.Periods.First().Status = PeriodStatus.Closed;
            context.SaveChanges();
            var res3 = investorValidator.ValidateInvest(user, new Invest {InvestmentProgramId = investment.Id});
            Assert.IsTrue(res3.Any(x => x.Contains(error)));
            Assert.AreEqual(1, res1.Count);

            context.Periods.First().Status = PeriodStatus.Planned;
            context.SaveChanges();
            var res4 = investorValidator.ValidateInvest(user, new Invest {InvestmentProgramId = investment.Id, Amount = 0});
            Assert.IsTrue(res4.Any(x => x == "Amount must be greater than zero"));
            Assert.AreEqual(1, res1.Count);

            var res5 = investorValidator.ValidateInvest(user, new Invest {InvestmentProgramId = investment.Id, Amount = 500});
            Assert.IsEmpty(res5);
        }
    }
}
