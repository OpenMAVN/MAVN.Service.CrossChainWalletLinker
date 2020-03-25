using Lykke.Service.CrossChainWalletLinker.Domain.Services;

namespace Lykke.Service.CrossChainWalletLinker.DomainServices
{
    public class NotificationsSettingsService : INotificationsSettingsService
    {
        public NotificationsSettingsService(
            string walletLinkingSuccessfulTemplateId,
            string walletLinkingUnsuccessfulTemplateId,
            string walletUnlinkingSuccessfulTemplateId,
            string walletUnlinkingUnsuccessfulTemplateId)
        {
            WalletLinkingSuccessfulTemplateId = walletLinkingSuccessfulTemplateId;
            WalletLinkingUnsuccessfulTemplateId = walletLinkingUnsuccessfulTemplateId;
            WalletUnlinkingSuccessfulTemplateId = walletUnlinkingSuccessfulTemplateId;
            WalletUnlinkingUnsuccessfulTemplateId = walletUnlinkingUnsuccessfulTemplateId;
        }

        public string WalletLinkingSuccessfulTemplateId { get; }
        public string WalletLinkingUnsuccessfulTemplateId { get; }
        public string WalletUnlinkingSuccessfulTemplateId { get; }
        public string WalletUnlinkingUnsuccessfulTemplateId { get; }
    }
}
