using System;
using System.Threading.Tasks;
using Falcon.Numerics;
using Lykke.Service.CrossChainWalletLinker.Domain.Models;

namespace Lykke.Service.CrossChainWalletLinker.Domain.Services
{
    public interface ICustomersService
    {
        Task<PublicAddressResultModel> GetPublicAddressAsync(Guid customerId);

        Task<Money18> GetNextWalletLinkingFeeAsync(Guid customerId);
    }
}
