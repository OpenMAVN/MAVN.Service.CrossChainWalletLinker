using System;
using MAVN.Numerics;

namespace MAVN.Service.CrossChainWalletLinker.Domain.Models
{
    public interface IWalletLinkingRequest
    {
        string CustomerId { get; set; }
        
        string PublicAddress { get; set; }
        
        string PrivateAddress { get; set; }
        
        string LinkCode { get; set; }
        
        bool IsConfirmedInPrivate { get; set; }
        
        bool IsConfirmedInPublic { get; set; }
        
        string Signature { get; set; }
        
        DateTime CreatedOn { get; set; }
        
        DateTime Timestamp { get; set; }
        
        Money18? Fee { get; set; }
    }
}
