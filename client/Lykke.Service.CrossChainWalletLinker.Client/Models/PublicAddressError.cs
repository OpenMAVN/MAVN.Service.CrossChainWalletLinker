using JetBrains.Annotations;

namespace Lykke.Service.CrossChainWalletLinker.Client.Models
{
    /// <summary>
    /// The public wallet address error
    /// </summary>
    [PublicAPI]
    public enum PublicAddressError
    {
        /// <summary>
        /// No error
        /// </summary>
        None,
        
        /// <summary>
        /// The customer id is not valid
        /// </summary>
        InvalidCustomerId,
    }
}
