using System;
using System.Linq;
using System.Threading.Tasks;
using Common.Log;
using Falcon.Numerics;
using Lykke.Common.Log;
using Lykke.Common.MsSql;
using Lykke.RabbitMqBroker.Publisher;
using MAVN.Service.CrossChainWalletLinker.Contract.Linking;
using MAVN.Service.CrossChainWalletLinker.Domain.Exceptions;
using MAVN.Service.CrossChainWalletLinker.Domain.Models;
using MAVN.Service.CrossChainWalletLinker.Domain.Repositories;
using MAVN.Service.CrossChainWalletLinker.Domain.Services;
using Lykke.Service.PrivateBlockchainFacade.Client;
using Lykke.Service.WalletManagement.Client;
using Lykke.Service.WalletManagement.Client.Enums;

namespace MAVN.Service.CrossChainWalletLinker.DomainServices
{
    public class WalletLinker : IWalletLinker
    {
        private readonly IWalletLinkingRequestsRepository _requestsRepository;
        private readonly IWalletLinkingRequestsCounterRepository _countersRepository;
        private readonly IPrivateBlockchainFacadeClient _pbfClient;
        private readonly ISettingsService _settingsService;
        private readonly IRabbitPublisher<WalletLinkingStatusChangeRequestedEvent> _requestedPublisher;
        private readonly IRabbitPublisher<WalletLinkingStatusChangeCompletedEvent> _completedPublisher;
        private readonly IRabbitPublisher<WalletLinkingStatusChangeFinalizedEvent> _finalizedPublisher;
        private readonly ISignatureValidator _signatureValidator;
        private readonly ICustomersService _customersService;
        private readonly IWalletManagementClient _walletManagementClient;
        private readonly ITransactionRunner _transactionRunner;
        private readonly int _linkCodeLength;
        private readonly ILog _log;


        public WalletLinker(IWalletLinkingRequestsRepository requestsRepository, 
            ILogFactory logFactory, 
            int linkCodeLength, 
            IPrivateBlockchainFacadeClient pbfClient, 
            ISettingsService settingsService, 
            IRabbitPublisher<WalletLinkingStatusChangeRequestedEvent> requestedPublisher, 
            IRabbitPublisher<WalletLinkingStatusChangeCompletedEvent> completedPublisher,
            IRabbitPublisher<WalletLinkingStatusChangeFinalizedEvent> finalizedPublisher,
            ISignatureValidator signatureValidator, 
            IWalletLinkingRequestsCounterRepository countersRepository, 
            ICustomersService customersService, 
            IWalletManagementClient walletManagementClient,
            ITransactionRunner transactionRunner)
        {
            _requestsRepository = requestsRepository;
            _linkCodeLength = linkCodeLength;
            _pbfClient = pbfClient;
            _settingsService = settingsService;
            _requestedPublisher = requestedPublisher;
            _completedPublisher = completedPublisher;
            _signatureValidator = signatureValidator;
            _countersRepository = countersRepository;
            _customersService = customersService;
            _walletManagementClient = walletManagementClient;
            _transactionRunner = transactionRunner;
            _finalizedPublisher = finalizedPublisher;
            _log = logFactory.CreateLog(this);
        }

        public async Task<LinkingRequestResultModel> CreateLinkRequestAsync(Guid customerId)
        {
            #region Validation
            
            if (customerId == Guid.Empty)
                return LinkingRequestResultModel.Failed(LinkingError.InvalidCustomerId);

            var walletError = await CheckWalletStatus(customerId.ToString());

            if(walletError != LinkingError.None)
                return LinkingRequestResultModel.Failed(walletError);

            var linkRequest = await _requestsRepository.GetByIdAsync(customerId.ToString());

            if (linkRequest != null)
            {
                _log.Warning(message: "Link request already exists", context: new {customerId});
                
                return LinkingRequestResultModel.Succeeded(linkRequest.LinkCode);
            }

            var walletResponse = await _pbfClient.CustomersApi.GetWalletAddress(customerId);

            if (walletResponse.Error != CustomerWalletAddressError.None)
            {
                _log.Error(message: "Can't get customer wallet address",
                    context: new {customerId, error = walletResponse.Error.ToString()});
                
                return LinkingRequestResultModel.Failed(LinkingError.CustomerWalletMissing);
            }
            
            #endregion

            var linkCode = GenerateRandomLinkCode();

            try
            {
                await _requestsRepository.AddAsync(customerId.ToString(), walletResponse.WalletAddress, linkCode);
            }
            catch (LinkingRequestAlreadyExistsException)
            {
                return LinkingRequestResultModel.Failed(LinkingError.LinkingRequestAlreadyExists);
            }
            
            _log.Info("New wallet linking request has been added", new {customerId, linkCode});
            
            return LinkingRequestResultModel.Succeeded(linkCode);
        }

