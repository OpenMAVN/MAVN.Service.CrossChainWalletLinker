namespace Lykke.Service.CrossChainWalletLinker.Domain.Models
{
    public enum ConfirmationError
    {
        None,
        LinkRequestDoesNotExist,
        LinkRequestAlreadyConfirmed,
        LinkRequestHasNotBeenApproved,
        InvalidPrivateAddress,
    }
}
