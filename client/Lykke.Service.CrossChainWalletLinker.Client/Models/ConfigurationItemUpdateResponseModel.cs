using JetBrains.Annotations;

namespace Lykke.Service.CrossChainWalletLinker.Client.Models
{
    /// <summary>
    /// The configuration item update response
    /// </summary>
    [PublicAPI]
    public class ConfigurationItemUpdateResponseModel
    {
        /// <summary>
        /// The configuration update error
        /// </summary>
        public ConfigurationItemUpdateError Error { get; set; }
    }
}
