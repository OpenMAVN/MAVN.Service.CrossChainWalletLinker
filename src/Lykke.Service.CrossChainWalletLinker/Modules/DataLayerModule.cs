using Autofac;
using JetBrains.Annotations;
using Lykke.Common.MsSql;
using Lykke.Service.CrossChainWalletLinker.MsSqlRepositories;
using Lykke.Service.CrossChainWalletLinker.Domain.Repositories;
using Lykke.Service.CrossChainWalletLinker.Settings;
using Lykke.SettingsReader;

namespace Lykke.Service.CrossChainWalletLinker.Modules
{
    [UsedImplicitly]
    public class DataLayerModule : Module
    {
        private readonly IReloadingManager<AppSettings> _appSettings;

        public DataLayerModule(IReloadingManager<AppSettings> appSettings)
        {
            _appSettings = appSettings;
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterMsSql(
                _appSettings.CurrentValue.CrossChainWalletLinkerService.Db.DataConnString,
                connString => new WalletLinkingContext(connString, false),
                dbConn => new WalletLinkingContext(dbConn));
            
            builder.RegisterType<WalletLinkingRequestsRepository>()
                .As<IWalletLinkingRequestsRepository>()
                .SingleInstance();
            
            builder.RegisterType<WalletLinkingRequestsCounterRepository>()
                .As<IWalletLinkingRequestsCounterRepository>()
                .SingleInstance();

            builder.RegisterType<ConfigurationItemsRepository>()
                .As<IConfigurationItemsRepository>()
                .SingleInstance();
        }
    }
}
