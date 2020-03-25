using System.Collections.Generic;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Lykke.Service.CrossChainWalletLinker.Client.Models;
using Refit;

namespace Lykke.Service.CrossChainWalletLinker.Client
{
    /// <summary>
    /// Cross chain configuration API interface  
    /// </summary>
    [PublicAPI]
    public interface ICrossChainConfigurationApi
    {
        /// <summary>
        /// Get all configuration items
        /// </summary>
        /// <returns></returns>
        [Get("/api/configuration")]
        Task<IEnumerable<ConfigurationItemResponseModel>> GetAllAsync();

        /// <summary>
        /// Get configuration item by type
        /// </summary>
        /// <param name="type">The configuration item type</param>
        /// <returns></returns>
        [Get("/api/configuration/{type}")]
        Task<ConfigurationItemResponseModel> GetItemAsync(ConfigurationItemType type);

        /// <summary>
        /// Updates or inserts new value into configuration
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [Post("/api/configuration")]
        Task<ConfigurationItemUpdateResponseModel> UpdateOrInsertItemAsync([Body] ConfigurationItemRequestModel request);
    }
}
