﻿using Autofac;
using JetBrains.Annotations;
using MAVN.Service.CrossChainWalletLinker.MsSqlRepositories;
using MAVN.Service.CrossChainWalletLinker.Domain.Repositories;
using MAVN.Service.CrossChainWalletLinker.Settings;
using Lykke.SettingsReader;
using MAVN.Persistence.PostgreSQL.Legacy;

namespace MAVN.Service.CrossChainWalletLinker.Modules
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
            builder.RegisterPostgreSQL(
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
