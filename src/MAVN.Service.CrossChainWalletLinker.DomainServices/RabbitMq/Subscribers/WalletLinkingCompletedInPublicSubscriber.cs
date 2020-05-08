using System.Threading.Tasks;
using Common.Log;
using Lykke.Common.Log;
using MAVN.Job.EthereumBridge.Contract;
using Lykke.RabbitMqBroker.Subscriber;
using MAVN.Service.CrossChainWalletLinker.Domain.RabbitMq.Handlers;

namespace MAVN.Service.CrossChainWalletLinker.DomainServices.RabbitMq.Subscribers
{
    public class WalletLinkingCompletedInPublicSubscriber : JsonRabbitSubscriber<EthereumWalletLinkingStatusChangeCompletedEvent>
    {
        private readonly IWalletLinkingStatusChangeCompletedInPublicEventHandler _walletLinkingStatusChangeCompletedInPublicEventHandler;
        private readonly ILog _log;

        public WalletLinkingCompletedInPublicSubscriber(
            string connectionString,
            string exchangeName,
            string queueName,
            ILogFactory logFactory,
            IWalletLinkingStatusChangeCompletedInPublicEventHandler walletLinkingStatusChangeCompletedInPublicEventHandler) 
            : base(connectionString, exchangeName, queueName, logFactory)
        {
            _walletLinkingStatusChangeCompletedInPublicEventHandler = walletLinkingStatusChangeCompletedInPublicEventHandler;
            _log = logFactory.CreateLog(this);
        }

        protected override async Task ProcessMessageAsync(EthereumWalletLinkingStatusChangeCompletedEvent message)
        {
            await _walletLinkingStatusChangeCompletedInPublicEventHandler.HandleAsync(message.PrivateAddress, message.PublicAddress);
            _log.Info($"Handled {nameof(EthereumWalletLinkingStatusChangeCompletedEvent)} event", message);
        }
    }
}
