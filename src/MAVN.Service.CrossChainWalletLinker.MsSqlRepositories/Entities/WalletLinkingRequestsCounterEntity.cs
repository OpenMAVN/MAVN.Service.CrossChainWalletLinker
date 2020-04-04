using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MAVN.Service.CrossChainWalletLinker.Domain.Models;

namespace MAVN.Service.CrossChainWalletLinker.MsSqlRepositories.Entities
{
    [Table("wallet_linking_requests_counter")]
    public class WalletLinkingRequestsCounterEntity : IWalletLinkingRequestsCounter
    {
        [Key, Required]
        [Column("customer_id")]
        public string CustomerId { get; set; }
        
        [Column("approvals_counter")]
        [DefaultValue(0)]
        public int ApprovalsCounter { get; set; }

        public static WalletLinkingRequestsCounterEntity Create(string customerId, int approvals)
        {
            return new WalletLinkingRequestsCounterEntity
            {
                CustomerId = customerId,
                ApprovalsCounter = approvals
            };
        }
    }
}
