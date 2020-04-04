using JetBrains.Annotations;
using Lykke.Sdk.Settings;
using Lykke.Service.PrivateBlockchainFacade.Client;
using Lykke.Service.WalletManagement.Client;

namespace MAVN.Service.CrossChainWalletLinker.Settings
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class AppSettings : BaseAppSettings
    {
        public CrossChainWalletLinkerSettings CrossChainWalletLinkerService { get; set; }
        public PrivateBlockchainFacadeServiceClientSettings PrivateBlockchainFacadeService { get; set; }
        public WalletManagementServiceClientSettings WalletManagementService { get; set; }
    }
}
