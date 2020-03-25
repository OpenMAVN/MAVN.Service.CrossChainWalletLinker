using Autofac;
using Common;
using JetBrains.Annotations;
using Lykke.Job.EthereumBridge.Contract;
using Lykke.Job.QuorumTransactionWatcher.Contract;
using Lykke.RabbitMqBroker.Publisher;
using Lykke.RabbitMqBroker.Subscriber;
using Lykke.Service.CrossChainWalletLinker.Contract.Linking;
using Lykke.Service.CrossChainWalletLinker.Domain.RabbitMq.Publishers;
using Lykke.Service.CrossChainWalletLinker.DomainServices.RabbitMq.Publishers;
using Lykke.Service.CrossChainWalletLinker.DomainServices.RabbitMq.Subscribers;
using Lykke.Service.CrossChainWalletLinker.Settings;
using Lykke.Service.PrivateBlockchainFacade.Contract.Events;
using Lykke.SettingsReader;

namespace Lykke.Service.CrossChainWalletLinker.Modules
{
    [UsedImplicitly]
    public class RabbitMqModule : Module
    {
        private readonly IReloadingManager<AppSettings> _appSettings;
        
        private const string WalletLinkingStatusChangeRequestedExchangeName = "lykke.wallet.walletlinkingstatuschangerequested";
        private const string WalletLinkingStatusChangeCompletedExchangeName = "lykke.wallet.walletlinkingstatuschangecompleted";
        private const string EthereumWalletLinkingStatusChangeCompletedExchangeName = "lykke.wallet.ethereumwalletlinkingstatuschangecompleted";
        private const string WalletLinkingStatusChangeFinalizedExchangeName = "lykke.wallet.walletlinkingstatuschangefinalized";
        // event from blockchain layer
        private const string WalletStatusChangeFailedExchangeName = "lykke.wallet.walletstatuschangefailed";
        // business event
        private const string WalletLinkingStatusChangeFailedExchangeName = "lykke.wallet.walletlinkingstatuschangefailed";
        private const string NotificationSystemPushNotificationsExchangeName = "notificationsystem.command.pushnotification";

        private const string DefaultQueueName = "crosschainwalletlinker";

        public RabbitMqModule(IReloadingManager<AppSettings> appSettings)
        {
            _appSettings = appSettings;
        }

        protected override void Load(ContainerBuilder builder)
        {
            var rabbitMqConnString =
                _appSettings.CurrentValue.CrossChainWalletLinkerService.RabbitMq.RabbitMqConnectionString;
            
            builder.RegisterType<JsonRabbitPublisher<WalletLinkingStatusChangeRequestedEvent>>()
                .As<IRabbitPublisher<WalletLinkingStatusChangeRequestedEvent>>()
                .WithParameter("connectionString", rabbitMqConnString)
                .WithParameter("exchangeName", WalletLinkingStatusChangeRequestedExchangeName)
                .As<IStartable>()
                .As<IStopable>()
                .SingleInstance();
            
            builder.RegisterType<JsonRabbitPublisher<WalletLinkingStatusChangeCompletedEvent>>()
                .As<IRabbitPublisher<WalletLinkingStatusChangeCompletedEvent>>()
                .WithParameter("connectionString", rabbitMqConnString)
                .WithParameter("exchangeName", WalletLinkingStatusChangeCompletedExchangeName)
                .As<IStartable>()
                .As<IStopable>()
                .SingleInstance();

            builder.RegisterType<JsonRabbitPublisher<WalletLinkingStatusChangeFinalizedEvent>>()
                .As<IRabbitPublisher<WalletLinkingStatusChangeFinalizedEvent>>()
                .WithParameter("connectionString", rabbitMqConnString)
                .WithParameter("exchangeName", WalletLinkingStatusChangeFinalizedExchangeName)
                .As<IStartable>()
                .As<IStopable>()
                .SingleInstance();
            
            builder.RegisterType<JsonRabbitPublisher<WalletLinkingStatusChangeFailedEvent>>()
                .As<IRabbitPublisher<WalletLinkingStatusChangeFailedEvent>>()
                .WithParameter("connectionString", rabbitMqConnString)
                .WithParameter("exchangeName", WalletLinkingStatusChangeFailedExchangeName)
                .As<IStartable>()
                .As<IStopable>()
                .SingleInstance();
            
            builder.RegisterType<UndecodedSubscriber>()
                .As<JsonRabbitSubscriber<UndecodedEvent>>()
                .As<IStopable>()
                .WithParameter("connectionString", rabbitMqConnString)
                .WithParameter("exchangeName", Context.GetEndpointName<UndecodedEvent>())
                .WithParameter("queueName", DefaultQueueName)
                .SingleInstance();
            
            builder.RegisterType<WalletLinkingCompletedInPublicSubscriber>()
                .As<JsonRabbitSubscriber<EthereumWalletLinkingStatusChangeCompletedEvent>>()
                .As<IStopable>()
                .WithParameter("connectionString", rabbitMqConnString)
                .WithParameter("exchangeName", EthereumWalletLinkingStatusChangeCompletedExchangeName)
                .WithParameter("queueName", DefaultQueueName)
                .SingleInstance();
            
            builder.RegisterType<WalletStatusChangeFailedSubscriber>()
                .As<JsonRabbitSubscriber<WalletStatusChangeFailedEvent>>()
                .As<IStopable>()
                .WithParameter("connectionString", rabbitMqConnString)
                .WithParameter("exchangeName", WalletStatusChangeFailedExchangeName)
                .WithParameter("queueName", DefaultQueueName)
                .SingleInstance();

            builder.RegisterType<PushNotificationsPublisher>()
                .WithParameter("connectionString", _appSettings.CurrentValue.CrossChainWalletLinkerService.RabbitMq.NotificationRabbitMqConnectionString)
                .WithParameter("exchangeName", NotificationSystemPushNotificationsExchangeName)
                .As<IPushNotificationsPublisher>()
                .As<IStartable>()
                .SingleInstance();
        }
    }
}
