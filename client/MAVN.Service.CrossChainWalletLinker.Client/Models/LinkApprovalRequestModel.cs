using System.ComponentModel.DataAnnotations;
using JetBrains.Annotations;

namespace MAVN.Service.CrossChainWalletLinker.Client.Models
{
    /// <summary>
    /// The wallet linking request approval
    /// </summary>
    [PublicAPI]
    public class LinkApprovalRequestModel
    {
        /// <summary>
        /// The private address
        /// </summary>
        [Required]
        public string PrivateAddress { get; set; }
        
        /// <summary>
        /// The public address to link to
        /// </summary>
        [Required]
        public string PublicAddress { get; set; }
        
        /// <summary>
        /// The signed link code with private address
        /// </summary>
        [Required]
        public string Signature { get; set; }
    }
}
