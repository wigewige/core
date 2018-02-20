using GenesisVision.Core.Models;
using GenesisVision.Core.Services.Interfaces;
using GenesisVision.Core.ViewModels.Trades;
using GenesisVision.Core.ViewModels.Trades.Interfaces;
using GenesisVision.DataModel;
using GenesisVision.DataModel.Enums;
using GenesisVision.DataModel.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;

namespace GenesisVision.Core.Services
{
    public class TradesService : ITradesService
    {
        public Dictionary<BrokerTradeServerType, List<string>> brokerCsvMap =
            new Dictionary<BrokerTradeServerType, List<string>>
            {
                {
                    BrokerTradeServerType.MetaTrader4, new List<string>
                                                       {
                                                           "Ticket",
                                                           "Symbol",
                                                           "PriceOpen",
                                                           "PriceClose",
                                                           "Profit",
                                                           "Volume",
                                                           "DateOpen",
                                                           "DateClose",
                                                           "Direction"
                                                       }
                },
                {
                    BrokerTradeServerType.MetaTrader5, new List<string>
                                                       {
                                                           "Ticket",
                                                           "Symbol",
                                                           "Price",
                                                           "Profit",
                                                           "Volume",
                                                           "Date",
                                                           "Direction",
                                                           "Entry"
                                                       }
                }
            };

        private readonly ApplicationDbContext context;

        public TradesService(ApplicationDbContext context)
        {
            this.context = context;
        }

        public OperationResult<List<OrderModel>> GetOrders(Guid accountId, DateTime? dateFrom = null, DateTime? dateTo = null)
        {
            return InvokeOperations.InvokeOperation(() =>
            {
                var trades = context.ManagersAccountsTrades
                                    .Where(x => x.ManagerAccountId == accountId);

                if (dateFrom.HasValue)
                    trades = trades.Where(x => x.DateClose >= dateFrom.Value);
                if (dateTo.HasValue)
                    trades = trades.Where(x => x.DateClose <= dateTo);

                var result = trades.Select(x => new OrderModel
                                                {
                                                    Id = x.Id,
                                                    Ticket = x.Ticket,
                                                    Volume = x.Volume,
                                                    Symbol = x.Symbol,
                                                    Profit = x.Profit,
                                                    Direction = x.Direction,
                                                    DateClose = x.DateClose,
                                                    DateOpen = x.DateOpen,
                                                    PriceClose = x.PriceClose,
                                                    PriceOpen = x.PriceOpen,
                                                    Date = x.Date,
                                                    Entry = x.Entry,
                                                    Price = x.Price
                                                })
                                   .ToList();
                return result;
            });
        }

        public OperationResult<List<OrderModel>> ConvertMetaTrader4OrdersFromCsv(string ipfsText)
        {
            return InvokeOperations.InvokeOperation(() =>
            {
                Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture("en-US");

                var csv = ipfsText.Split(Environment.NewLine);
                var header = GetHeaderMap(BrokerTradeServerType.MetaTrader4, csv.First());
                if (!header.IsSuccess)
                    throw new Exception(header.Errors.FirstOrDefault());

                var trades = new List<OrderModel>();
                for (var i = 0; i < csv.Length; i++)
                {
                    if (i == 0)
                        continue;

                    var fields = csv[i].Split(";");
                    var order = new OrderModel();
                    foreach (var headerText in header.Data)
                    {
                        var field = fields[headerText.Value].Replace("\"", "");
                        var propInfo = order.GetType().GetProperty(headerText.Key);
                        dynamic value = null;
                        if (propInfo.PropertyType == typeof(string))
                            value = field;
                        else if (propInfo.PropertyType == typeof(decimal))
                            value = Convert.ToDecimal(field);
                        else if (propInfo.PropertyType == typeof(int))
                            value = Convert.ToInt32(field);
                        else if (propInfo.PropertyType == typeof(long))
                            value = Convert.ToInt64(field);
                        else if (propInfo.PropertyType == typeof(DateTime))
                            value = DateTime.ParseExact(field, "G", null);
                        else if (propInfo.PropertyType == typeof(TradeDirectionType))
                            value = Enum.Parse(typeof(TradeDirectionType), field);

                        propInfo.SetValue(order, value, null);
                    }
                    trades.Add(order);
                }
                return trades;
            });
        }

        public OperationResult SaveNewTrade(NewTradeEvent tradeEvent)
        {
            return InvokeOperations.InvokeOperation(() =>
            {
                var type = context.ManagersAccounts
                                  .Include(x => x.BrokerTradeServer)
                                  .First(x => x.Id == tradeEvent.ManagerAccountId).BrokerTradeServer.Type;

                switch (type)
                {
                    case BrokerTradeServerType.MetaTrader4:
                        var mt4Orders = (IEnumerable<IMetaTrader4Order>)tradeEvent.Trades;
                        foreach (var mt4Order in mt4Orders.Where(x => x.Direction == TradeDirectionType.Buy || x.Direction == TradeDirectionType.Sell))
                        {
                            context.Add(new ManagersAccountsTrades
                                        {
                                            Id = Guid.NewGuid(),
                                            ManagerAccountId = tradeEvent.ManagerAccountId,
                                            Ticket = mt4Order.Ticket,
                                            Symbol = mt4Order.Symbol,
                                            Volume = mt4Order.Volume,
                                            Profit = mt4Order.Profit,
                                            Direction = mt4Order.Direction,
                                            DateOpen = mt4Order.DateOpen,
                                            DateClose = mt4Order.DateClose,
                                            PriceOpen = mt4Order.PriceOpen,
                                            PriceClose = mt4Order.PriceClose
                                        });
                        }
                        break;
                    case BrokerTradeServerType.MetaTrader5:
                        var mt5Orders = (IEnumerable<IMetaTrader5Order>)tradeEvent.Trades;
                        foreach (var mt5Order in mt5Orders.Where(x => (x.Direction == TradeDirectionType.Buy || x.Direction == TradeDirectionType.Sell) && x.Entry != TradeEntryType.In))
                        {
                            context.Add(new ManagersAccountsTrades
                                        {
                                            Id = Guid.NewGuid(),
                                            ManagerAccountId = tradeEvent.ManagerAccountId,
                                            Ticket = mt5Order.Ticket,
                                            Symbol = mt5Order.Symbol,
                                            Volume = mt5Order.Volume,
                                            Profit = mt5Order.Profit,
                                            Direction = mt5Order.Direction,
                                            Date = mt5Order.Date,
                                            Price = mt5Order.Price,
                                            Entry = mt5Order.Entry
                                        });
                        }
                        break;
                    default:
                        throw new NotSupportedException();
                }

                context.SaveChanges();
            });
        }

        private OperationResult<Dictionary<string, int>> GetHeaderMap(BrokerTradeServerType type, string headerStr)
        {
            var mapLabels = brokerCsvMap[type];
            var result = mapLabels.ToDictionary(x => x, x => -1);
            var headerLabels = headerStr.Split(';').ToList();
            foreach (var mapLabel in mapLabels)
            {
                for (var i = 0; i < headerLabels.Count; i++)
                {
                    if (headerLabels[i].Replace("\"", "") != mapLabel)
                        continue;

                    result[mapLabel] = i;
                    break;
                }
            }

            return result.Any(x => x.Value == -1)
                ? OperationResult<Dictionary<string, int>>.Failed("Header does not contains all labels")
                : OperationResult<Dictionary<string, int>>.Ok(result);
        }
    }
}
