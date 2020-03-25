namespace Lykke.Service.CrossChainWalletLinker.Domain.Models
{
    public interface IWalletLinkingRequestsCounter
    {
        string CustomerId { get; set; }
        
        int ApprovalsCounter { get; set; }
    }
}
