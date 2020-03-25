using JetBrains.Annotations;

namespace Lykke.Service.CrossChainWalletLinker.Client.Models
{
    /// <summary>
    /// The wallet linking approval response model
    /// </summary>
    [PublicAPI]
    public class LinkingApprovalResponseModel
    {
        /// <summary>
        /// The wallet linking error
        /// </summary>
        public LinkingError Error { get; set; }
    }
}
