using Autofac;
using JetBrains.Annotations;
using MAVN.Service.CrossChainWalletLinker.Settings;
using MAVN.Service.PrivateBlockchainFacade.Client;
using MAVN.Service.WalletManagement.Client;
using Lykke.SettingsReader;

namespace MAVN.Service.CrossChainWalletLinker.Modules
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
