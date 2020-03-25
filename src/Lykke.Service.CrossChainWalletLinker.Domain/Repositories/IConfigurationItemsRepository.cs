using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Common.MsSql;
using Lykke.Service.CrossChainWalletLinker.Domain.Enums;
using Lykke.Service.CrossChainWalletLinker.Domain.Models;

namespace Lykke.Service.CrossChainWalletLinker.Domain.Repositories
{
    public interface IConfigurationItemsRepository
    {
        Task UpsertAsync(ConfigurationItemType type, string value, TransactionContext txContext = null);

        Task<IConfigurationItem> GetAsync(ConfigurationItemType type, TransactionContext txContext = null);
        
        Task<IEnumerable<IConfigurationItem>> GetAllAsync(TransactionContext txContext = null);
    }
}
