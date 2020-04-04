using Falcon.Numerics;
using JetBrains.Annotations;

namespace MAVN.Service.CrossChainWalletLinker.Client.Models
{
    /// <summary>
    /// The next wallet linking fee response model
    /// </summary>
    [PublicAPI]
    public class NextFeeResponseModel
    {
        /// <summary>
        /// The customer id
        /// </summary>
        public string CustomerId { get; set; }
        
        /// <summary>
        /// The fee value
        /// </summary>
        public Money18 Fee { get; set; }
    }
}
