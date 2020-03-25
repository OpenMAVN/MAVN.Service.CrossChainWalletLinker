using System.Threading.Tasks;

namespace Lykke.Service.CrossChainWalletLinker.Domain.RabbitMq.Handlers
{
    public interface IWalletLinkingStatusChangeCompletedInPublicEventHandler
    {
        Task HandleAsync(string privateAddress, string publicAddress);
    }
}
