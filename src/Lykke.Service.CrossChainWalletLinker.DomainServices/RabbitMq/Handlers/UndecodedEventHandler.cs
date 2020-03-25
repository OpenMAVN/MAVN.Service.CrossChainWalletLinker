using System;
using System.Threading.Tasks;
using Common;
using Common.Log;
using Lykke.Common.Log;
using Lykke.RabbitMqBroker.Publisher;
using Lykke.Service.CrossChainWalletLinker.Contract.Linking;
using Lykke.Service.CrossChainWalletLinker.Domain.Enums;
using Lykke.Service.CrossChainWalletLinker.Domain.Models;
using Lykke.Service.CrossChainWalletLinker.Domain.RabbitMq.Handlers;
using Lykke.Service.CrossChainWalletLinker.Domain.RabbitMq.Publishers;
using Lykke.Service.CrossChainWalletLinker.Domain.Repositories;
using Lykke.Service.CrossChainWalletLinker.Domain.Services;

namespace Lykke.Service.CrossChainWalletLinker.DomainServices.RabbitMq.Handlers
{
    public class UndecodedEventHandler : IUndecodedEventHandler
    {
        private readonly IBlockchainEventDecoder _blockchainEventDecoder;
        private readonly ISettingsService _settingsService;
        private readonly IWalletLinker _walletLinker;
        private readonly IWalletLinkingRequestsRepository _requestsRepository;
        private readonly IPushNotificationsPublisher _pushNotificationsPublisher;
        private readonly IRabbitPublisher<WalletLinkingStatusChangeCompletedEvent> _statusChangeCompletedPublisher;
        private readonly ILog _log;

        public UndecodedEventHandler(IBlockchainEventDecoder blockchainEventDecoder, 
            ISettingsService settingsService,
            ILogFactory logFactory, 
            IWalletLinker walletLinker, 
            IWalletLinkingRequestsRepository requestsRepository, 
            IPushNotificationsPublisher pushNotificationsPublisher,
            IRabbitPublisher<WalletLinkingStatusChangeCompletedEvent> statusChangeCompletedPublisher)
        {
            _blockchainEventDecoder = blockchainEventDecoder;
            _settingsService = settingsService;
            _walletLinker = walletLinker;
            _requestsRepository = requestsRepository;
            _pushNotificationsPublisher = pushNotificationsPublisher;
            _statusChangeCompletedPublisher = statusChangeCompletedPublisher;
            _log = logFactory.CreateLog(this);
        }

        public async Task HandleAsync(string[] topics, string data, string contractAddress)
        {
            //This means that the event is raised by another smart contract and we are not interested in it
            if (!string.Equals(contractAddress, 
                _settingsService.GetPrivateBlockchainGatewayContractAddress(), 
                StringComparison.InvariantCultureIgnoreCase))
            {
                _log.Info("The contract address differs from the expected one. Event handling will be stopped.",
                    new { expected = _settingsService.GetPrivateBlockchainGatewayContractAddress(), current = contractAddress });

                return;
            }

            var eventType = _blockchainEventDecoder.GetEventType(topics[0]);

            switch (eventType)
            {
                case BlockchainEventType.Unknown:
                    return;
                case BlockchainEventType.PublicAccountLinked:
                    await HandleWalletLinkedAsync(topics, data);
                    break;
                case BlockchainEventType.PublicAccountUnlinked:
                    await HandleWalletUnlinkedAsync(topics, data);
                    break;
                default: throw new InvalidOperationException("Unsupported blockchain event type");
            }
        }
        
        private async Task HandleWalletLinkedAsync(string[] topics, string data)
        {
            var publicAccountLinkedDto = _blockchainEventDecoder.DecodePublicAccountLinkedEvent(topics, data);
            
            var confirmationResult = await _walletLinker.ConfirmInPrivateAsync(publicAccountLinkedDto.PrivateAddress);

            if (confirmationResult.Error != ConfirmationError.None)
            {
                _log.Error(message: "Couldn't mark link request as confirmed in private blockchain",
                    context: new
                    {
                        publicAccountLinkedDto.PrivateAddress,
                        publicAccountLinkedDto.PublicAddress,
                        confirmationResult.Error
                    });
            }
        }

        private async Task HandleWalletUnlinkedAsync(string[] topics, string data)
        {
            var publicAccountUnlinkedDto = _blockchainEventDecoder.DecodePublicAccountUnlinkedEvent(topics, data);

            var linkRequest =
                await _requestsRepository.GetByPrivateAddressAsync(publicAccountUnlinkedDto.PrivateAddress);

            if (linkRequest != null)
            {
                await _pushNotificationsPublisher.PublishWalletUnlinkingSuccessfulAsync(linkRequest.CustomerId);
                await _requestsRepository.DeleteByIdAsync(linkRequest.CustomerId);
            }
            else
            {
                _log.Error(message: "Wallet linking request has not been deleted upon confirmation from blockchain",
                    context: new {publicAccountUnlinkedDto.PrivateAddress, publicAccountUnlinkedDto.PublicAddress});
            }
            
            await _statusChangeCompletedPublisher.PublishAsync(new WalletLinkingStatusChangeCompletedEvent
            {
                CustomerId = linkRequest?.CustomerId, 
                PrivateAddress = publicAccountUnlinkedDto.PrivateAddress,
                PublicAddress = null,
                EventId = Guid.NewGuid().ToString(),
                Fee = linkRequest?.Fee ?? 0
            });
            
            _log.Info("Wallet has been unlinked and confirmed in private blockchain", 
                context: publicAccountUnlinkedDto.ToJson());
        }
    }
}
