using JetBrains.Annotations;

namespace MAVN.Service.CrossChainWalletLinker.Client.Models
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
