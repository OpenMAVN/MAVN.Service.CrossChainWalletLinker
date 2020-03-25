using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Common.MsSql;
using Lykke.Service.CrossChainWalletLinker.Domain.Enums;
using Lykke.Service.CrossChainWalletLinker.Domain.Models;
using Lykke.Service.CrossChainWalletLinker.Domain.Repositories;
using Lykke.Service.CrossChainWalletLinker.MsSqlRepositories.Entities;
using Microsoft.EntityFrameworkCore;

namespace Lykke.Service.CrossChainWalletLinker.MsSqlRepositories
{
    public class ConfigurationItemsRepository : IConfigurationItemsRepository
    {
        private readonly MsSqlContextFactory<WalletLinkingContext> _contextFactory;

        public ConfigurationItemsRepository(MsSqlContextFactory<WalletLinkingContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task UpsertAsync(ConfigurationItemType type, string value, TransactionContext txContext = null)
        {
            using (var context = _contextFactory.CreateDataContext(txContext))
            {
                var entity = await context.ConfigurationItems.SingleOrDefaultAsync(x => x.Type == type);

                if (entity == null)
                {
                    entity = ConfigurationItemEntity.Create(Guid.NewGuid().ToString(), type, value);

                    context.ConfigurationItems.Add(entity);
                }
                else
                {
                    entity.Value = value;
                }

                await context.SaveChangesAsync();
            }
        }

        public async Task<IConfigurationItem> GetAsync(ConfigurationItemType type, TransactionContext txContext = null)
        {
            using (var context = _contextFactory.CreateDataContext(txContext))
            {
                var entity = await context.ConfigurationItems.SingleOrDefaultAsync(x => x.Type == type);

                return entity;
            }
        }

        public async Task<IEnumerable<IConfigurationItem>> GetAllAsync(TransactionContext txContext = null)
        {
            using (var context = _contextFactory.CreateDataContext(txContext))
            {
                var entities = await context.ConfigurationItems.ToListAsync();

                return entities;
            }
        }
    }
}
