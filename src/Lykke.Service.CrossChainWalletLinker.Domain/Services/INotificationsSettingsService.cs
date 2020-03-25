namespace Lykke.Service.CrossChainWalletLinker.Domain.Services
{
    public interface INotificationsSettingsService
    {
        string WalletLinkingSuccessfulTemplateId { get; }
        string WalletLinkingUnsuccessfulTemplateId { get; }
        string WalletUnlinkingSuccessfulTemplateId { get; }
        string WalletUnlinkingUnsuccessfulTemplateId { get; }
    }
}
