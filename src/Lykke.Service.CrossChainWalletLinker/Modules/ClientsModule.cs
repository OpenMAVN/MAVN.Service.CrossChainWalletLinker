using Autofac;
using JetBrains.Annotations;
using Lykke.Service.CrossChainWalletLinker.Settings;
using Lykke.Service.PrivateBlockchainFacade.Client;
using Lykke.Service.WalletManagement.Client;
using Lykke.SettingsReader;

namespace Lykke.Service.CrossChainWalletLinker.Modules
{
    [UsedImplicitly]
    public class ClientsModule : Module
    {
        private readonly IReloadingManager<AppSettings> _appSettings;

        public ClientsModule(IReloadingManager<AppSettings> appSettings)
        {
            _appSettings = appSettings;
        }
        
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterPrivateBlockchainFacadeClient(_appSettings.CurrentValue.PrivateBlockchainFacadeService, null);
            builder.RegisterWalletManagementClient(_appSettings.CurrentValue.WalletManagementService, null);
        }
    }
}
