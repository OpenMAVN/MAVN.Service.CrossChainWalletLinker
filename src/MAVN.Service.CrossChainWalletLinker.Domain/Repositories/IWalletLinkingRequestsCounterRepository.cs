﻿using System.Threading.Tasks;
using MAVN.Persistence.PostgreSQL.Legacy;
using MAVN.Service.CrossChainWalletLinker.Domain.Models;

namespace MAVN.Service.CrossChainWalletLinker.Domain.Repositories
{
    public interface IWalletLinkingRequestsCounterRepository
    {
        Task UpsertAsync(string customerId, int approvalsCounter, TransactionContext txContext = null);

        Task<IWalletLinkingRequestsCounter> GetAsync(string customerId, TransactionContext txContext = null);
    }
}
