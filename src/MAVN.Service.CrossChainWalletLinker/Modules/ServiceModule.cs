using Autofac;
using JetBrains.Annotations;
using Lykke.Common.MsSql;
using Lykke.Sdk;
using MAVN.Service.CrossChainWalletLinker.Domain.RabbitMq.Handlers;
using MAVN.Service.CrossChainWalletLinker.Domain.Services;
using MAVN.Service.CrossChainWalletLinker.DomainServices;
using MAVN.Service.CrossChainWalletLinker.DomainServices.RabbitMq.Handlers;
using MAVN.Service.CrossChainWalletLinker.Services;
using MAVN.Service.CrossChainWalletLinker.Settings;
using Lykke.SettingsReader;

namespace MAVN.Service.CrossChainWalletLinker.Modules
{
    [UsedImplicitly]
    public class ServiceModule : Module
    {
        private readonly IReloadingManager<AppSettings> _appSettings;

        public ServiceModule(IReloadingManager<AppSettings> appSettings)
        {
            _appSettings = appSettings;
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<StartupManager>()
                .As<IStartupManager>()
                .SingleInstance();
            
            builder.RegisterType<ShutdownManager>()
                .As<IShutdownManager>()
                .SingleInstance();
            
            builder.RegisterType<SettingsService>()
                .As<ISettingsService>()
                .WithParameter("privateBlockchainGatewayContractAddress",
                    _appSettings.CurrentValue.CrossChainWalletLinkerService.PrivateBlockchainGatewayContractAddress)
                .WithParameter("masterWalletAddress",
                    _appSettings.CurrentValue.CrossChainWalletLinkerService.MasterWalletAddress)
                .SingleInstance();
            
            builder.RegisterType<WalletLinker>()
                .WithParameter(TypedParameter.From(_appSettings.CurrentValue.CrossChainWalletLinkerService.LinkCodeLength))
                .As<IWalletLinker>();

            builder.RegisterType<CustomersService>()
                .As<ICustomersService>();

            builder.RegisterType<BlockchainEventDecoder>()
                .As<IBlockchainEventDecoder>()
                .SingleInstance();
            
            builder.RegisterType<UndecodedEventHandler>()
                .As<IUndecodedEventHandler>()
                .SingleInstance();

            builder.RegisterType<WalletLinkingStatusChangeCompletedInPublicEventHandler>()
                .As<IWalletLinkingStatusChangeCompletedInPublicEventHandler>()
                .SingleInstance();
            
            builder.RegisterType<WalletStatusChangeFailedEventHandler>()
                .As<IWalletStatusChangeFailedEventHandler>()
                .SingleInstance();

            builder.RegisterType<SignatureValidator>()
                .As<ISignatureValidator>();

            builder.RegisterType<NotificationsSettingsService>()
                .WithParameter("walletLinkingSuccessfulTemplateId",
                    _appSettings.CurrentValue.CrossChainWalletLinkerService.Notifications.PushNotifications.WalletLinkingSuccessfulTemplateId)
                .WithParameter("walletLinkingUnsuccessfulTemplateId",
                    _appSettings.CurrentValue.CrossChainWalletLinkerService.Notifications.PushNotifications.WalletLinkingUnsuccessfulTemplateId)
                .WithParameter("walletUnlinkingSuccessfulTemplateId",
                    _appSettings.CurrentValue.CrossChainWalletLinkerService.Notifications.PushNotifications.WalletUnlinkingSuccessfulTemplateId)
                .WithParameter("walletUnlinkingUnsuccessfulTemplateId",
                    _appSettings.CurrentValue.CrossChainWalletLinkerService.Notifications.PushNotifications.WalletUnlinkingUnsuccessfulTemplateId)
                .As<INotificationsSettingsService>()
                .SingleInstance();
        }
    }
}
