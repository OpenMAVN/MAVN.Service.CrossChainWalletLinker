namespace Lykke.Service.CrossChainWalletLinker.Domain.Models
{
    /// <summary>
    /// The public wallet address linking status
    /// </summary>
    public enum PublicAddressStatus
    {
        /// <summary>
        /// Public wallet address is not linked 
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
        Linked,
    }
}
