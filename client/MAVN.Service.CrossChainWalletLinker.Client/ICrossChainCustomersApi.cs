using System;
using System.Threading.Tasks;
using JetBrains.Annotations;
using MAVN.Service.CrossChainWalletLinker.Client.Models;
using Refit;

namespace MAVN.Service.CrossChainWalletLinker.Client
{
    /// <summary>
    /// Cross chain customers API interface  
    /// </summary>
    [PublicAPI]
    public interface ICrossChainCustomersApi
    {
        /// <summary>
        /// Get customer linked public address
        /// </summary>
        /// <param name="customerId">The customer id</param>
        /// <returns></returns>
        [Get("/api/customers/{customerId}/publicAddress")]
        Task<PublicAddressResponseModel> GetLinkedPublicAddressAsync(Guid customerId);

        /// <summary>
        /// Get next wallet linking fee value
        /// </summary>
        /// <param name="customerId">The customer id</param>
        /// <returns></returns>
        [Get("/api/customers/{customerId}/nextFee")]
        Task<NextFeeResponseModel> GetNextFeeAsync(Guid customerId);
    }
}
