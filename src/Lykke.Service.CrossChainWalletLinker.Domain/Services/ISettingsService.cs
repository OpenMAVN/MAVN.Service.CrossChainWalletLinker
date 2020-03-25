namespace Lykke.Service.CrossChainWalletLinker.Domain.Services
{
    public interface ISettingsService
    {
        string GetMasterWalletAddress();

        string GetPrivateBlockchainGatewayContractAddress();
    }
}
