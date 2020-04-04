namespace MAVN.Service.CrossChainWalletLinker.Domain.Models
{
    public enum LinkingError
    {
        None,
        InvalidCustomerId,
        LinkingRequestAlreadyExists,
        CustomerWalletMissing,
        LinkingRequestDoesNotExist,
        InvalidPublicAddress,
        InvalidSignature,
        LinkingRequestAlreadyApproved,
        InvalidPrivateAddress,
        CannotDeleteLinkingRequestWhileConfirming,
        NotEnoughFunds,
        CustomerDoesNotExist,
        CustomerWalletBlocked,
    }
}
