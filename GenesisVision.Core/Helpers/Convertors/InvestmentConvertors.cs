using GenesisVision.Core.ViewModels.Investment;
using GenesisVision.Core.ViewModels.Manager;
using GenesisVision.DataModel.Models;
using System.Collections.Generic;
using System.Linq;

namespace GenesisVision.Core.Helpers.Convertors
{
    public static partial class Convertors
    {
        public static Period ToPeriod(this Periods p)
        {
            return new Period
                   {
                       Id = p.Id,
                       DateFrom = p.DateFrom,
                       DateTo = p.DateTo,
                       Status = p.Status,
                       Number = p.Number,
                       StartBalance = p.StartBalance,
                       InvestmentRequest = p.InvestmentRequests?.Select(ToInvestmentRequest).ToList() ??
                                           new List<InvestmentRequest>()
                   };
        }

        public static ManagerRequest ToManagerRequest(this ManagerRequests request)
        {
            return new ManagerRequest
                   {
                       UserId = request.Id,
                       RequestId = request.Id,
                       Currency = request.TradePlatformCurrency,
                       Password = request.TradePlatformPassword,
                       Name = $"{request.User?.Profile?.FirstName} {request.User?.Profile?.MiddleName} {request.User?.Profile?.LastName}",
                       Email = request.User?.Email,
                   };
        }

        public static InvestmentRequest ToInvestmentRequest(this InvestmentRequests inv)
        {
            return new InvestmentRequest
                   {
                       Id = inv.Id,
                       Date = inv.Date,
                       Status = inv.Status,
                       Type = inv.Type,
                       Amount = inv.Amount
                   };
        }

        public static InvestmentProgram ToInvestmentProgram(this InvestmentPrograms inv)
        {
            return new InvestmentProgram
                   {
                       Account = new ManagerAccount
                                 {
                                     Id = inv.ManagerAccount.Id,
                                     Login = inv.ManagerAccount.Login,
                                     Currency = inv.ManagerAccount.Currency,
                                     IpfsHash = inv.ManagerAccount.IpfsHash,
                                     IsConfirmed = inv.ManagerAccount.IsConfirmed,
                                     BrokerTradeServer = inv.ManagerAccount.BrokerTradeServer?.ToBrokerTradeServers(),
                                 },
                       Investment = new Investment
                                    {
                                        Id = inv.Id,
                                        InvestMinAmount = inv.InvestMinAmount,
                                        InvestMaxAmount = inv.InvestMaxAmount,
                                        Description = inv.Description,
                                        IsEnabled = inv.IsEnabled,
                                        FeeEntrance = inv.FeeEntrance,
                                        FeeSuccess = inv.FeeSuccess,
                                        FeeManagement = inv.FeeManagement,
                                        DateFrom = inv.DateFrom,
                                        DateTo = inv.DateTo,
                                        Period = inv.Period,
                                        ManagerAccountId = inv.ManagerAccountId,
                                        ManagerTokensId = inv.ManagerTokensId,
                                        Logo = inv.Logo,
                                        Rating = inv.Rating,
                                        OrdersCount = inv.OrdersCount,
                                        TotalProfit = inv.TotalProfit,
                                        LastPeriod = inv.Periods?.OrderByDescending(x => x.Number).FirstOrDefault()?.ToPeriod()
                                    },
                       Token = new ManagerToken
                               {
                                   Id = inv.Token.Id,
                                   TokenSymbol = inv.Token.TokenSymbol,
                                   TokenAddress = inv.Token.TokenAddress,
                                   TokenName = inv.Token.TokenName
                               }
                   };
        }
    }
}
