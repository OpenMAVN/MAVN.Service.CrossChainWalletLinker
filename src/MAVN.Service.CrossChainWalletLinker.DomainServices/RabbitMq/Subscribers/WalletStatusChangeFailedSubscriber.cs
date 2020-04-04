using System.Threading.Tasks;
using Common.Log;
using Lykke.Common.Log;
using Lykke.RabbitMqBroker.Subscriber;
using MAVN.Service.CrossChainWalletLinker.Domain.RabbitMq.Handlers;
using Lykke.Service.PrivateBlockchainFacade.Contract.Events;

namespace MAVN.Service.CrossChainWalletLinker.DomainServices.RabbitMq.Subscribers
{
    public class WalletStatusChangeFailedSubscriber : JsonRabbitSubscriber<WalletStatusChangeFailedEvent>
    {
        private readonly IWalletStatusChangeFailedEventHandler _walletStatusChangeFailedEventHandler;
        private readonly ILog _log;
        
        public WalletStatusChangeFailedSubscriber(
            string connectionString, 
            string exchangeName, 
            string queueName, 
            ILogFactory logFactory, 
            IWalletStatusChangeFailedEventHandler walletStatusChangeFailedEventHandler) 
            : base(connectionString, exchangeName, queueName, logFactory)
        {
            _walletStatusChangeFailedEventHandler = walletStatusChangeFailedEventHandler;
            _log = logFactory.CreateLog(this);
        }

        protected override async Task ProcessMessageAsync(WalletStatusChangeFailedEvent message)
        {
            await _walletStatusChangeFailedEventHandler.HandleAsync(message.InternalWalletAddress, message.PublicWalletAddress);
            _log.Info($"Handled {nameof(WalletStatusChangeFailedEvent)} event", message);
        }
    }
}
