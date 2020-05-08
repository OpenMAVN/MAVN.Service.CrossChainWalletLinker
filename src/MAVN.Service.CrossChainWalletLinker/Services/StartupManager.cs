using System.Threading.Tasks;
using MAVN.Job.EthereumBridge.Contract;
using MAVN.Job.QuorumTransactionWatcher.Contract;
using Lykke.RabbitMqBroker.Subscriber;
using Lykke.Sdk;
using MAVN.Service.PrivateBlockchainFacade.Contract.Events;

namespace MAVN.Service.CrossChainWalletLinker.Services
{
    public class StartupManager : IStartupManager
    {
        private readonly JsonRabbitSubscriber<UndecodedEvent> _undecodedSub;
        private readonly JsonRabbitSubscriber<EthereumWalletLinkingStatusChangeCompletedEvent> _ethereumWalletLinkingCompletedSub;
        private readonly JsonRabbitSubscriber<WalletStatusChangeFailedEvent> _walletStatusChangeFailedSub;

        public StartupManager(
            JsonRabbitSubscriber<UndecodedEvent> undecodedSub, 
            JsonRabbitSubscriber<EthereumWalletLinkingStatusChangeCompletedEvent> ethereumWalletLinkingCompletedSub,
            JsonRabbitSubscriber<WalletStatusChangeFailedEvent> walletStatusChangeFailedSub)
        {
            _undecodedSub = undecodedSub;
            _ethereumWalletLinkingCompletedSub = ethereumWalletLinkingCompletedSub;
            _walletStatusChangeFailedSub = walletStatusChangeFailedSub;
        }
        public Task StartAsync()
        {
            _undecodedSub.Start();
            
            _ethereumWalletLinkingCompletedSub.Start();
            
            _walletStatusChangeFailedSub.Start();

            return Task.CompletedTask;
        }
    }
}
