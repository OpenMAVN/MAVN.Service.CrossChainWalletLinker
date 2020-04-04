using JetBrains.Annotations;

namespace MAVN.Service.CrossChainWalletLinker.Client
{
    /// <summary>
    /// CrossChainWalletLinker client interface.
    /// </summary>
    [PublicAPI]
    public interface ICrossChainWalletLinkerClient
    {
        // Make your app's controller interfaces visible by adding corresponding properties here.
        // NO actual methods should be placed here (these go to controller interfaces, for example - ICrossChainWalletLinkerApi).
        // ONLY properties for accessing controller interfaces are allowed.

        /// <summary>Application Api interface</summary>
        ICrossChainWalletLinkerApi WalletLinkingApi { get; }
        
        /// <summary>
        /// Customer API interface
        /// </summary>
        ICrossChainCustomersApi CustomersApi { get; }
        
        /// <summary>
        /// Configuration API interface
        /// </summary>
        ICrossChainConfigurationApi ConfigurationApi { get; }
    }
}
