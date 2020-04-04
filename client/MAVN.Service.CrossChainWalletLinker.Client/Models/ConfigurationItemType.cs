using JetBrains.Annotations;

namespace MAVN.Service.CrossChainWalletLinker.Client.Models
{
    /// <summary>
    /// The configuration item type
    /// </summary>
    [PublicAPI]
    public enum ConfigurationItemType
    {
        /// <summary>
        /// Fee value when linking for the first time
        /// </summary>
        FirstTimeLinkingFee,
        
        /// <summary>
        /// Fee value when linking for the second and subsequent times
        /// </summary>
        SubsequentLinkingFee,
    }
}
