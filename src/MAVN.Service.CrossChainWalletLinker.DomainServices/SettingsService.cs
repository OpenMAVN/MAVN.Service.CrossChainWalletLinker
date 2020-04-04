using MAVN.Service.CrossChainWalletLinker.Domain.Services;

namespace MAVN.Service.CrossChainWalletLinker.DomainServices
{
    public class SettingsService : ISettingsService
    {
        private readonly string _privateBlockchainGatewayContractAddress;
        private readonly string _masterWalletAddress;

        public SettingsService(
            string privateBlockchainGatewayContractAddress,
            string masterWalletAddress)
        {
            _privateBlockchainGatewayContractAddress = privateBlockchainGatewayContractAddress;
            _masterWalletAddress = masterWalletAddress;
        }

        public string GetMasterWalletAddress()
            => _masterWalletAddress;

        public string GetPrivateBlockchainGatewayContractAddress()
            => _privateBlockchainGatewayContractAddress;
    }
}
