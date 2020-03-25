using System.Threading.Tasks;
using Lykke.Common.MsSql;
using Lykke.Service.CrossChainWalletLinker.Domain.Models;

namespace Lykke.Service.CrossChainWalletLinker.Domain.Repositories
{
    public interface IWalletLinkingRequestsCounterRepository
    {
        Task UpsertAsync(string customerId, int approvalsCounter, TransactionContext txContext = null);

        Task<IWalletLinkingRequestsCounter> GetAsync(string customerId, TransactionContext txContext = null);
    }
}
