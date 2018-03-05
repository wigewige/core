using GenesisVision.Common.Services.Interfaces;
using GenesisVision.Core.ViewModels.Broker;
using GenesisVision.Core.ViewModels.Investment;
using GenesisVision.Core.ViewModels.Manager;
using GenesisVision.DataModel.Enums;
using GenesisVision.DataModel.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GenesisVision.Core.Helpers.Convertors
{
    public static partial class Convertors
    {
        public static BrokerInvestmentProgram ToBrokerInvestmentProgram(this InvestmentPrograms program)
        {
            var result = new BrokerInvestmentProgram
                         {
                             Id = program.Id,
                             FeeEntrance = program.FeeEntrance,
                             FeeSuccess = program.FeeSuccess,
                             FeeManagement = program.FeeManagement,
                             IsEnabled = program.IsEnabled,
                             DateFrom = program.DateFrom,
                             DateTo = program.DateTo,
                             Description = program.Description,
                             Period = program.Period,
                             InvestMinAmount = program.InvestMinAmount,
                             InvestMaxAmount = program.InvestMaxAmount,
                             LastPeriod = program.Periods?.FirstOrDefault(x => x.Status == PeriodStatus.InProccess)?.ToPeriod(),
                             ManagerAccountId = program.ManagerAccountId,
                             Login = program.ManagerAccount.Login,
                             IpfsHash = program.ManagerAccount.IpfsHash,
                             TradeIpfsHash = program.ManagerAccount.TradeIpfsHash
                         };
            return result;
        }

        public static InvestmentProgram ToInvestmentProgram(this InvestmentPrograms program, Guid? userId, UserType? userType)
        {
            var result = new InvestmentProgram
                         {
                             Id = program.Id,
                             Title = program.Title,
                             Level = 1,
                             Logo = program.Logo,
                             Balance = 0m,
                             Currency = program.ManagerAccount.Currency,
                             TradesCount = program.ManagerAccount.ManagersAccountsTrades.Count,
                             InvestorsCount = GetInvestorsCount(program),
                             PeriodDuration = program.Period,
                             EndOfPeriod = program.Periods.OrderByDescending(p => p.Number).First().DateFrom,
                             ProfitAvg = 0m,
                             ProfitTotal = 0m,
                             AvailableInvestment = 0m,
                             FeeSuccess = program.FeeSuccess,
                             FeeManagement = program.FeeManagement,
                             HasNewRequests = GetHasNewRequests(program, userType, userId),
                         };
            return result;
        }

        public static InvestmentProgramDetails ToInvestmentProgramDetails(this InvestmentPrograms program, Guid? userId, UserType? userType)
        {
            var result = new InvestmentProgramDetails
                         {
                             Id = program.Id,
                             Title = program.Title,
                             Description = program.Description,
                             Level = 1,
                             Login = program.ManagerAccount.Login,
                             Logo = program.Logo,
                             Balance = 0m,
                             OwnBalance = 0m,
                             Currency = program.ManagerAccount.Currency,
                             TradesCount = program.ManagerAccount.ManagersAccountsTrades.Count,
                             InvestorsCount = GetInvestorsCount(program),
                             PeriodDuration = program.Period,
                             EndOfPeriod = program.Periods.OrderByDescending(p => p.Number).First().DateFrom,
                             ProfitAvg = 0m,
                             ProfitTotal = 0m,
                             AvailableInvestment = 0m,
                             InvestedTokens = 0,
                             FeeSuccess = program.FeeSuccess,
                             FeeManagement = program.FeeManagement,
                             IpfsHash = program.ManagerAccount.IpfsHash,
                             TradeIpfsHash = program.ManagerAccount.TradeIpfsHash,
                             Manager = program.ManagerAccount.User.ToProfilePublic(),
                             HasNewRequests = GetHasNewRequests(program, userType, userId),
                             IsHistoryEnable = GetIsHistoryEnable(program, userType, userId),
                             IsInvestEnable = GetIsInvestEnable(program, userType, userId),
                             IsWithdrawEnable = GetIsWithdrawEnable(program, userType, userId),
                             CanCloseProgram = GetCanCloseProgram(program, userType, userId)
                         };
            return result;
        }

        public static InvestmentProgramDashboard ToInvestmentProgramDashboard(this InvestmentPrograms program, Guid? userId, UserType? userType)
        {
            var result = new InvestmentProgramDashboard
                         {
                             Id = program.Id,
                             Title = program.Title,
                             Level = 1,
                             Logo = program.Logo,
                             Balance = 0m,
                             Currency = program.ManagerAccount.Currency,
                             TradesCount = program.ManagerAccount.ManagersAccountsTrades.Count,
                             InvestorsCount = GetInvestorsCount(program),
                             PeriodDuration = program.Period,
                             EndOfPeriod = program.Periods.OrderByDescending(p => p.Number).First().DateFrom,
                             ProfitAvg = 0m,
                             ProfitTotal = 0m,
                             AvailableInvestment = 0m,
                             InvestedTokens = 0,
                             FeeSuccess = program.FeeSuccess,
                             FeeManagement = program.FeeManagement,
                             Manager = program.ManagerAccount.User.ToProfilePublic(),
                             HasNewRequests = GetHasNewRequests(program, userType, userId),
                             IsHistoryEnable = GetIsHistoryEnable(program, userType, userId),
                             IsInvestEnable = GetIsInvestEnable(program, userType, userId),
                             IsWithdrawEnable = GetIsWithdrawEnable(program, userType, userId),
                             CanCloseProgram = GetCanCloseProgram(program, userType, userId)
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
                       ManagerStartBalance = p.ManagerStartBalance,
                       ManagerStartShare = p.ManagerStartShare,
                       InvestmentRequest = p.InvestmentRequests?.Select(ToInvestmentRequest).ToList() ??
                                           new List<InvestmentProgramRequest>()
                   };
        }

        public static ManagerRequest ToManagerRequest(this ManagerRequests request, IRateService rateService)
        {
            return new ManagerRequest
                   {
                       UserId = request.Id,
                       RequestId = request.Id,
                       Currency = request.TradePlatformCurrency,
                       Password = request.TradePlatformPassword,
                       Name = $"{request.User?.Profile?.FirstName} {request.User?.Profile?.MiddleName} {request.User?.Profile?.LastName}",
                       Email = request.User?.Email,
                       DepositInUsd = request.DepositInUsd
                   };
        }

        public static InvestmentProgramRequest ToInvestmentRequest(this InvestmentRequests inv)
        {
            return new InvestmentProgramRequest
                   {
                       Id = inv.Id,
                       Date = inv.Date,
                       Status = inv.Status,
                       Type = inv.Type,
                       Amount = inv.Amount
                   };
        }
        
        private static int GetInvestorsCount(InvestmentPrograms program)
        {
            return program.InvestmentRequests
                          .Where(r => r.Type == InvestmentRequestType.Invest &&
                                      r.Status == InvestmentRequestStatus.Executed)
                          .Select(r => r.UserId)
                          .Distinct()
                          .Count();
        }

        private static bool GetHasNewRequests(InvestmentPrograms program, UserType? userType, Guid? userId)
        {
            if (!userId.HasValue)
                return false;

            return program.InvestmentRequests.Any(x => x.UserId == userId && x.Status == InvestmentRequestStatus.New);
        }

        private static bool GetIsHistoryEnable(InvestmentPrograms program, UserType? userType, Guid? userId)
        {
            if (!userId.HasValue)
                return false;

            return program.InvestmentRequests.Any(x => x.UserId == userId);
        }

        private static bool GetIsInvestEnable(InvestmentPrograms program, UserType? userType, Guid? userId)
        {
            if (!userId.HasValue || userType != UserType.Investor)
                return false;

            return !program.InvestmentRequests.Any(x => x.UserId == userId &&
                                                        x.Type == InvestmentRequestType.Withdrawal &&
                                                        x.Status == InvestmentRequestStatus.New);
        }

        private static bool GetIsWithdrawEnable(InvestmentPrograms program, UserType? userType, Guid? userId)
        {
            if (!userId.HasValue || userType != UserType.Investor)
                return false;

            if (program.Token.InvestorTokens.FirstOrDefault(x => x.InvestorAccount.UserId == userId)?.Amount <= 0)
                return false;

            return !program.InvestmentRequests.Any(x => x.UserId == userId &&
                                                        x.Type == InvestmentRequestType.Invest &&
                                                        x.Status == InvestmentRequestStatus.New);
        }

        private static bool GetCanCloseProgram(InvestmentPrograms program, UserType? userType, Guid? userId)
        {
            if (!userId.HasValue || userType != UserType.Manager)
                return false;

            return program.ManagerAccount.UserId == userId;
        }
    }
}
