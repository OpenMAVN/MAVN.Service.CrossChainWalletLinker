using JetBrains.Annotations;

namespace MAVN.Service.CrossChainWalletLinker.Client.Models
{
    /// <summary>
    /// The configuration item response model
    /// </summary>
    [PublicAPI]
    public class ConfigurationItemResponseModel
    {
        /// <summary>
        /// The configuration item type
        /// </summary>
        public ConfigurationItemType Type { get; set; }
        
        /// <summary>
        /// The configuration item value
        /// </summary>
        public string Value { get; set; }
    }
}
