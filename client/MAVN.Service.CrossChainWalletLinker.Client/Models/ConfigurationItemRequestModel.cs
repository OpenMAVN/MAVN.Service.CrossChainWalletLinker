using System.ComponentModel.DataAnnotations;
using JetBrains.Annotations;

namespace MAVN.Service.CrossChainWalletLinker.Client.Models
{
    /// <summary>
    /// The new configuration item request model 
    /// </summary>
    [PublicAPI]
    public class ConfigurationItemRequestModel
    {
        /// <summary>
        /// The configuration item type
        /// </summary>
        public ConfigurationItemType Type { get; set; }
        
        /// <summary>
        /// The configuration item value
        /// </summary>
        [Required]
        public string Value { get; set; }
    }
}
