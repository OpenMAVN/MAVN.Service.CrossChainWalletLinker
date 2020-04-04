using MAVN.Service.CrossChainWalletLinker.Domain.Enums;

namespace MAVN.Service.CrossChainWalletLinker.Domain.Models
{
    public interface IConfigurationItem
    {
        ConfigurationItemType Type { get; set; }
        
        string Value { get; set; }
    }
}
