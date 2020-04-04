using MAVN.Service.CrossChainWalletLinker.Domain.Enums;
using MAVN.Service.CrossChainWalletLinker.Domain.Models;

namespace MAVN.Service.CrossChainWalletLinker.Domain.Services
{
    public interface IBlockchainEventDecoder
    {
        PublicAccountLinkedDto DecodePublicAccountLinkedEvent(string[] topics, string data);

        PublicAccountUnlinkedDto DecodePublicAccountUnlinkedEvent(string[] topics, string data);

        BlockchainEventType GetEventType(string topic);
    }
}
