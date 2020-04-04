namespace MAVN.Service.CrossChainWalletLinker.Contract.Linking
{
    /// <summary>
    /// Wallet linking status change has been finalized (completed in private and public blockchains)
    /// </summary>
    public class WalletLinkingStatusChangeFinalizedEvent
    {
        /// <summary>
        /// The wallet address in private blockchain
        /// </summary>
        public string PrivateAddress { get; set; }

        /// <summary>
        /// The wallet address in public blockchain.
        /// Can be null if it was unlink request.
        /// </summary>
        public string PublicAddress { get; set; }       
    }
}