        public async Task<LinkingApprovalResultModel> ApproveLinkRequestAsync(string privateAddress, string publicAddress, string signature)
        {
            #region Validation
            
            if (string.IsNullOrEmpty(privateAddress))
                return LinkingApprovalResultModel.Failed(LinkingError.InvalidPrivateAddress);
            
            if (string.IsNullOrEmpty(publicAddress))
                return LinkingApprovalResultModel.Failed(LinkingError.InvalidPublicAddress);
            
            if (string.IsNullOrEmpty(signature))
                return LinkingApprovalResultModel.Failed(LinkingError.InvalidSignature);
            
            var linkRequest = await _requestsRepository.GetByPrivateAddressAsync(privateAddress);
            
            if (linkRequest == null)
                return LinkingApprovalResultModel.Failed(LinkingError.LinkingRequestDoesNotExist);
            
            if (linkRequest.IsApproved())
                return LinkingApprovalResultModel.Failed(LinkingError.LinkingRequestAlreadyApproved);

            var walletError = await CheckWalletStatus(linkRequest.CustomerId);

            if (walletError != LinkingError.None)
                return LinkingApprovalResultModel.Failed(walletError);

            if (!_signatureValidator.Validate(linkRequest.LinkCode, signature, publicAddress))
                return LinkingApprovalResultModel.Failed(LinkingError.InvalidSignature);
            
            var fee = await _customersService.GetNextWalletLinkingFeeAsync(Guid.Parse(linkRequest.CustomerId));

            if (fee > 0)
            {
                var balanceResult = await _pbfClient.CustomersApi.GetBalanceAsync(Guid.Parse(linkRequest.CustomerId));
            
                if (balanceResult.Error != CustomerBalanceError.None)
                {
                    _log.Error(message: "Couldn't get balance for customer wallet",
                        context: new {customerId = linkRequest.CustomerId, error = balanceResult.Error.ToString()});
                    
                    return LinkingApprovalResultModel.Failed(LinkingError.NotEnoughFunds);
                }

                if (balanceResult.Total < fee)
                {
                    _log.Warning("The balance is not enough to pay wallet linking fee",
                        context: new {balanceTotal = balanceResult.Total, fee});
                    
                    return LinkingApprovalResultModel.Failed(LinkingError.NotEnoughFunds);
                }
            }

            #endregion

            await _transactionRunner.RunWithTransactionAsync(async txContext =>
            {
                await _requestsRepository.SetApprovedAsync(linkRequest.CustomerId, publicAddress, signature, fee,
                    txContext);

                var counter = await _countersRepository.GetAsync(linkRequest.CustomerId, txContext);

                var newCounterValue = counter?.ApprovalsCounter + 1 ?? 1;

                await _countersRepository.UpsertAsync(linkRequest.CustomerId, newCounterValue, txContext);
            });

            await PublishLinkCommandAsync(linkRequest.CustomerId, privateAddress, publicAddress, fee);
            
            _log.Info("Wallet linking request has been approved", new {linkRequest.CustomerId, publicAddress, fee});
            
            return LinkingApprovalResultModel.Succeeded();
        }

        public async Task<UnlinkResultModel> UnlinkAsync(Guid customerId)
        {
            #region Validation
            
            if (customerId == Guid.Empty)
                return UnlinkResultModel.Failed(LinkingError.InvalidCustomerId);
            
            var linkRequest = await _requestsRepository.GetByIdAsync(customerId.ToString());
            
            if (linkRequest == null)
                return UnlinkResultModel.Failed(LinkingError.LinkingRequestDoesNotExist);

            var walletError = await CheckWalletStatus(linkRequest.CustomerId);

            if (walletError != LinkingError.None)
                return UnlinkResultModel.Failed(walletError);

            if (linkRequest.IsApproved() && !linkRequest.IsConfirmed())
                return UnlinkResultModel.Failed(LinkingError.CannotDeleteLinkingRequestWhileConfirming);
            
            #endregion

            await PublishUnlinkCommandAsync(customerId.ToString(), linkRequest.PrivateAddress);

            _log.Info("Wallet has been submitted for unlinking",
                new {customerId, linkRequest.PrivateAddress, linkRequest.PublicAddress});
            
            return UnlinkResultModel.Succeeded();
        }

