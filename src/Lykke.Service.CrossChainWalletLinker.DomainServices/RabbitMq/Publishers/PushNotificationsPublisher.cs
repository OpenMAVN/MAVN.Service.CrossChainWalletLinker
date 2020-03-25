using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Common;
using Lykke.Common.Log;
using Lykke.RabbitMqBroker.Publisher;
using Lykke.Service.CrossChainWalletLinker.Domain.RabbitMq.Publishers;
using Lykke.Service.CrossChainWalletLinker.Domain.Services;
using Lykke.Service.NotificationSystem.SubscriberContract;

namespace Lykke.Service.CrossChainWalletLinker.DomainServices.RabbitMq.Publishers
{
    public class PushNotificationsPublisher : JsonRabbitPublisher<PushNotificationEvent>, IPushNotificationsPublisher
    {
        private readonly INotificationsSettingsService _notificationsSettingsService;

        public PushNotificationsPublisher(
            INotificationsSettingsService notificationsSettingsService,
            ILogFactory logFactory,
            string connectionString,
            string exchangeName)
            : base(logFactory, connectionString, exchangeName)
        {
            _notificationsSettingsService = notificationsSettingsService;
        }

        public Task PublishWalletLinkingSuccessfulAsync(string customerId)
            => PublishNotificationAsync(customerId, _notificationsSettingsService.WalletLinkingSuccessfulTemplateId);

        public Task PublishWalletLinkingUnsuccessfulAsync(string customerId)
            => PublishNotificationAsync(customerId, _notificationsSettingsService.WalletLinkingUnsuccessfulTemplateId);

        public Task PublishWalletUnlinkingSuccessfulAsync(string customerId)
            => PublishNotificationAsync(customerId, _notificationsSettingsService.WalletUnlinkingSuccessfulTemplateId);

        public Task PublishWalletUnlinkingUnsuccessfulAsync(string customerId)
            => PublishNotificationAsync(customerId, _notificationsSettingsService.WalletUnlinkingUnsuccessfulTemplateId);

        private Task PublishNotificationAsync(string customerId, string templateId)
        {
            return PublishAsync(new PushNotificationEvent
            {
                CustomerId = customerId,
                Source = $"{AppEnvironment.Name} - {AppEnvironment.Version}",
                MessageTemplateId = templateId,
                CustomPayload = new Dictionary<string, string> { { "route", "wallet" } }
            });
        }

    }
}
