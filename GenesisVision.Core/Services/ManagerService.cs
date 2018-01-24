﻿using GenesisVision.Core.Helpers.Convertors;
using GenesisVision.Core.Models;
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

        public ManagerService(ApplicationDbContext context)
        {
            this.context = context;
        }

        public OperationResult<Guid> CreateNewInvestmentRequest(NewInvestmentRequest request)
        {
            return InvokeOperations.InvokeOperation(() =>
            {
                var req = new ManagerRequests
                          {
                              Id = Guid.NewGuid(),
                              UserId = request.UserId,
                              Date = DateTime.Now,
                              Type = ManagerRequestType.FromCabinet,
                              Status = ManagerRequestStatus.Created,
                              BrokerTradeServerId = request.BrokerTradeServerId,
                              DepositAmount = request.DepositAmount,
                              TradePlatformPassword = request.TradePlatformPassword,
                              TradePlatformCurrency = "USD",
                              TokenName = request.TokenName,
                              TokenSymbol = request.TokenSymbol,
                              Logo = request.Logo,
                              Description = request.Description,
                              DateFrom = request.DateFrom ?? DateTime.Now,
                              DateTo = request.DateTo,
                              Period = request.Period,
                              FeeSuccess = request.FeeSuccess,
                              FeeManagement = request.FeeManagement,
                              FeeEntrance = request.FeeEntrance,
                              InvestMaxAmount = request.InvestMaxAmount,
                              InvestMinAmount = request.InvestMinAmount
                          };

                var wallet = context.Wallets.First(x => x.UserId == request.UserId);
                wallet.Amount -= req.DepositAmount;

                var tx = new WalletTransactions
                         {
                             Id = Guid.NewGuid(),
                             Type = WalletTransactionsType.OpenProgram,
                             UserId = request.UserId,
                             Amount = request.DepositAmount,
                             Date = DateTime.Now
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
                                    .Select(x => x.ToManagerRequest())
                                    .ToList();
                return result;
            });
        }
    }
}
