using JetBrains.Annotations;

namespace Lykke.Service.CrossChainWalletLinker.Client.Models
{
    /// <summary>
    /// The wallet unlink response model
    /// </summary>
    [PublicAPI]
    public class UnlinkResponseModel
    {
        /// <summary>
        /// The wallet linking error
        /// </summary>
        public LinkingError Error { get; set; }
    }
}
