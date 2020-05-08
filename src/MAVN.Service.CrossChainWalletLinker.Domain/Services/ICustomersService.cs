using System;
using System.Threading.Tasks;
using MAVN.Numerics;
using MAVN.Service.CrossChainWalletLinker.Domain.Models;

namespace MAVN.Service.CrossChainWalletLinker.Domain.Services
{
    public interface ICustomersService
    {
        Task<PublicAddressResultModel> GetPublicAddressAsync(Guid customerId);

        Task<Money18> GetNextWalletLinkingFeeAsync(Guid customerId);
    }
}
