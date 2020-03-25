using System.Threading.Tasks;

namespace Lykke.Service.CrossChainWalletLinker.Domain.RabbitMq.Handlers
{
    public interface IWalletStatusChangeFailedEventHandler
    {
        Task HandleAsync(string privateAddress, string publicAddress);
    }
}
