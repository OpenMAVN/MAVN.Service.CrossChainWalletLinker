using System.Threading.Tasks;
using Common.Log;
using Lykke.Common.Log;
using Lykke.RabbitMqBroker.Publisher;
using MAVN.Service.CrossChainWalletLinker.Contract.Linking;
using MAVN.Service.CrossChainWalletLinker.Domain.Models;
using MAVN.Service.CrossChainWalletLinker.Domain.RabbitMq.Handlers;
using MAVN.Service.CrossChainWalletLinker.Domain.RabbitMq.Publishers;
using MAVN.Service.CrossChainWalletLinker.Domain.Services;

namespace MAVN.Service.CrossChainWalletLinker.DomainServices.RabbitMq.Handlers
{
    public class WalletLinkingStatusChangeCompletedInPublicEventHandler : IWalletLinkingStatusChangeCompletedInPublicEventHandler
    {
        private readonly IWalletLinker _walletLinker;
        private readonly IRabbitPublisher<WalletLinkingStatusChangeFinalizedEvent> _publisher;
        private readonly IPushNotificationsPublisher _pushNotificationsPublisher;
        private readonly ILog _log;

        public WalletLinkingStatusChangeCompletedInPublicEventHandler(IWalletLinker walletLinker,
            ILogFactory logFactory,
            IRabbitPublisher<WalletLinkingStatusChangeFinalizedEvent> publisher,
            IPushNotificationsPublisher pushNotificationsPublisher)
        {
            _walletLinker = walletLinker;
            _publisher = publisher;
            _pushNotificationsPublisher = pushNotificationsPublisher;
            _log = logFactory.CreateLog(this);
        }

        public async Task HandleAsync(string privateAddress, string publicAddress)
        {
            if (string.IsNullOrEmpty(publicAddress))
            {
                // it is unlink completion

                await _publisher.PublishAsync(new WalletLinkingStatusChangeFinalizedEvent
                {
                    PrivateAddress = privateAddress,
                    PublicAddress = publicAddress
                });

                _log.Info("Unlink request has been confirmed in public blockchain",
                    new { privateAddress, publicAddress });
                return;
            }

            var linkingRequest = await _walletLinker.GetByPrivateAddressAsync(privateAddress);

            if (linkingRequest == null)
            {
                _log.Error(message: "The wallet linking request was not found", context: new { privateAddress, publicAddress });

                return;
            }

            // it is link completion
            var confirmationResult = await _walletLinker.ConfirmInPublicAsync(privateAddress);

            if (confirmationResult.Error != ConfirmationError.None)
            {
                _log.Error(message: "Couldn't mark link request as confirmed in public blockchain",
                    context: new { privateAddress, publicAddress, confirmationResult.Error });

                return;
            }

            await _pushNotificationsPublisher.PublishWalletLinkingSuccessfulAsync(linkingRequest.CustomerId);


            await _publisher.PublishAsync(new WalletLinkingStatusChangeFinalizedEvent
            {
                PrivateAddress = privateAddress,
                PublicAddress = publicAddress
            });
        }
    }
}
