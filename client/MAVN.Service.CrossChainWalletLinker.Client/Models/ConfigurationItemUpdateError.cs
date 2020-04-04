using JetBrains.Annotations;

namespace MAVN.Service.CrossChainWalletLinker.Client.Models
{
    /// <summary>
    /// The configuration item update error
    /// </summary>
    [PublicAPI]
    public enum ConfigurationItemUpdateError
    {
        /// <summary>
        /// No errors
        /// </summary>
        None,
        
        /// <summary>
        /// The configuration item type provided is not expected by server
        /// </summary>
        TypeNotRecognized
    }
}
