using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Common.Log;
using Lykke.Common.Log;
using Lykke.Service.CrossChainWalletLinker.Client;
using Lykke.Service.CrossChainWalletLinker.Client.Models;
using Lykke.Service.CrossChainWalletLinker.Domain.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Lykke.Service.CrossChainWalletLinker.Controllers
{
    [ApiController]
    [Route("api/configuration")]
    public class ConfigurationController : ControllerBase, ICrossChainConfigurationApi
    {
        private readonly IConfigurationItemsRepository _configurationItemsRepository;
        private readonly IMapper _mapper;
        private readonly ILog _log;

        public ConfigurationController(IConfigurationItemsRepository configurationItemsRepository,
            IMapper mapper,
            ILogFactory logFactory)
        {
            _configurationItemsRepository = configurationItemsRepository;
            _mapper = mapper;
            _log = logFactory.CreateLog(this);
        }
        
        /// <summary>
        /// Get all configuration items
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ConfigurationItemResponseModel>), (int) HttpStatusCode.OK)]
        public async Task<IEnumerable<ConfigurationItemResponseModel>> GetAllAsync()
        {
            var items = await _configurationItemsRepository.GetAllAsync();

            return _mapper.Map<IEnumerable<ConfigurationItemResponseModel>>(items);
        }

        /// <summary>
        /// Get configuration item by type
        /// </summary>
        /// <param name="type">The configuration item type</param>
        /// <returns></returns>
        [HttpGet("{type}")]
        [ProducesResponseType(typeof(ConfigurationItemResponseModel), (int) HttpStatusCode.OK)]
        [ProducesResponseType(typeof(void), (int) HttpStatusCode.NoContent)]
        public async Task<ConfigurationItemResponseModel> GetItemAsync(ConfigurationItemType type)
        {
            var domainType = type.ToDomain();

            if (domainType == null)
            {
                _log.Warning($"Configuration item type {type.ToString()} is not recognized");
                
                return null;
            }

            var item = await _configurationItemsRepository.GetAsync(domainType.Value);

            return _mapper.Map<ConfigurationItemResponseModel>(item);
        }

        /// <summary>
        /// Add or update configuration item
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(typeof(ConfigurationItemUpdateResponseModel), (int) HttpStatusCode.OK)]
        [ProducesResponseType(typeof(void), (int) HttpStatusCode.BadRequest)]
        public async Task<ConfigurationItemUpdateResponseModel> UpdateOrInsertItemAsync(ConfigurationItemRequestModel request)
        {
            var domainType = request.Type.ToDomain();

            if (domainType == null)
            {
                _log.Warning($"Configuration item type {request.Type.ToString()} is not recognized");
                
                return new ConfigurationItemUpdateResponseModel{Error = ConfigurationItemUpdateError.TypeNotRecognized};
            }

            await _configurationItemsRepository.UpsertAsync(domainType.Value, request.Value);
            
            _log.Info($"The configuration item of type {domainType.ToString()} has been added/updated successfully");
            
            return new ConfigurationItemUpdateResponseModel{Error = ConfigurationItemUpdateError.None};
        }
    }
}
