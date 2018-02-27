using GenesisVision.Core.ViewModels.Investment;
using GenesisVision.Core.ViewModels.Manager;
using GenesisVision.DataModel.Models;
using System.Collections.Generic;
using System.Linq;
using GenesisVision.DataModel.Enums;

namespace GenesisVision.Core.Helpers.Convertors
{
    public static partial class Convertors
    {
        public static InvestmentProgramDetails ToInvestmentProgramDetails(this InvestmentPrograms program)
        {
            var result = new InvestmentProgramDetails
                         {
                             Id = program.Id,
                             Title = program.Title,
                             Level = 1,
                             Logo = program.Logo,
                             Balance = 0m,
                             TradesCount = program.ManagerAccount.ManagersAccountsTrades.Count,
                             InvestorsCount = program.InvestmentRequests
                                                     .Where(r => r.Type == InvestmentRequestType.Invest)
                                                     .Select(r => r.UserId)
                                                     .Distinct()
                                                     .Count(),
                             PeriodDuration = program.Period,
                             EndOfPeriod = program.Periods.OrderByDescending(p => p.Number).First().DateFrom,
                             ProfitAvg = 0m,
                             ProfitTotal = 0m,
                             AvailableInvestment = 0m,
                             InvestedTokens = 0,
                             FeeSuccess = program.FeeSuccess,
                             FeeManagement = program.FeeManagement,
                             IsPending = false,
                             IsHistoryEnable = false,
                             IsInvestEnable = true,
                             IsWithdrawEnable = false,
                         };
            return result;
        }

        public static InvestmentProgram ToInvestmentProgram(this InvestmentPrograms program)
        {
            var result = new InvestmentProgram
                         {
                             Id = program.Id,
                             Title = program.Title,
                             Level = 1,
                             Logo = program.Logo,
                             Balance = 0m,
                             TradesCount = program.ManagerAccount.ManagersAccountsTrades.Count,
                             InvestorsCount = program.InvestmentRequests
                                                     .Where(r => r.Type == InvestmentRequestType.Invest)
                                                     .Select(r => r.UserId)
                                                     .Distinct()
                                                     .Count(),
                             PeriodDuration = program.Period,
                             EndOfPeriod = program.Periods.OrderByDescending(p => p.Number).First().DateFrom,
                             ProfitAvg = 0m,
                             AvailableInvestment = 0m,
                             FeeSuccess = program.FeeSuccess,
                             FeeManagement = program.FeeManagement,
                             IsPending = false,
                         };
            return result;
        }

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
        
        public static InvestmentProgramStatistic ToInvestmentProgramStatistic(this ManagersAccountsStatistics statistic)
        {
            return new InvestmentProgramStatistic
                   {
                       Date = statistic.Date,
                       CurrentBalance = statistic.CurrentBalance,
                       Profit = statistic.Profit
                   };
        }
    }
}
