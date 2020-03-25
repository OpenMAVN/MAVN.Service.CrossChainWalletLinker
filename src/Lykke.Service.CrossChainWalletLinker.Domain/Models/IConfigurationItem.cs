using Lykke.Service.CrossChainWalletLinker.Domain.Enums;

namespace Lykke.Service.CrossChainWalletLinker.Domain.Models
{
    public interface IConfigurationItem
    {
        ConfigurationItemType Type { get; set; }
        
        string Value { get; set; }
    }
}
