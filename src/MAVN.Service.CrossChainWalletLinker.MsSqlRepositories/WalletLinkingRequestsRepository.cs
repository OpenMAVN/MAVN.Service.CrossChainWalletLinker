using System;
using System.Threading.Tasks;
using Common.Log;
using MAVN.Numerics;
using Lykke.Common.Log;
using MAVN.Persistence.PostgreSQL.Legacy;
using MAVN.Service.CrossChainWalletLinker.Domain.Exceptions;
using MAVN.Service.CrossChainWalletLinker.Domain.Models;
using MAVN.Service.CrossChainWalletLinker.Domain.Repositories;
using MAVN.Service.CrossChainWalletLinker.MsSqlRepositories.Entities;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace MAVN.Service.CrossChainWalletLinker.MsSqlRepositories
{
    public class WalletLinkingRequestsRepository : IWalletLinkingRequestsRepository
    {
        private readonly PostgreSQLContextFactory<WalletLinkingContext> _contextFactory;
        private readonly ILog _log;

        private const string EntityNotFoundWhenApproving = "Entity was not found during approval";
        private const string EntityNotFoundWhenConfirming = "Entity was not found during confirmation";
        private const string CannotDeleteLinkingRequest = "Can't delete linking request";

        public WalletLinkingRequestsRepository(PostgreSQLContextFactory<WalletLinkingContext> contextFactory, ILogFactory logFactory)
        {
            _contextFactory = contextFactory;
            _log = logFactory.CreateLog(this);
        }

        public async Task AddAsync(string customerId, string privateAddress, string linkCode, TransactionContext txContext = null)
        {
            using (var context = _contextFactory.CreateDataContext(txContext))
            {
                var entity = WalletLinkingRequestEntity.Create(customerId, privateAddress, linkCode);
                
                await context.LinkingRequests.AddAsync(entity);

                try
                {
                    await context.SaveChangesAsync();
                }
                catch (DbUpdateException e)
                {
                    if (e.InnerException is PostgresException sqlException &&
                       sqlException.SqlState == PostgresErrorCodes.UniqueViolation)
                    {
                        throw new LinkingRequestAlreadyExistsException(customerId);
                    }

                    throw;
                }
            }
        }

        public async Task DeleteByIdAsync(string customerId, TransactionContext txContext = null)
        {
            using (var context = _contextFactory.CreateDataContext(txContext))
            {
                var entity = new WalletLinkingRequestEntity{CustomerId = customerId};

                context.LinkingRequests.Attach(entity);

                context.LinkingRequests.Remove(entity);

                try
                {
                    await context.SaveChangesAsync();
                }
                catch (DbUpdateException e)
                {
                    _log.Error(e, CannotDeleteLinkingRequest);
                    
                    throw new InvalidOperationException(CannotDeleteLinkingRequest);
                }
            }
        }

        public async Task SetApprovedAsync(string customerId, string publicAddress, string signature, Money18 fee, TransactionContext txContext = null)
        {
            using (var context = _contextFactory.CreateDataContext(txContext))
            {
                var entity = new WalletLinkingRequestEntity {CustomerId = customerId};

                context.LinkingRequests.Attach(entity);

                entity.PublicAddress = publicAddress;
                entity.Signature = signature;
                entity.Timestamp = DateTime.UtcNow;
                entity.Fee = fee;

                try
                {
                    await context.SaveChangesAsync();
                }
                catch (DbUpdateException e)
                {
                    _log.Error(e, EntityNotFoundWhenApproving);
                    
                    throw new InvalidOperationException(EntityNotFoundWhenApproving);
                }
            }
        }

        public async Task SetPrivatelyConfirmedAsync(string customerId, TransactionContext txContext = null)
        {
            using (var context = _contextFactory.CreateDataContext(txContext))
            {
                var entity = new WalletLinkingRequestEntity{CustomerId = customerId};

                context.LinkingRequests.Attach(entity);

                entity.IsConfirmedInPrivate = true;
                entity.Timestamp = DateTime.UtcNow;

                try
                {
                    await context.SaveChangesAsync();
                }
                catch (DbUpdateException e)
                {
                    _log.Error(e, EntityNotFoundWhenConfirming);
                    
                    throw new InvalidOperationException(EntityNotFoundWhenConfirming);
                }
            }
        }

        public async Task SetPubliclyConfirmedAsync(string customerId, TransactionContext txContext = null)
        {
            using (var context = _contextFactory.CreateDataContext(txContext))
            {
                var entity = new WalletLinkingRequestEntity{CustomerId = customerId};

                context.LinkingRequests.Attach(entity);

                entity.IsConfirmedInPublic = true;
                entity.Timestamp = DateTime.UtcNow;

                try
                {
                    await context.SaveChangesAsync();
                }
                catch (DbUpdateException e)
                {
                    _log.Error(e, EntityNotFoundWhenConfirming);
                    
                    throw new InvalidOperationException(EntityNotFoundWhenConfirming);
                }
            }
        }

        public async Task<IWalletLinkingRequest> GetByIdAsync(string customerId, TransactionContext txContext = null)
        {
            using (var context = _contextFactory.CreateDataContext(txContext))
            {
                var entity = await context.LinkingRequests.FindAsync(customerId);

                return entity;
            }
        }

        public async Task<IWalletLinkingRequest> GetByPrivateAddressAsync(string privateAddress, TransactionContext txContext = null)
        {
            using (var context = _contextFactory.CreateDataContext(txContext))
            {
                var entity =
                    await context.LinkingRequests.SingleOrDefaultAsync(e => e.PrivateAddress == privateAddress);

                return entity;
            }
        }
    }
}
