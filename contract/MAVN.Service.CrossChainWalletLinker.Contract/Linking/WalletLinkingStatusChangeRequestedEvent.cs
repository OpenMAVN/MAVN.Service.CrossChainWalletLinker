using MAVN.Numerics;

namespace MAVN.Service.CrossChainWalletLinker.Contract.Linking
{
    /// <summary>
    /// The command to link/unlink internal wallet with external wallet
    /// </summary>
    public class WalletLinkingStatusChangeRequestedEvent
    {
        /// <summary>
        /// The customer id
        /// </summary>
        public string CustomerId { get; set; }
        
        /// <summary>
        /// The customer private wallet address
        /// </summary>
        public string PrivateAddress { get; set; }
        
        /// <summary>
        /// The customer public wallet address
        /// </summary>
        public string PublicAddress { get; set; }
        
        /// <summary>
        /// The wallet linking fee value
        /// </summary>
        public Money18 Fee { get; set; }
        
        /// <summary>
        /// The master wallet address
        /// </summary>
        public string MasterWalletAddress { get; set; }

        /// <summary>
        /// The unique event identifier, to be used to handle deduplication etc.
        /// </summary>
        public string EventId { get; set; }
        
        /// <summary>
        /// The linking direction
        /// </summary>
        public LinkingDirection Direction { get; set; }
    }
}
