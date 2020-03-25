using JetBrains.Annotations;

namespace Lykke.Service.CrossChainWalletLinker.Client.Models
{
    /// <summary>
    /// The public wallet address status
    /// </summary>
    [PublicAPI]
    public enum PublicAddressStatus
    {
        /// <summary>
        /// The public wallet address is not linked
        /// </summary>
        NotLinked,
        
        /// <summary>
        /// The wallet linking is pending customer approval
        /// </summary>
        PendingCustomerApproval,
        
        /// <summary>
        /// The linking process is pending confirmation in blockchain
        /// </summary>
        PendingConfirmation,
            
        /// <summary>
        /// The public wallet address is linked
        /// </summary>
        Linked
    }
}
