using System;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Lykke.Service.CrossChainWalletLinker.Client.Models;
using Refit;

namespace Lykke.Service.CrossChainWalletLinker.Client
{
    // This is an example of service controller interfaces.
    // Actual interface methods must be placed here (not in ICrossChainWalletLinkerClient interface).

    /// <summary>
    /// CrossChainWalletLinker client API interface.
    /// </summary>
    [PublicAPI]
    public interface ICrossChainWalletLinkerApi
    {
        /// <summary>
        /// Create new wallet linking request, it is async
        /// </summary>
        /// <param name="customerId">The customer id</param>
        /// <returns></returns>
        [Post("/api/linkRequests")]
        Task<LinkingRequestResponseModel> CreateLinkRequestAsync(Guid customerId);

        /// <summary>
        /// Unlink wallet, it is synchronous
        /// </summary>
        /// <param name="customerId">The customer id</param>
        /// <returns></returns>
        [Delete("/api/linkRequests")]
        Task<UnlinkResponseModel> UnlinkAsync(Guid customerId);

        /// <summary>
        /// Approve existing linking request
        /// </summary>
        /// <param name="request">Approval details</param>
        /// <returns></returns>
        [Post("/api/linkRequests/approval")]
        Task<LinkingApprovalResponseModel> ApproveLinkRequestAsync([Body] LinkApprovalRequestModel request);
    }
}
