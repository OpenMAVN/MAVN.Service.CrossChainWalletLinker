using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Lykke.Service.CrossChainWalletLinker.Domain.Enums;
using Lykke.Service.CrossChainWalletLinker.Domain.Models;

namespace Lykke.Service.CrossChainWalletLinker.MsSqlRepositories.Entities
{
    [Table("configuration_items")]
    public class ConfigurationItemEntity : IConfigurationItem
    {
        [Key, Required]
        [Column("id")]
        public string Id { get; set; }
        
        [Required]
        [Column("type")]
        public ConfigurationItemType Type { get; set; }
        
        [Required]
        [Column("value")]
        public string Value { get; set; }

        public static ConfigurationItemEntity Create(string id, ConfigurationItemType type, string value)
        {
            return new ConfigurationItemEntity
            {
                Id = id,
                Type = type,
                Value = value
            };
        }
    }
}