        public async Task<ConfirmationResultModel> ConfirmInPrivateAsync(string privateAddress)
        {
            #region Validation
            
            if (string.IsNullOrEmpty(privateAddress))
                return ConfirmationResultModel.Failed(ConfirmationError.InvalidPrivateAddress);
            
            var linkRequest = await _requestsRepository.GetByPrivateAddressAsync(privateAddress);
            
            if (linkRequest == null)
                return ConfirmationResultModel.Failed(ConfirmationError.LinkRequestDoesNotExist);
            
            if (linkRequest.IsPendingApproval())
                return ConfirmationResultModel.Failed(ConfirmationError.LinkRequestHasNotBeenApproved);
            
            if (linkRequest.IsConfirmedInPrivate)
                return ConfirmationResultModel.Failed(ConfirmationError.LinkRequestAlreadyConfirmed);
            
            #endregion

            await _requestsRepository.SetPrivatelyConfirmedAsync(linkRequest.CustomerId);
            
            _log.Info("Link request has been confirmed in private blockchain", new {linkRequest.CustomerId});
            
            await _completedPublisher.PublishAsync(new WalletLinkingStatusChangeCompletedEvent
            {
                CustomerId = linkRequest.CustomerId, 
                PublicAddress = linkRequest.PublicAddress,
                PrivateAddress = linkRequest.PrivateAddress,
                EventId = Guid.NewGuid().ToString(),
                Fee = linkRequest.Fee ?? 0
            });
                
            return ConfirmationResultModel.Succeeded();
        }

        public async Task<ConfirmationResultModel> ConfirmInPublicAsync(string privateAddress)
        {
            #region Validation
            
            if (string.IsNullOrEmpty(privateAddress))
                return ConfirmationResultModel.Failed(ConfirmationError.InvalidPrivateAddress);
            
            var linkRequest = await _requestsRepository.GetByPrivateAddressAsync(privateAddress);
            
            if (linkRequest == null)
                return ConfirmationResultModel.Failed(ConfirmationError.LinkRequestDoesNotExist);
            
            if (linkRequest.IsPendingApproval())
                return ConfirmationResultModel.Failed(ConfirmationError.LinkRequestHasNotBeenApproved);
            
            if (linkRequest.IsConfirmedInPublic)
                return ConfirmationResultModel.Failed(ConfirmationError.LinkRequestAlreadyConfirmed);
            
            #endregion

            await _requestsRepository.SetPubliclyConfirmedAsync(linkRequest.CustomerId);
            
            _log.Info("Link request has been confirmed in public blockchain", new {linkRequest.CustomerId});
            
            if (linkRequest.IsConfirmedInPrivate)
            {
                await _finalizedPublisher.PublishAsync(new WalletLinkingStatusChangeFinalizedEvent
                {
                    PrivateAddress = linkRequest.PrivateAddress,
                    PublicAddress = linkRequest.PublicAddress
                });
                
                _log.Info("The wallet linking has been finalized", new {linkRequest.CustomerId});
            }
            else
            {
                _log.Error(
                    message: "The wallet linking transaction has been confirmed in public blockchain but not confirmed in private blockchain yet",
                    context: new {linkRequest.CustomerId, linkRequest.PrivateAddress, linkRequest.PublicAddress});
            }
            
            return ConfirmationResultModel.Succeeded();
        }

        public async Task<IWalletLinkingRequest> GetByPrivateAddressAsync(string privateAddress)
        {
            if (string.IsNullOrEmpty(privateAddress))
                return null;
            
            return await _requestsRepository.GetByPrivateAddressAsync(privateAddress);
        }

        private async Task<LinkingError> CheckWalletStatus(string customerId)
        {
            var walletBlockStateResponse = await _walletManagementClient.Api.GetCustomerWalletBlockStateAsync(customerId);

            if (walletBlockStateResponse.Error == CustomerWalletBlockStatusError.CustomerNotFound)
                return LinkingError.CustomerDoesNotExist;

            if (walletBlockStateResponse.Status == CustomerWalletActivityStatus.Blocked)
                return LinkingError.CustomerWalletBlocked;

            return LinkingError.None;
        }

        private string GenerateRandomLinkCode()
        {
            var rnd = new Random();

            return Enumerable.Range(0, _linkCodeLength)
                .Select(r => rnd.Next(10))
                .Aggregate("", (current, next) => current + next);
        }

        private Task PublishUnlinkCommandAsync(string customerId, string privateAddress)
        {
            return _requestedPublisher.PublishAsync(new WalletLinkingStatusChangeRequestedEvent
            {
                Direction = LinkingDirection.Unlink,
                CustomerId = customerId,
                PrivateAddress = privateAddress,
                PublicAddress = null,
                Fee = 0,
                MasterWalletAddress = _settingsService.GetMasterWalletAddress(),
                EventId = Guid.NewGuid().ToString()
            });
        }

        private Task PublishLinkCommandAsync(string customerId, string privateAddress, string publicAddress, Money18 fee)
        {
            return _requestedPublisher.PublishAsync(new WalletLinkingStatusChangeRequestedEvent
            {
                Direction = LinkingDirection.Link,
                CustomerId = customerId,
                PrivateAddress = privateAddress,
                PublicAddress = publicAddress,
                Fee = fee,
                MasterWalletAddress = _settingsService.GetMasterWalletAddress(),
                EventId = Guid.NewGuid().ToString()
            });
        }
    }
}
