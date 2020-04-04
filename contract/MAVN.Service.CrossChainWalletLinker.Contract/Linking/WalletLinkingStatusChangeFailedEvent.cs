using Falcon.Numerics;

namespace MAVN.Service.CrossChainWalletLinker.Contract.Linking
{
    /// <summary>
    /// Wallet linking status change has failed in private blockchain
    /// </summary>
    public class WalletLinkingStatusChangeFailedEvent
    {
        /// <summary>
        /// The customer id
        /// </summary>
        public string CustomerId { get; set; }
        
        /// <summary>
        /// The wallet address in private blockchain
        /// </summary>
        public string PrivateAddress { get; set; }
        
        /// <summary>
        /// The wallet address in public blockchain.
        /// Can be null if it was unlink request.
        /// </summary>
        public string PublicAddress { get; set; }
        
        /// <summary>
        /// The unique event identifier, to be used to handle deduplication etc.
        /// </summary>
        public string EventId { get; set; }
        
        /// <summary>
        /// The fee value for wallet linking/unlinking
        /// </summary>
        public Money18 Fee { get; set; }
    }
}
