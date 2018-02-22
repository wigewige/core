using GenesisVision.Core.ViewModels.Trades;
using GenesisVision.DataModel.Models;

namespace GenesisVision.Core.Helpers.Convertors
{
    public static partial class Convertors
    {
        public static OpenOrderModel ToOpenOrder(this ManagersAccountsOpenTrades trade)
        {
            return new OpenOrderModel
                   {
                       Id = trade.Id,
                       Date = trade.DateOpenOrder,
                       Profit = trade.Profit,
                       Price = trade.Price,
                       Direction = trade.Direction,
                       Volume = trade.Volume,
                       Ticket = trade.Ticket,
                       Symbol = trade.Symbol
                   };
        }

        public static OrderModel ToOrder(this ManagersAccountsTrades trade)
        {
            return new OrderModel
                   {
                       Id = trade.Id,
                       Ticket = trade.Ticket,
                       Volume = trade.Volume,
                       Symbol = trade.Symbol,
                       Profit = trade.Profit,
                       Direction = trade.Direction,
                       DateClose = trade.DateClose,
                       DateOpen = trade.DateOpen,
                       PriceClose = trade.PriceClose,
                       PriceOpen = trade.PriceOpen,
                       Date = trade.Date,
                       Entry = trade.Entry,
                       Price = trade.Price
                   };
        }
    }
}
