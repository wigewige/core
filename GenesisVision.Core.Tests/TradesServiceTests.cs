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
        public void TestMetaTraderOrdersSuccess1()
        {
            var ipfsText = 
@"""Login"";""Ticket"";""Symbol"";""PriceOpen"";""PriceClose"";""Profit"";""Volume"";""DateOpen"";""DateClose"";""Direction"";
""102"";""150016"";""TEST"";""0,989"";""1,037"";""183"";""4"";""13.12.2017 7:48:57"";""13.12.2017 19:29:57"";""Sell"";
""102"";""431770"";""TEST"";""0,992"";""1,074"";""108"";""9"";""13.12.2017 5:59:49"";""13.12.2017 19:04:49"";""Sell"";
""102"";""486041"";""TEST"";""0,968"";""1,069"";""383"";""6"";""13.12.2017 3:37:49"";""13.12.2017 18:11:49"";""Sell"";
""102"";""200852"";""TEST"";""0,934"";""1,021"";""431"";""5"";""13.12.2017 3:43:54"";""13.12.2017 17:09:54"";""Buy"";";

            var result = tradesService.ConvertMetaTraderOrdersFromCsv(ipfsText);

            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(4, result.Data.Count);
            Assert.AreEqual(1, result.Data.Count(x => x.Direction == Direction.Buy));
            Assert.AreEqual(3, result.Data.Count(x => x.Direction == Direction.Sell));
            Assert.AreEqual(new DateTime(2017, 12, 13, 7, 48, 57), result.Data.First().DateOpen);
            Assert.AreEqual(new DateTime(2017, 12, 13, 17, 09, 54), result.Data.Last().DateClose);
            Assert.AreEqual(150016, result.Data.First().Ticket);
            Assert.AreEqual("TEST", result.Data.First().Symbol);
            Assert.AreEqual(0.934, result.Data.Last().PriceOpen);
        }

        [Test]
        public void TestMetaTraderOrdersSuccess2()
        {
            var ipfsText =
@"""DateOpen"";""Login"";""Ticket"";""PriceOpen"";""PriceClose"";""Profit"";""Volume"";""DateClose"";""Direction"";""Symbol"";
""13.12.2017 4:30:44"";""102"";""275610"";""0,922"";""1,054"";""277"";""6"";""13.12.2017 18:32:44"";""Sell"";""TEST"";
""13.12.2017 6:35:40"";""102"";""446302"";""0,963"";""1,021"";""372"";""1"";""13.12.2017 18:02:40"";""Buy"";""TEST"";";

            var result = tradesService.ConvertMetaTraderOrdersFromCsv(ipfsText);

            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(2, result.Data.Count);
            Assert.AreEqual(1, result.Data.Count(x => x.Direction == Direction.Buy));
            Assert.AreEqual(1, result.Data.Count(x => x.Direction == Direction.Sell));
            Assert.AreEqual(new DateTime(2017, 12, 13, 4, 30, 44), result.Data.First().DateOpen);
            Assert.AreEqual("TEST", result.Data.First().Symbol);
            Assert.AreEqual(0.963, result.Data.Last().PriceOpen);
        }

        [Test]
        public void TestMetaTraderOrdersWrong1()
        {
            var ipfsText =
@"""DateOpen"";""Login"";""Ticket"";""PriceOpen"";""PriceClose"";""Profit"";""DateClose"";""Direction"";""Symbol"";
""13.12.2017 4:30:44"";""102"";""275610"";""0,922"";""1,054"";""277"";""13.12.2017 18:32:44"";""Sell"";""TEST"";
""13.12.2017 6:35:40"";""102"";""446302"";""0,963"";""1,021"";""372"";""13.12.2017 18:02:40"";""Buy"";""TEST"";";

            var result = tradesService.ConvertMetaTraderOrdersFromCsv(ipfsText);

            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual("Header does not contains all labels", result.Errors.First());
        }

        [Test]
        public void TestMetaTraderOrdersWrong2()
        {
            var ipfsText =
@"""DateOpen"";""Login"";""Ticket"";""PriceOpen"";""PriceClose"";""Profit"";""Volume"";""DateClose"";""Direction"";""Symbol"";
""13.12.2017 4:30:44"";""102"";""275610"";""0,922"";""1,054"";""277"";""6"";""13.12.2017 18:32:44"";""Sell"";""TEST"";
""13.12.2017 6:35:40"";""102"";""error!"";""0,963"";""1,021"";""372"";""1"";""13.12.2017 18:02:40"";""Buy"";""TEST"";";

            var result = tradesService.ConvertMetaTraderOrdersFromCsv(ipfsText);

            Assert.IsFalse(result.IsSuccess);
        }
    }
}
