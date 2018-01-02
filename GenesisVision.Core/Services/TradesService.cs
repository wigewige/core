using GenesisVision.DataModel.Models;
using GenesisVision.Core.Models;
using GenesisVision.Core.Services.Interfaces;
using GenesisVision.Core.ViewModels.Trades;
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
                                                           "Login",
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
                }
            };

        public OperationResult<List<MetaTraderOrder>> ConvertMetaTraderOrdersFromCsv(string ipfsText)
        {
            return InvokeOperations.InvokeOperation(() =>
            {
                Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture("en-US");

                var csv = ipfsText.Split(Environment.NewLine);
                var header = GetHeaderMap(BrokerTradeServerType.MetaTrader4, csv.First());
                if (!header.IsSuccess)
                    throw new Exception(header.Errors.FirstOrDefault());

                var trades = new List<MetaTraderOrder>();
                for (var i = 0; i < csv.Length; i++)
                {
                    if (i == 0)
                        continue;

                    var fields = csv[i].Split(";");
                    var order = new MetaTraderOrder();
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
                        else if (propInfo.PropertyType == typeof(Direction))
                            value = Enum.Parse(typeof(Direction), field);

                        propInfo.SetValue(order, value, null);
                    }
                    trades.Add(order);
                }
                return trades;
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
