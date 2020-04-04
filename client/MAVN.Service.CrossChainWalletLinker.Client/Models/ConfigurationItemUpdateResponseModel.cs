using JetBrains.Annotations;

namespace MAVN.Service.CrossChainWalletLinker.Client.Models
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
