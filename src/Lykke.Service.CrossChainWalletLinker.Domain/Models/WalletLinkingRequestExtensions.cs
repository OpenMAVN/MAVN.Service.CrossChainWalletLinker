using System;

namespace Lykke.Service.CrossChainWalletLinker.Domain.Models
{
    public static class WalletLinkingRequestExtensions
    {
        public static bool IsConfirmed(this IWalletLinkingRequest src)
        {
            if (src == null)
                throw new ArgumentNullException(nameof(src));

            return src.IsConfirmedInPrivate && src.IsConfirmedInPublic;
        }

        public static bool IsPendingApproval(this IWalletLinkingRequest src)
        {
            if (src == null)
                throw new ArgumentNullException(nameof(src));

            return string.IsNullOrEmpty(src.PublicAddress) && string.IsNullOrEmpty(src.Signature);
        }

        public static bool IsApproved(this IWalletLinkingRequest src)
        {
            if (src == null)
                throw new ArgumentNullException(nameof(src));
            
            return !string.IsNullOrEmpty(src.PublicAddress) && !string.IsNullOrEmpty(src.Signature);
        }
    }
}
