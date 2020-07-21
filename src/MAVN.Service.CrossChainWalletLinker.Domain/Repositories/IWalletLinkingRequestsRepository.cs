﻿using System.Threading.Tasks;
using MAVN.Numerics;
using MAVN.Persistence.PostgreSQL.Legacy;
using MAVN.Service.CrossChainWalletLinker.Domain.Models;

namespace MAVN.Service.CrossChainWalletLinker.Domain.Repositories
{
    public interface IWalletLinkingRequestsRepository
    {
        Task AddAsync(string customerId, string privateAddress, string linkCode, TransactionContext txContext = null);

        Task DeleteByIdAsync(string customerId, TransactionContext txContext = null);
        
        Task SetApprovedAsync(string customerId, string publicAddress, string signature, Money18 fee, TransactionContext txContext = null);

        Task SetPrivatelyConfirmedAsync(string customerId, TransactionContext txContext = null);
        
        Task SetPubliclyConfirmedAsync(string customerId, TransactionContext txContext = null);

        Task<IWalletLinkingRequest> GetByIdAsync(string customerId, TransactionContext txContext = null);
        
        Task<IWalletLinkingRequest> GetByPrivateAddressAsync(string privateAddress, TransactionContext txContext = null);
    }
}
