using Lykke.SettingsReader.Attributes;

namespace MAVN.Service.CrossChainWalletLinker.Client 
{
    /// <summary>
    /// CrossChainWalletLinker client settings.
    /// </summary>
    public class CrossChainWalletLinkerServiceClientSettings 
    {
        /// <summary>Service url.</summary>
        [HttpCheck("api/isalive")]
        public string ServiceUrl {get; set;}
    }
}
