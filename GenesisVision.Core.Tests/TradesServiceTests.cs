using System;
using GenesisVision.Core.Models;
using GenesisVision.Core.Services;
using GenesisVision.Core.Services.Interfaces;
using GenesisVision.Core.ViewModels.Trades;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace GenesisVision.Core.Tests
{
    [TestFixture]
    public class TradesServiceTests
    {
        private ITradesService tradesService;

        [SetUp]
        public void Init()
        {
            tradesService = new TradesService();
        }

        [Test]
        public void TestMetaTraderOrdersAllGoodSuccess()
        {
            var ipfsText =
@"""Login"";""Ticket"";""Symbol"";""PriceOpen"";""PriceClose"";""Profit"";""Volume"";""DateOpen"";""DateClose"";""Direction"";
""102"";""236872"";""TEST"";""0.915"";""1.098"";""281"";""4"";""12/22/2017 2:08:45 PM"";""12/23/2017 1:31:45 AM"";""Sell"";
""102"";""125616"";""TEST"";""0.926"";""1.060"";""260"";""2"";""12/22/2017 2:34:42 PM"";""12/23/2017 12:52:42 AM"";""Sell"";
""102"";""236960"";""TEST"";""0.964"";""1.061"";""334"";""2"";""12/22/2017 10:55:39 AM"";""12/23/2017 12:33:39 AM"";""Buy"";
""102"";""190553"";""TEST"";""0.939"";""1.041"";""266"";""4"";""12/22/2017 9:58:46 AM"";""12/23/2017 12:21:46 AM"";""Sell"";";

            var result = tradesService.ConvertMetaTraderOrdersFromCsv(ipfsText);

            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(4, result.Data.Count);
            Assert.AreEqual(1, result.Data.Count(x => x.Direction == Direction.Buy));
            Assert.AreEqual(3, result.Data.Count(x => x.Direction == Direction.Sell));
            Assert.AreEqual(new DateTime(2017, 12, 22, 14, 08, 45), result.Data.First().DateOpen);
            Assert.AreEqual(new DateTime(2017, 12, 23, 0, 21, 46), result.Data.Last().DateClose);
            Assert.AreEqual(236872, result.Data.First().Ticket);
            Assert.AreEqual("TEST", result.Data.First().Symbol);
            Assert.AreEqual(0.939, result.Data.Last().PriceOpen);
        }

        [Test]
        public void TestMetaTraderOrdersMixedHeadersSuccess()
        {
            var ipfsText =
@"""Volume"";""DateOpen"";""Login"";""Ticket"";""PriceOpen"";""PriceClose"";""Profit"";""DateClose"";""Direction"";""Symbol"";
""6"";""12/22/2017 9:56:46 AM"";""102"";""466172"";""0.992"";""1.071"";""117"";""12/22/2017 11:43:46 PM"";""Sell"";""TEST"";
""9"";""12/22/2017 2:07:38 PM"";""102"";""182837"";""0.956"";""1.077"";""287"";""12/22/2017 9:59:38 PM"";""Buy"";""TEST"";";

            var result = tradesService.ConvertMetaTraderOrdersFromCsv(ipfsText);

            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(2, result.Data.Count);
            Assert.AreEqual(1, result.Data.Count(x => x.Direction == Direction.Buy));
            Assert.AreEqual(1, result.Data.Count(x => x.Direction == Direction.Sell));
            Assert.AreEqual(new DateTime(2017, 12, 22, 9, 56, 46), result.Data.First().DateOpen);
            Assert.AreEqual("TEST", result.Data.First().Symbol);
            Assert.AreEqual(0.956, result.Data.Last().PriceOpen);
        }

        [Test]
        public void TestMetaTraderOrdersNotAllHeadersWrong()
        {
            var ipfsText =
@"""Volume"";""DateOpen"";""Ticket"";""PriceOpen"";""PriceClose"";""Profit"";""DateClose"";""Direction"";""Symbol"";
""6"";""12/22/2017 9:56:46 AM"";""466172"";""0.992"";""1.071"";""117"";""12/22/2017 11:43:46 PM"";""Sell"";""TEST"";
""9"";""12/22/2017 2:07:38 PM"";""182837"";""0.956"";""1.077"";""287"";""12/22/2017 9:59:38 PM"";""Buy"";""TEST"";";

            var result = tradesService.ConvertMetaTraderOrdersFromCsv(ipfsText);

            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual("Header does not contains all labels", result.Errors.First());
        }

        [Test]
        public void TestMetaTraderOrdersBadCsvWrong()
        {
            var ipfsText =
@"""Volume"";""DateOpen"";""Ticket"";""PriceOpen"";""PriceClose"";""Profit"";""DateClose"";""Direction"";""Symbol"";
""6"";""12/22/2017 9:56:46 AM"";""466172"";""0.992"";""1.071"";""117"";""12/22/2017 11:43:46 PM"";""error!"";""TEST"";
""9"";""12/22/2017 2:07:38 PM"";""error!"";""0.956"";""1.077"";""287"";""12/22/2017 9:59:38 PM"";""Buy"";""TEST"";";

            var result = tradesService.ConvertMetaTraderOrdersFromCsv(ipfsText);

            Assert.IsFalse(result.IsSuccess);
        }
    }
}
