using GenesisVision.DataModel;
using GenesisVision.DataModel.Enums;
using GenesisVision.DataModel.Models;
using GenesisVision.PaymentService.Models;
using GenesisVision.PaymentService.Services;
using GenesisVision.PaymentService.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GenesisVision.Core.Tests.Payments
{
    [TestFixture]
    public class ExternalPaymetsTests
    {
        private ApplicationDbContext context;
        private IPaymentTransactionService paymentService;

        private ApplicationUser user;

        [SetUp]
        public void Init()
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseInMemoryDatabase("databaseManagerService")
                .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning));
            context = new ApplicationDbContext(optionsBuilder.Options);

            paymentService = new PaymentTransactionService(Substitute.For<ILogger<IPaymentTransactionService>>(), context);

            user = new ApplicationUser
                   {
                       Id = Guid.NewGuid(),
                       IsEnabled = true,
                       Wallets = new List<Wallets> {new Wallets {Amount = 0, Currency = WalletCurrency.GVT}},
                       BlockchainAddresses = new List<BlockchainAddresses>
                                             {
                                                 new BlockchainAddresses {Id = Guid.NewGuid(), Address = "0x00", Currency = "GVT"}
                                             }
                   };
            context.Add(user);
            context.SaveChanges();
        }

        [Test]
        public void PaymentsCallbackTest1()
        {
            var wallet = context.Wallets.First(w => w.UserId == user.Id && w.Currency == WalletCurrency.GVT);
            Assert.IsNull(wallet.WalletTransactions);

            var payment = new ProcessPaymentTransaction
                          {
                              Amount = 100,
                              Address = "0x00",
                              Status = PaymentTransactionStatus.ConfirmedAndValidated,
                              Currency = "GVT"
                          };

            paymentService.ProcessCallback(payment);

            Assert.AreEqual(100, context.Wallets.First(w => w.UserId == user.Id && w.Currency == WalletCurrency.GVT).Amount);
        }
    }
}
