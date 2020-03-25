using System.Threading.Tasks;

namespace Lykke.Service.CrossChainWalletLinker.Domain.RabbitMq.Handlers
{
    public interface IUndecodedEventHandler
    {
        Task HandleAsync(string[] topics, string data, string contractAddress);
    }
}
