using JetBrains.Annotations;

namespace MAVN.Service.CrossChainWalletLinker.Client.Models
{
    /// <summary>
    /// The wallet linking response model
    /// </summary>
    [PublicAPI]
    public class LinkingRequestResponseModel
    {
        /// <summary>
        /// The linking code
        /// </summary>
        public string LinkCode { get; set; }
        
        /// <summary>
        /// Wallet linking error
        /// </summary>
        public LinkingError Error { get; set; }
    }
}
