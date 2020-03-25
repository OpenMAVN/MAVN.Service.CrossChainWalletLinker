using System;
using System.Threading.Tasks;
using Common.Log;
using Lykke.Common.Log;
using Lykke.RabbitMqBroker.Publisher;
using Lykke.Service.CrossChainWalletLinker.Contract.Linking;
using Lykke.Service.CrossChainWalletLinker.Domain.RabbitMq.Handlers;
using Lykke.Service.CrossChainWalletLinker.Domain.RabbitMq.Publishers;
using Lykke.Service.CrossChainWalletLinker.Domain.Repositories;
using MongoDB.Bson;

namespace Lykke.Service.CrossChainWalletLinker.DomainServices.RabbitMq.Handlers
{
    public class WalletStatusChangeFailedEventHandler : IWalletStatusChangeFailedEventHandler
    {
        private readonly IWalletLinkingRequestsRepository _requestsRepository;
        private readonly IRabbitPublisher<WalletLinkingStatusChangeFailedEvent> _failurePublisher;
        private readonly IPushNotificationsPublisher _pushNotificationsPublisher;
        private readonly ILog _log;

        public WalletStatusChangeFailedEventHandler(ILogFactory logFactory,
            IWalletLinkingRequestsRepository requestsRepository,
            IRabbitPublisher<WalletLinkingStatusChangeFailedEvent> failurePublisher,
            IPushNotificationsPublisher pushNotificationsPublisher)
        {
            _requestsRepository = requestsRepository;
            _failurePublisher = failurePublisher;
            _pushNotificationsPublisher = pushNotificationsPublisher;
            _log = logFactory.CreateLog(this);
        }

        public async Task HandleAsync(string privateAddress, string publicAddress)
        {
            var linkingRequest = await _requestsRepository.GetByPrivateAddressAsync(privateAddress);

            if (string.IsNullOrEmpty(publicAddress))
            {
                // it was unlink operation
                _log.Error(message: "Wallet unlinking operation failed", context: new {privateAddress});

                if (linkingRequest != null)
                    await _pushNotificationsPublisher.PublishWalletUnlinkingUnsuccessfulAsync(linkingRequest.CustomerId);
            }
            else
            {
                // it was link operation
                _log.Error(message: "Wallet linking operation failed", context: new {privateAddress, publicAddress});

                if (linkingRequest != null)
                {
                    await _requestsRepository.DeleteByIdAsync(linkingRequest.CustomerId);
                    await _pushNotificationsPublisher.PublishWalletLinkingUnsuccessfulAsync(linkingRequest.CustomerId);

                    _log.Info("The linking request has been deleted because the corresponding transaction failed",
                        context: linkingRequest.ToJson());
                }
            }

            await _failurePublisher.PublishAsync(new WalletLinkingStatusChangeFailedEvent
            {
                CustomerId = linkingRequest?.CustomerId,
                PrivateAddress = privateAddress,
                PublicAddress = publicAddress,
                Fee = linkingRequest?.Fee ?? 0,
                EventId = Guid.NewGuid().ToString()
            });
        }
    }
}
