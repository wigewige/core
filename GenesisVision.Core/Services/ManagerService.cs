﻿using GenesisVision.Common.Models;
using GenesisVision.Common.Services.Interfaces;
using GenesisVision.Core.Helpers.Convertors;
using GenesisVision.Core.Services.Interfaces;
using GenesisVision.Core.ViewModels.Manager;
using GenesisVision.DataModel;
using GenesisVision.DataModel.Enums;
using GenesisVision.DataModel.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GenesisVision.Core.Services
{
    public class ManagerService : IManagerService
    {
        private readonly ApplicationDbContext context;
        private readonly IRateService rateService;

        public ManagerService(ApplicationDbContext context, IRateService rateService)
        {
            this.context = context;
            this.rateService = rateService;
        }

        public OperationResult<Guid> CreateNewInvestmentRequest(NewInvestmentRequest request)
        {
            return InvokeOperations.InvokeOperation(() =>
            {
                var rate = rateService.GetRate(Currency.GVT, Currency.USD);
                if (!rate.IsSuccess)
                    throw new Exception("Cann't get rate GVT/USD");

                var req = new ManagerRequests
                          {
                              Id = Guid.NewGuid(),
                              UserId = request.UserId,
                              Date = DateTime.UtcNow,
                              Type = ManagerRequestType.FromCabinet,
                              Status = ManagerRequestStatus.Created,
                              BrokerTradeServerId = request.BrokerTradeServerId,
                              DepositAmount = request.DepositAmount,
                              DepositInUsd = request.DepositAmount * rate.Data,
                              TradePlatformPassword = request.TradePlatformPassword,
                              TradePlatformCurrency = Currency.USD,
                              TokenName = request.TokenName,
                              TokenSymbol = request.TokenSymbol,
                              Logo = request.Logo,
                              Description = request.Description,
                              Title = request.Title,
                              DateFrom = request.DateFrom ?? DateTime.UtcNow,
                              DateTo = request.DateTo,
                              Period = request.Period,
                              FeeSuccess = request.FeeSuccess ?? 0,
                              FeeManagement = request.FeeManagement ?? 0,
                              FeeEntrance = 0,
                              InvestMaxAmount = request.InvestMaxAmount,
                              InvestMinAmount = request.InvestMinAmount ?? 0
                          };

                var wallet = context.Wallets.First(x => x.UserId == request.UserId && x.Currency == Currency.GVT);
                wallet.Amount -= req.DepositAmount;

                var tx = new WalletTransactions
                         {
                             Id = Guid.NewGuid(),
                             Type = WalletTransactionsType.OpenProgram,
                             WalletId = wallet.Id,
                             Amount = request.DepositAmount,
                             Date = DateTime.UtcNow
                         };

                context.Add(req);
                context.Add(tx);
                context.SaveChanges();

                return req.Id;
            });
        }

        public OperationResult<List<ManagerRequest>> GetCreateAccountRequests(Guid brokerTradeServerId)
        {
            return InvokeOperations.InvokeOperation(() =>
            {
                var result = context.ManagerRequests
                                    .Include(x => x.User)
                                    .ThenInclude(x => x.Profile)
                                    .Where(x => x.BrokerTradeServerId == brokerTradeServerId && x.Status == ManagerRequestStatus.Created)
                                    .Select(x => x.ToManagerRequest(rateService))
                                    .ToList();
                return result;
            });
        }
    }
}
