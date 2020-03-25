using System;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Common.Log;
using Lykke.Common.Log;
using Lykke.Service.CrossChainWalletLinker.Client;
using Lykke.Service.CrossChainWalletLinker.Client.Models;
using Lykke.Service.CrossChainWalletLinker.Domain.Services;
using Microsoft.AspNetCore.Mvc;
using LinkingError = Lykke.Service.CrossChainWalletLinker.Domain.Models.LinkingError;

namespace Lykke.Service.CrossChainWalletLinker.Controllers
{
    [ApiController]
    [Route("api/linkRequests")]
    public class LinkRequestsController : ControllerBase, ICrossChainWalletLinkerApi
    {
        private readonly IWalletLinker _walletLinker;
        private readonly IMapper _mapper;
        private readonly ILog _log;

        public LinkRequestsController(IWalletLinker walletLinker, IMapper mapper, ILogFactory logFactory)
        {
            _walletLinker = walletLinker;
            _mapper = mapper;
            _log = logFactory.CreateLog(this);
        }
        
        /// <summary>
        /// Create wallet linking request
        /// </summary>
        /// <param name="customerId">The customer id</param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(typeof(LinkingRequestResponseModel), (int) HttpStatusCode.OK)]
        [ProducesResponseType((int) HttpStatusCode.BadRequest)]
        public async Task<LinkingRequestResponseModel> CreateLinkRequestAsync([Required] Guid customerId)
        {
            var linkResult = await _walletLinker.CreateLinkRequestAsync(customerId);

            if (linkResult.Error != LinkingError.None)
            {
                _log.Warning(message: "Wallet link request has not been added",
                    context: new {customerId, linkResult.Error});
            }

            return _mapper.Map<LinkingRequestResponseModel>(linkResult);
        }
        
        /// <summary>
        /// Approve existing wallet linking request
        /// </summary>
        /// <param name="request">The approval details</param>
        /// <returns></returns>
        [HttpPost("approval")]
        [ProducesResponseType(typeof(LinkingApprovalResponseModel), (int) HttpStatusCode.OK)]
        [ProducesResponseType((int) HttpStatusCode.BadRequest)]
        public async Task<LinkingApprovalResponseModel> ApproveLinkRequestAsync([FromBody] LinkApprovalRequestModel request)
        {
            var approvalResult = await _walletLinker.ApproveLinkRequestAsync(request.PrivateAddress, 
                request.PublicAddress, 
                request.Signature);

            if (approvalResult.Error != LinkingError.None)
            {
                _log.Warning(message: "Wallet link request has not been approved",
                    context: new {request.PrivateAddress, request.PublicAddress, approvalResult.Error});
            }

            return _mapper.Map<LinkingApprovalResponseModel>(approvalResult);
        }

        /// <summary>
        /// Delete link between wallets
        /// </summary>
        /// <param name="customerId">The customerId</param>
        /// <returns></returns>
        [HttpDelete]
        [ProducesResponseType(typeof(UnlinkResponseModel), (int) HttpStatusCode.OK)]
        [ProducesResponseType((int) HttpStatusCode.BadRequest)]
        public async Task<UnlinkResponseModel> UnlinkAsync([Required] Guid customerId)
        {
            var unlinkResult = await _walletLinker.UnlinkAsync(customerId);

            if (unlinkResult.Error != LinkingError.None)
            {
                _log.Warning("Public wallet address was not unlinked",
                    context: new {customerId, unlinkResult.Error});
            }

            return _mapper.Map<UnlinkResponseModel>(unlinkResult);
        }
    }
}
