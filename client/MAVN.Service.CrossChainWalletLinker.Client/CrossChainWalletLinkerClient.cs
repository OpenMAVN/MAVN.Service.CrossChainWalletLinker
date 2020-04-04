using Lykke.HttpClientGenerator;

namespace MAVN.Service.CrossChainWalletLinker.Client
{
    /// <summary>
    /// CrossChainWalletLinker API aggregating interface.
    /// </summary>
    public class CrossChainWalletLinkerClient : ICrossChainWalletLinkerClient
    {
        // Note: Add similar Api properties for each new service controller

        /// <summary>Interface to CrossChainWalletLinker Api.</summary>
        public ICrossChainWalletLinkerApi WalletLinkingApi { get; private set; }
        
        /// <summary>
        /// Interface for base API
        /// </summary>
        public ICrossChainCustomersApi CustomersApi { get; private set; }
        
        /// <summary>
        /// Interface for configuration API
        /// </summary>
        public ICrossChainConfigurationApi ConfigurationApi { get; private set; }

        /// <summary>C-tor</summary>
        public CrossChainWalletLinkerClient(IHttpClientGenerator httpClientGenerator)
        {
            WalletLinkingApi = httpClientGenerator.Generate<ICrossChainWalletLinkerApi>();
            CustomersApi = httpClientGenerator.Generate<ICrossChainCustomersApi>();
            ConfigurationApi = httpClientGenerator.Generate<ICrossChainConfigurationApi>();
        }
    }
}
