using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Falcon.Numerics;
using MAVN.Service.CrossChainWalletLinker.Domain.Models;

namespace MAVN.Service.CrossChainWalletLinker.MsSqlRepositories.Entities
{
    [Table("wallet_linking_requests")]
    public class WalletLinkingRequestEntity : IWalletLinkingRequest
    {
        [Key, Required]
        [Column("customer_id")]
        public string CustomerId { get; set; }

        [Column("public_address")] 
        public string PublicAddress { get; set; }

        [Required] 
        [Column("private_address")] 
        public string PrivateAddress { get; set; }

        [Required] 
        [Column("link_code")] 
        public string LinkCode { get; set; }

        [Column("is_confirmed_in_private")]
        [DefaultValue(false)]
        public bool IsConfirmedInPrivate { get; set; }

        [Column("is_confirmed_in_public")]
        [DefaultValue(false)]
        public bool IsConfirmedInPublic { get; set; }

        [Column("signature")] 
        public string Signature { get; set; }

        [Required] 
        [Column("created_on")] 
        public DateTime CreatedOn { get; set; }

        [Required] 
        [Column("timestamp")] 
        public DateTime Timestamp { get; set; }
        
        [Column("fee")] 
        public Money18? Fee { get; set; }

        public static WalletLinkingRequestEntity Create(string customerId, 
            string privateAddress,
            string linkCode)
        {
            var now = DateTime.UtcNow;

            return new WalletLinkingRequestEntity
            {
                CustomerId = customerId,
                PublicAddress = null,
                PrivateAddress = privateAddress,
                LinkCode = linkCode,
                IsConfirmedInPrivate = false,
                IsConfirmedInPublic = false,
                CreatedOn = now,
                Timestamp = now,
                Signature = null
            };
        }
    }
}
