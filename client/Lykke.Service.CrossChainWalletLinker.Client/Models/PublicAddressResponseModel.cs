using JetBrains.Annotations;

namespace Lykke.Service.CrossChainWalletLinker.Client.Models
{
    /// <summary>
    /// The public address response model
    /// </summary>
    [PublicAPI]
    public class PublicAddressResponseModel
    {
        /// <summary>
        /// The linked public address
        /// </summary>
        public string PublicAddress { get; set; }
        
        /// <summary>
        /// The public address status
        /// </summary>
        public PublicAddressStatus Status { get; set; }
        
        /// <summary>
        /// Wallet linking error
        /// </summary>
        public PublicAddressError Error { get; set; }
    }
}
