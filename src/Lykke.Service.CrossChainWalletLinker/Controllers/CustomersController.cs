using System;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Lykke.Service.CrossChainWalletLinker.Client;
using Lykke.Service.CrossChainWalletLinker.Client.Models;
using Lykke.Service.CrossChainWalletLinker.Domain.Services;
using Microsoft.AspNetCore.Mvc;

namespace Lykke.Service.CrossChainWalletLinker.Controllers
{
    [ApiController]
    [Route("api/customers")]
    public class CustomersController : ControllerBase, ICrossChainCustomersApi
    {
        private readonly ICustomersService _customersService;
        private readonly IMapper _mapper;

        public CustomersController(ICustomersService customersService, IMapper mapper)
        {
            _customersService = customersService;
            _mapper = mapper;
        }
        
        /// <summary>
        /// Get the customer linked wallet public address 
        /// </summary>
        /// <param name="customerId">The customer id</param>
        /// <returns></returns>
        [HttpGet("{customerId}/publicAddress")]
        [ProducesResponseType(typeof(PublicAddressResponseModel), (int) HttpStatusCode.OK)]
        [ProducesResponseType((int) HttpStatusCode.BadRequest)]
        public async Task<PublicAddressResponseModel> GetLinkedPublicAddressAsync([Required] Guid customerId)
        {
            var publicAddressResult = await _customersService.GetPublicAddressAsync(customerId);

            return _mapper.Map<PublicAddressResponseModel>(publicAddressResult);
        }

        /// <summary>
        /// Get next fee for wallet linking for the customer
        /// </summary>
        /// <param name="customerId">The customer id</param>
        /// <returns></returns>
        [HttpGet("{customerId}/nextFee")]
        [ProducesResponseType(typeof(NextFeeResponseModel), (int) HttpStatusCode.OK)]
        [ProducesResponseType((int) HttpStatusCode.BadRequest)]
        public async Task<NextFeeResponseModel> GetNextFeeAsync([Required] Guid customerId)
        {
            var fee = await _customersService.GetNextWalletLinkingFeeAsync(customerId);

            return new NextFeeResponseModel {CustomerId = customerId.ToString(), Fee = fee};
        }
    }
}
