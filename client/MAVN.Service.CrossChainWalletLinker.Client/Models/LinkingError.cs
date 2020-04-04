using JetBrains.Annotations;

namespace MAVN.Service.CrossChainWalletLinker.Client.Models
{
    /// <summary>
    /// The wallet linking/unlinking error
    /// </summary>
    [PublicAPI]
    public enum LinkingError
    {
        /// <summary>
        /// No errors
        /// </summary>
        None,
        
        /// <summary>
        /// The customer id provided is not valid
        /// </summary>
        InvalidCustomerId,
        
        /// <summary>
        /// The linking/unlinking request already exists
        /// </summary>
        LinkingRequestAlreadyExists,
        
        /// <summary>
        /// The customer's private wallet address is not assigned
        /// </summary>
        CustomerWalletMissing,
        
        /// <summary>
        /// The linking/unlinking request does not exist yet
        /// </summary>
        LinkingRequestDoesNotExist,
        
        /// <summary>
        /// The public address is not valid
        /// </summary>
        InvalidPublicAddress,
        
        /// <summary>
        /// The signature is not valid
        /// </summary>
        InvalidSignature,
        
        /// <summary>
        /// The linking request has already been approved
        /// </summary>
        LinkingRequestAlreadyApproved,
        
        /// <summary>
        /// The private address is not valid
        /// </summary>
        InvalidPrivateAddress,
        
        /// <summary>
        /// The linking request can't be deleted while it is being processed in blockchain
        /// </summary>
        CannotDeleteLinkingRequestWhileConfirming,
        
        /// <summary>
        /// The balance is not enough to approve linking request if fee > 0
        /// </summary>
        NotEnoughFunds,

        /// <summary>
        /// There is no customer with this customerId
        /// </summary>
        CustomerDoesNotExist,

        /// <summary>
        /// Customer's wallet is blocked
        /// </summary>
        CustomerWalletBlocked,
    }
}
