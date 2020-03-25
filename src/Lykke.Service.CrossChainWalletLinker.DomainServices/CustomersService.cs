using System;
using System.Threading.Tasks;
using Common.Log;
using Falcon.Numerics;
using Lykke.Common.Log;
using Lykke.Service.CrossChainWalletLinker.Domain.Enums;
using Lykke.Service.CrossChainWalletLinker.Domain.Models;
using Lykke.Service.CrossChainWalletLinker.Domain.Repositories;
using Lykke.Service.CrossChainWalletLinker.Domain.Services;

namespace Lykke.Service.CrossChainWalletLinker.DomainServices
{
    public class CustomersService : ICustomersService
    {
        private readonly IWalletLinkingRequestsRepository _requestsRepository;
        private readonly IWalletLinkingRequestsCounterRepository _counterRepository;
        private readonly IConfigurationItemsRepository _configurationItemsRepository;
        private readonly ILog _log;

        public CustomersService(IWalletLinkingRequestsRepository requestsRepository, 
            IWalletLinkingRequestsCounterRepository counterRepository, 
            IConfigurationItemsRepository configurationItemsRepository,
            ILogFactory logFactory)
        {
            _requestsRepository = requestsRepository;
            _counterRepository = counterRepository;
            _configurationItemsRepository = configurationItemsRepository;
            _log = logFactory.CreateLog(this);
        }
        
        public async Task<PublicAddressResultModel> GetPublicAddressAsync(Guid customerId)
        {
            if (customerId == Guid.Empty)
                return PublicAddressResultModel.Failed(PublicAddressError.InvalidCustomerId);

            var linkingRequest = await _requestsRepository.GetByIdAsync(customerId.ToString());

            if (linkingRequest == null)
                return PublicAddressResultModel.Succeeded(string.Empty, PublicAddressStatus.NotLinked);
            
            if (linkingRequest.IsPendingApproval())
                return PublicAddressResultModel.Succeeded(string.Empty, PublicAddressStatus.PendingCustomerApproval);

            if (linkingRequest.IsConfirmed())
                return PublicAddressResultModel.Succeeded(linkingRequest.PublicAddress, PublicAddressStatus.Linked);
                
            return PublicAddressResultModel.Succeeded(linkingRequest.PublicAddress, PublicAddressStatus.PendingConfirmation);
        }

        public async Task<Money18> GetNextWalletLinkingFeeAsync(Guid customerId)
        {
            if (customerId == Guid.Empty)
                throw new ArgumentNullException(nameof(customerId));

            var counter = await _counterRepository.GetAsync(customerId.ToString());

            var fee = await _configurationItemsRepository.GetAsync((counter?.ApprovalsCounter ?? 0) < 1
                ? ConfigurationItemType.FirstTimeLinkingFee
                : ConfigurationItemType.SubsequentLinkingFee);

            if (fee == null)
            {
                _log.Warning("Wallet linking fee has not been set, please use API to update it");

                return 0;
            }

            if (!Money18.TryParse(fee.Value, out var result))
            {
                _log.Error(message: "Couldn't parse Money18 from configuration value", context: new {fee = fee.Value});
                
                return 0;
            } 
            
            return result;
        }
    }
}
