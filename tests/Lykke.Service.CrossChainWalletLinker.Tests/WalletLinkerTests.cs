using System;
using System.Threading.Tasks;
using Lykke.Common.MsSql;
using Lykke.Logs;
using Lykke.RabbitMqBroker.Publisher;
using Lykke.Service.CrossChainWalletLinker.Contract.Linking;
using Lykke.Service.CrossChainWalletLinker.Domain.Exceptions;
using Lykke.Service.CrossChainWalletLinker.Domain.Models;
using Lykke.Service.CrossChainWalletLinker.Domain.Repositories;
using Lykke.Service.CrossChainWalletLinker.Domain.Services;
using Lykke.Service.CrossChainWalletLinker.DomainServices;
using Lykke.Service.CrossChainWalletLinker.MsSqlRepositories;
using Lykke.Service.CrossChainWalletLinker.MsSqlRepositories.Entities;
using Lykke.Service.PrivateBlockchainFacade.Client;
using Lykke.Service.PrivateBlockchainFacade.Client.Models;
using Lykke.Service.WalletManagement.Client;
using Lykke.Service.WalletManagement.Client.Enums;
using Lykke.Service.WalletManagement.Client.Models.Responses;
using Moq;
using Xunit;
using LinkingError = Lykke.Service.CrossChainWalletLinker.Domain.Models.LinkingError;

namespace Lykke.Service.CrossChainWalletLinker.Tests
{
    public class WalletLinkerTests
    {
        #region Mocks
        
        private readonly Mock<IWalletLinkingRequestsRepository> _requestsRepositoryMock = new Mock<IWalletLinkingRequestsRepository>();
        
        private readonly Mock<IWalletLinkingRequestsCounterRepository> _countersRepositoryMock = new Mock<IWalletLinkingRequestsCounterRepository>();
        
        private readonly Mock<IPrivateBlockchainFacadeClient> _pbfClientMock = new Mock<IPrivateBlockchainFacadeClient>();
        
        private readonly Mock<ISettingsService> _settingsServiceMock = new Mock<ISettingsService>();
        
        private readonly Mock<IRabbitPublisher<WalletLinkingStatusChangeRequestedEvent>> _requestedPublisherMock = new Mock<IRabbitPublisher<WalletLinkingStatusChangeRequestedEvent>>();
        
        private readonly Mock<IRabbitPublisher<WalletLinkingStatusChangeCompletedEvent>> _completedPublisherMock = new Mock<IRabbitPublisher<WalletLinkingStatusChangeCompletedEvent>>();
        
        private readonly Mock<IRabbitPublisher<WalletLinkingStatusChangeFinalizedEvent>> _finalizedPublisherMock = new Mock<IRabbitPublisher<WalletLinkingStatusChangeFinalizedEvent>>();
        
        private readonly Mock<ISignatureValidator> _signatureValidatorMock = new Mock<ISignatureValidator>();
        
        private readonly Mock<ICustomersService> _customersServiceMock = new Mock<ICustomersService>();

        private readonly Mock<IWalletManagementClient> _wmClientMock = new Mock<IWalletManagementClient>();

        #endregion

        #region Consts

        private const string FakeCustomerId = "3f1443e2-b848-4567-8fb5-ebe7337a87e9";
        
        private const string FakeCustomerWalletAddress = "0x7d275eb17ceaae04b17768d4459741bae062ee09";

        private const string FakeSignature = "signature";
        
        private const int LinkCodeLength = 5;
        
        #endregion

        [Fact]
        public async Task CreateLinkRequestAsync_InvalidParameters_ReturnsError()
        {
            var sut = CreateSutInstance();

            var result = await sut.CreateLinkRequestAsync(Guid.Empty);

            Assert.Equal(LinkingError.InvalidCustomerId, result.Error);
            Assert.Null(result.LinkCode);
        }

        [Fact]
        public async Task CreateLinkRequestAsync_AlreadyExists_ReturnsError()
        {
            const string linkCode = "linkCode";
            
            _requestsRepositoryMock
                .Setup(x => x.GetByIdAsync(It.IsAny<string>(), null))
                .ReturnsAsync(new WalletLinkingRequestEntity {LinkCode = linkCode});

            _wmClientMock.Setup(x => x.Api.GetCustomerWalletBlockStateAsync(FakeCustomerId))
                .ReturnsAsync(new CustomerWalletBlockStatusResponse { Status = CustomerWalletActivityStatus.Active });

            var sut = CreateSutInstance();

            var result = await sut.CreateLinkRequestAsync(Guid.Parse(FakeCustomerId));
            
            Assert.Equal(LinkingError.None, result.Error);
            Assert.Equal(linkCode, result.LinkCode);
        }

        [Theory]
        [ClassData(typeof(CustomerWalletAddressErrorsTestData))]
        public async Task CreateLinkRequestAsync_ErrorWhenGetCustomerAddress_ReturnsError(CustomerWalletAddressError error)
        {
            _requestsRepositoryMock
                .Setup(x => x.GetByIdAsync(It.IsAny<string>(), null))
                .ReturnsAsync((IWalletLinkingRequest) null);

            _pbfClientMock
                .Setup(x => x.CustomersApi.GetWalletAddress(It.IsAny<Guid>()))
                .ReturnsAsync(new CustomerWalletAddressResponseModel {Error = error, WalletAddress = null});

            _wmClientMock.Setup(x => x.Api.GetCustomerWalletBlockStateAsync(FakeCustomerId))
                .ReturnsAsync(new CustomerWalletBlockStatusResponse { Status = CustomerWalletActivityStatus.Active });

            var sut = CreateSutInstance();

            var result = await sut.CreateLinkRequestAsync(Guid.Parse(FakeCustomerId));
            
            Assert.Equal(LinkingError.CustomerWalletMissing, result.Error);
            Assert.Null(result.LinkCode);
        }

        [Fact]
        public async Task CreateLinkRequestAsync_RepositoryException_ReturnsError()
        {
            _requestsRepositoryMock
                .Setup(x => x.GetByIdAsync(It.IsAny<string>(), null))
                .ReturnsAsync((IWalletLinkingRequest) null);

            _pbfClientMock
                .Setup(x => x.CustomersApi.GetWalletAddress(It.IsAny<Guid>()))
                .ReturnsAsync(new CustomerWalletAddressResponseModel
                {
                    Error = CustomerWalletAddressError.None, WalletAddress = FakeCustomerWalletAddress
                });

            _requestsRepositoryMock
                .Setup(x => x.AddAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), null))
                .ThrowsAsync(new LinkingRequestAlreadyExistsException(FakeCustomerId));

            _wmClientMock.Setup(x => x.Api.GetCustomerWalletBlockStateAsync(FakeCustomerId))
                .ReturnsAsync(new CustomerWalletBlockStatusResponse { Status = CustomerWalletActivityStatus.Active });

            var sut = CreateSutInstance();

            var result = await sut.CreateLinkRequestAsync(Guid.Parse(FakeCustomerId));
            
            Assert.Equal(LinkingError.LinkingRequestAlreadyExists, result.Error);
            Assert.Null(result.LinkCode);
        }

        [Fact]
        public async Task CreateLinkRequestAsync_NoErrors_ReturnsSuccess()
        {
            _requestsRepositoryMock
                .Setup(x => x.GetByIdAsync(It.IsAny<string>(), null))
                .ReturnsAsync((IWalletLinkingRequest) null);

            _pbfClientMock
                .Setup(x => x.CustomersApi.GetWalletAddress(It.IsAny<Guid>()))
                .ReturnsAsync(new CustomerWalletAddressResponseModel
                {
                    Error = CustomerWalletAddressError.None, WalletAddress = FakeCustomerWalletAddress
                });

            _requestsRepositoryMock
                .Setup(x => x.AddAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), null))
                .Returns(Task.CompletedTask);

            _wmClientMock.Setup(x => x.Api.GetCustomerWalletBlockStateAsync(FakeCustomerId))
                .ReturnsAsync(new CustomerWalletBlockStatusResponse { Status = CustomerWalletActivityStatus.Active });

            var sut = CreateSutInstance();

            var result = await sut.CreateLinkRequestAsync(Guid.Parse(FakeCustomerId));
            
            Assert.Equal(LinkingError.None, result.Error);
            Assert.NotNull(result.LinkCode);
            Assert.Equal(LinkCodeLength, result.LinkCode.Length);
        }

        [Theory]
        [InlineData(null, FakeCustomerWalletAddress, FakeSignature, LinkingError.InvalidPrivateAddress)]
        [InlineData("", FakeCustomerWalletAddress, FakeSignature, LinkingError.InvalidPrivateAddress)]
        [InlineData(FakeCustomerWalletAddress, null, FakeSignature, LinkingError.InvalidPublicAddress)]
        [InlineData(FakeCustomerWalletAddress, "", FakeSignature, LinkingError.InvalidPublicAddress)]
        [InlineData(FakeCustomerWalletAddress, FakeCustomerWalletAddress, null, LinkingError.InvalidSignature)]
        [InlineData(FakeCustomerWalletAddress, FakeCustomerWalletAddress, "", LinkingError.InvalidSignature)]
        public async Task ApproveLinkRequestAsync_InvalidParameters_ReturnsError(string privateAddress,
            string publicAddress, 
            string signature, 
            LinkingError expectedError)
        {
            var sut = CreateSutInstance();

            var result = await sut.ApproveLinkRequestAsync(privateAddress, publicAddress, signature);
            
            Assert.Equal(expectedError, result.Error);
        }

        [Fact]
        public async Task ApproveLinkRequestAsync_NotExists_ReturnsError()
        {
            _requestsRepositoryMock
                .Setup(x => x.GetByPrivateAddressAsync(It.IsAny<string>(), null))
                .ReturnsAsync((IWalletLinkingRequest) null);

            var sut = CreateSutInstance();

            var result = await sut.ApproveLinkRequestAsync(FakeCustomerWalletAddress, 
                FakeCustomerWalletAddress, FakeSignature);

            Assert.Equal(LinkingError.LinkingRequestDoesNotExist, result.Error);
        }

        [Fact]
        public async Task ApproveLinkRequestAsync_AlreadyApproved_ReturnsError()
        {
            _requestsRepositoryMock
                .Setup(x => x.GetByPrivateAddressAsync(It.IsAny<string>(), null))
                .ReturnsAsync(new WalletLinkingRequestEntity{PublicAddress = FakeCustomerWalletAddress, Signature = FakeSignature});
            
            var sut = CreateSutInstance();

            var result = await sut.ApproveLinkRequestAsync(FakeCustomerWalletAddress, 
                FakeCustomerWalletAddress, FakeSignature);

            Assert.Equal(LinkingError.LinkingRequestAlreadyApproved, result.Error);
        }

        [Fact]
        public async Task ApproveLinkRequestAsync_InvalidSignature_ReturnsError()
        {
            _requestsRepositoryMock
                .Setup(x => x.GetByPrivateAddressAsync(It.IsAny<string>(), null))
                .ReturnsAsync(new WalletLinkingRequestEntity{CustomerId = FakeCustomerId});

            _signatureValidatorMock
                .Setup(x => x.Validate(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(false);

            _wmClientMock.Setup(x => x.Api.GetCustomerWalletBlockStateAsync(FakeCustomerId))
                .ReturnsAsync(new CustomerWalletBlockStatusResponse { Status = CustomerWalletActivityStatus.Active });

            var sut = CreateSutInstance();

            var result = await sut.ApproveLinkRequestAsync(FakeCustomerWalletAddress, 
                FakeCustomerWalletAddress, FakeSignature);
            
            Assert.Equal(LinkingError.InvalidSignature, result.Error);
        }

        [Fact]
        public async Task ApproveLinkRequestAsync_ZeroFee_NoBalanceValidation()
        {
            const decimal zeroFee = 0;
            
            _requestsRepositoryMock
                .Setup(x => x.GetByPrivateAddressAsync(It.IsAny<string>(), null))
                .ReturnsAsync(new WalletLinkingRequestEntity {CustomerId = FakeCustomerId});
            
            _signatureValidatorMock
                .Setup(x => x.Validate(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(true);

            _customersServiceMock
                .Setup(x => x.GetNextWalletLinkingFeeAsync(It.IsAny<Guid>()))
                .ReturnsAsync(zeroFee);
            
            _pbfClientMock
                .Setup(x => x.CustomersApi.GetBalanceAsync(It.IsAny<Guid>()))
                .Verifiable();

            _wmClientMock.Setup(x => x.Api.GetCustomerWalletBlockStateAsync(FakeCustomerId))
                .ReturnsAsync(new CustomerWalletBlockStatusResponse { Status = CustomerWalletActivityStatus.Active });

            var sut = CreateSutInstance();
            
            var result = await sut.ApproveLinkRequestAsync(FakeCustomerWalletAddress, 
                FakeCustomerWalletAddress, FakeSignature);
            
            _pbfClientMock.Verify(x => x.CustomersApi.GetBalanceAsync(It.IsAny<Guid>()), Times.Never);
            
            Assert.Equal(LinkingError.None, result.Error);
        }

        [Fact]
        public async Task ApproveLinkRequestAsync_NonZeroFee_ErrorGettingBalance_ReturnsNotEnoughFunds()
        {
            const decimal nonZeroFee = 1;
            
            _requestsRepositoryMock
                .Setup(x => x.GetByPrivateAddressAsync(It.IsAny<string>(), null))
                .ReturnsAsync(new WalletLinkingRequestEntity {CustomerId = FakeCustomerId});
            
            _signatureValidatorMock
                .Setup(x => x.Validate(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(true);

            _customersServiceMock
                .Setup(x => x.GetNextWalletLinkingFeeAsync(It.IsAny<Guid>()))
                .ReturnsAsync(nonZeroFee);

            _pbfClientMock
                .Setup(x => x.CustomersApi.GetBalanceAsync(It.IsAny<Guid>()))
                .ReturnsAsync(new CustomerBalanceResponseModel {Error = CustomerBalanceError.CustomerWalletMissing});

            _wmClientMock.Setup(x => x.Api.GetCustomerWalletBlockStateAsync(FakeCustomerId))
                .ReturnsAsync(new CustomerWalletBlockStatusResponse { Status = CustomerWalletActivityStatus.Active });

            var sut = CreateSutInstance();
            
            var result = await sut.ApproveLinkRequestAsync(FakeCustomerWalletAddress, 
                FakeCustomerWalletAddress, FakeSignature);
            
            Assert.Equal(LinkingError.NotEnoughFunds, result.Error);
        }

        [Fact]
        public async Task ApproveLinkRequestAsync_NonZeroFee_BalanceNotEnough_ReturnsNotEnoughFunds()
        {
            const decimal nonZeroFee = 100;

            const decimal balance = 10;
            
            _requestsRepositoryMock
                .Setup(x => x.GetByPrivateAddressAsync(It.IsAny<string>(), null))
                .ReturnsAsync(new WalletLinkingRequestEntity {CustomerId = FakeCustomerId});
            
            _signatureValidatorMock
                .Setup(x => x.Validate(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(true);

            _customersServiceMock
                .Setup(x => x.GetNextWalletLinkingFeeAsync(It.IsAny<Guid>()))
                .ReturnsAsync(nonZeroFee);

            _pbfClientMock
                .Setup(x => x.CustomersApi.GetBalanceAsync(It.IsAny<Guid>()))
                .ReturnsAsync(new CustomerBalanceResponseModel {Error = CustomerBalanceError.None, Total = balance});

            _wmClientMock.Setup(x => x.Api.GetCustomerWalletBlockStateAsync(FakeCustomerId))
                .ReturnsAsync(new CustomerWalletBlockStatusResponse { Status = CustomerWalletActivityStatus.Active });

            var sut = CreateSutInstance();
            
            var result = await sut.ApproveLinkRequestAsync(FakeCustomerWalletAddress, 
                FakeCustomerWalletAddress, FakeSignature);
            
            Assert.Equal(LinkingError.NotEnoughFunds, result.Error);
        }

        [Fact]
        public async Task ApproveLinkRequestAsync_NoErrors_ReturnsSuccess()
        {
            _requestsRepositoryMock
                .Setup(x => x.GetByPrivateAddressAsync(It.IsAny<string>(), null))
                .ReturnsAsync(new WalletLinkingRequestEntity {CustomerId = FakeCustomerId});
            
            _countersRepositoryMock
                .Setup(x => x.UpsertAsync(It.IsAny<string>(), It.IsAny<int>(), null))
                .Verifiable();
            
            _signatureValidatorMock
                .Setup(x => x.Validate(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(true);

            _wmClientMock.Setup(x => x.Api.GetCustomerWalletBlockStateAsync(FakeCustomerId))
                .ReturnsAsync(new CustomerWalletBlockStatusResponse {Status = CustomerWalletActivityStatus.Active});

            var sut = CreateSutInstance();

            var result = await sut.ApproveLinkRequestAsync(FakeCustomerWalletAddress, 
                FakeCustomerWalletAddress, FakeSignature);

            _countersRepositoryMock.Verify(x => x.UpsertAsync(It.IsAny<string>(), It.IsAny<int>(), null), Times.Once);
            
            Assert.Equal(LinkingError.None, result.Error);
        }

        [Fact]
        public async Task UnlinkAsync_InvalidParameters_ReturnsError()
        {
            var sut = CreateSutInstance();

            var result = await sut.UnlinkAsync(Guid.Empty);
            
            Assert.Equal(LinkingError.InvalidCustomerId, result.Error);
        }

        [Fact]
        public async Task UnlinkAsync_NoLinkingRequest_ReturnsError()
        {
            _requestsRepositoryMock
                .Setup(x => x.GetByIdAsync(It.IsAny<string>(), null))
                .ReturnsAsync((IWalletLinkingRequest) null);
            
            var sut = CreateSutInstance();
            
            var result = await sut.UnlinkAsync(Guid.Parse(FakeCustomerId));
            
            Assert.Equal(LinkingError.LinkingRequestDoesNotExist, result.Error);
        }

        [Fact]
        public async Task UnlinkAsync_LinkingRequestNotApproved_ReturnsSuccess()
        {
            _requestsRepositoryMock
                .Setup(x => x.GetByIdAsync(It.IsAny<string>(), null))
                .ReturnsAsync(new WalletLinkingRequestEntity
                {
                    Signature = string.Empty,
                    PublicAddress = string.Empty,
                    IsConfirmedInPrivate = false,
                    IsConfirmedInPublic = false,
                    CustomerId = FakeCustomerId,
                });

            _wmClientMock.Setup(x => x.Api.GetCustomerWalletBlockStateAsync(FakeCustomerId))
                .ReturnsAsync(new CustomerWalletBlockStatusResponse { Status = CustomerWalletActivityStatus.Active });

            var sut = CreateSutInstance();

            var result = await sut.UnlinkAsync(Guid.Parse(FakeCustomerId));
            
            Assert.Equal(LinkingError.None, result.Error);
        }

        [Theory]
        [InlineData(false, true)]
        [InlineData(true, false)]
        [InlineData(false, false)]
        public async Task UnlinkAsync_LinkingRequestApprovedButNotConfirmed_ReturnsError(bool isPrivatelyConfirmed, bool isPubliclyConfirmed)
        {
            _requestsRepositoryMock
                .Setup(x => x.GetByIdAsync(It.IsAny<string>(), null))
                .ReturnsAsync(new WalletLinkingRequestEntity
                {
                    Signature = FakeSignature,
                    PublicAddress = FakeCustomerWalletAddress,
                    IsConfirmedInPrivate = isPrivatelyConfirmed,
                    IsConfirmedInPublic = isPubliclyConfirmed,
                    CustomerId = FakeCustomerId
                });

            _wmClientMock.Setup(x => x.Api.GetCustomerWalletBlockStateAsync(FakeCustomerId))
                .ReturnsAsync(new CustomerWalletBlockStatusResponse { Status = CustomerWalletActivityStatus.Active });

            var sut = CreateSutInstance();

            var result = await sut.UnlinkAsync(Guid.Parse(FakeCustomerId));
            
            Assert.Equal(LinkingError.CannotDeleteLinkingRequestWhileConfirming, result.Error);
        }

        [Fact]
        public async Task UnlinkAsync_LinkingRequestApprovedAndConfirmed_ReturnsSuccess()
        {
            _requestsRepositoryMock
                .Setup(x => x.GetByIdAsync(It.IsAny<string>(), null))
                .ReturnsAsync(new WalletLinkingRequestEntity
                {
                    Signature = FakeSignature,
                    PublicAddress = FakeCustomerWalletAddress,
                    IsConfirmedInPrivate = true,
                    IsConfirmedInPublic = true,
                    CustomerId = FakeCustomerId
                });

            _wmClientMock.Setup(x => x.Api.GetCustomerWalletBlockStateAsync(FakeCustomerId))
                .ReturnsAsync(new CustomerWalletBlockStatusResponse { Status = CustomerWalletActivityStatus.Active });

            var sut = CreateSutInstance();

            var result = await sut.UnlinkAsync(Guid.Parse(FakeCustomerId));
            
            Assert.Equal(LinkingError.None, result.Error);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public async Task ConfirmInPrivateAsync_InvalidParameters_ReturnsError(string privateAddress)
        {
            var sut = CreateSutInstance();

            var result = await sut.ConfirmInPrivateAsync(privateAddress);
            
            Assert.Equal(ConfirmationError.InvalidPrivateAddress, result.Error);
        }

        [Fact]
        public async Task ConfirmInPrivateAsync_LinkingRequestDoesNotExist_ReturnsError()
        {
            _requestsRepositoryMock
                .Setup(x => x.GetByPrivateAddressAsync(It.IsAny<string>(), null))
                .ReturnsAsync((IWalletLinkingRequest) null);
            
            var sut = CreateSutInstance();

            var result = await sut.ConfirmInPrivateAsync(FakeCustomerWalletAddress);
            
            Assert.Equal(ConfirmationError.LinkRequestDoesNotExist, result.Error);
        }

        [Fact]
        public async Task ConfirmInPrivateAsync_LinkingRequestHasNotBeenApproved_ReturnsError()
        {
            _requestsRepositoryMock
                .Setup(x => x.GetByPrivateAddressAsync(It.IsAny<string>(), null))
                .ReturnsAsync(new WalletLinkingRequestEntity());
            
            var sut = CreateSutInstance();

            var result = await sut.ConfirmInPrivateAsync(FakeCustomerWalletAddress);
            
            Assert.Equal(ConfirmationError.LinkRequestHasNotBeenApproved, result.Error);
        }

        [Fact]
        public async Task ConfirmInPrivateAsync_LinkingRequestAlreadyConfirmed_ReturnsError()
        {
            _requestsRepositoryMock
                .Setup(x => x.GetByPrivateAddressAsync(It.IsAny<string>(), null))
                .ReturnsAsync(new WalletLinkingRequestEntity{PublicAddress = FakeCustomerWalletAddress, Signature = FakeSignature, IsConfirmedInPrivate = true});
            
            var sut = CreateSutInstance();

            var result = await sut.ConfirmInPrivateAsync(FakeCustomerWalletAddress);
            
            Assert.Equal(ConfirmationError.LinkRequestAlreadyConfirmed, result.Error);
        }

        [Fact]
        public async Task ConfirmInPrivateAsync_AlreadyConfirmedInPublic_PublishesCompletedEvent()
        {
            _requestsRepositoryMock
                .Setup(x => x.GetByPrivateAddressAsync(It.IsAny<string>(), null))
                .ReturnsAsync(new WalletLinkingRequestEntity{PublicAddress = FakeCustomerWalletAddress, Signature = FakeSignature, IsConfirmedInPrivate = false, IsConfirmedInPublic = true});
            
            _completedPublisherMock
                .Setup(x => x.PublishAsync(It.IsAny<WalletLinkingStatusChangeCompletedEvent>()))
                .Returns(Task.CompletedTask)
                .Verifiable();
            
            var sut = CreateSutInstance();

            var result = await sut.ConfirmInPrivateAsync(FakeCustomerWalletAddress);
            
            _completedPublisherMock.Verify(x => x.PublishAsync(It.IsAny<WalletLinkingStatusChangeCompletedEvent>()), Times.Once);
            
            Assert.Equal(ConfirmationError.None, result.Error);
        }

        [Fact]
        public async Task ConfirmInPrivateAsync_NotConfirmedInPublicYet_DoesNotPublishFinalizedEvent()
        {
            _requestsRepositoryMock
                .Setup(x => x.GetByPrivateAddressAsync(It.IsAny<string>(), null))
                .ReturnsAsync(new WalletLinkingRequestEntity{PublicAddress = FakeCustomerWalletAddress, Signature = FakeSignature, IsConfirmedInPrivate = false, IsConfirmedInPublic = false});
            
            _finalizedPublisherMock
                .Setup(x => x.PublishAsync(It.IsAny<WalletLinkingStatusChangeFinalizedEvent>()))
                .Returns(Task.CompletedTask)
                .Verifiable();
            
            var sut = CreateSutInstance();

            var result = await sut.ConfirmInPrivateAsync(FakeCustomerWalletAddress);
            
            _finalizedPublisherMock.Verify(x => x.PublishAsync(It.IsAny<WalletLinkingStatusChangeFinalizedEvent>()), Times.Never);
            
            Assert.Equal(ConfirmationError.None, result.Error);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public async Task ConfirmInPublicAsync_InvalidParameters_ReturnsError(string privateAddress)
        {
            var sut = CreateSutInstance();

            var result = await sut.ConfirmInPublicAsync(privateAddress);
            
            Assert.Equal(ConfirmationError.InvalidPrivateAddress, result.Error);
        }
        
        [Fact]
        public async Task ConfirmInPublicAsync_LinkingRequestDoesNotExist_ReturnsError()
        {
            _requestsRepositoryMock
                .Setup(x => x.GetByPrivateAddressAsync(It.IsAny<string>(), null))
                .ReturnsAsync((IWalletLinkingRequest) null);
            
            var sut = CreateSutInstance();

            var result = await sut.ConfirmInPublicAsync(FakeCustomerWalletAddress);
            
            Assert.Equal(ConfirmationError.LinkRequestDoesNotExist, result.Error);
        }
        
        [Fact]
        public async Task ConfirmInPublicAsync_LinkingRequestHasNotBeenApproved_ReturnsError()
        {
            _requestsRepositoryMock
                .Setup(x => x.GetByPrivateAddressAsync(It.IsAny<string>(), null))
                .ReturnsAsync(new WalletLinkingRequestEntity());
            
            var sut = CreateSutInstance();

            var result = await sut.ConfirmInPublicAsync(FakeCustomerWalletAddress);
            
            Assert.Equal(ConfirmationError.LinkRequestHasNotBeenApproved, result.Error);
        }
        
        [Fact]
        public async Task ConfirmInPublicAsync_LinkingRequestAlreadyConfirmed_ReturnsError()
        {
            _requestsRepositoryMock
                .Setup(x => x.GetByPrivateAddressAsync(It.IsAny<string>(), null))
                .ReturnsAsync(new WalletLinkingRequestEntity{PublicAddress = FakeCustomerWalletAddress, Signature = FakeSignature, IsConfirmedInPublic = true});
            
            var sut = CreateSutInstance();

            var result = await sut.ConfirmInPublicAsync(FakeCustomerWalletAddress);
            
            Assert.Equal(ConfirmationError.LinkRequestAlreadyConfirmed, result.Error);
        }
        
        [Fact]
        public async Task ConfirmInPublicAsync_AlreadyConfirmedInPrivate_PublishesFinalizedEvent()
        {
            _requestsRepositoryMock
                .Setup(x => x.GetByPrivateAddressAsync(It.IsAny<string>(), null))
                .ReturnsAsync(new WalletLinkingRequestEntity{PublicAddress = FakeCustomerWalletAddress, Signature = FakeSignature, IsConfirmedInPrivate = true, IsConfirmedInPublic = false});

            _finalizedPublisherMock
                .Setup(x => x.PublishAsync(It.IsAny<WalletLinkingStatusChangeFinalizedEvent>()))
                .Returns(Task.CompletedTask)
                .Verifiable();

            var sut = CreateSutInstance();

            var result = await sut.ConfirmInPublicAsync(FakeCustomerWalletAddress);
            
            _finalizedPublisherMock.Verify(x => x.PublishAsync(It.IsAny<WalletLinkingStatusChangeFinalizedEvent>()), Times.Once);
            
            Assert.Equal(ConfirmationError.None, result.Error);
        }
        
        [Fact]
        public async Task ConfirmInPublicAsync_NotConfirmedInPrivateYet_DoesNotPublishCompletedEvent()
        {
            _requestsRepositoryMock
                .Setup(x => x.GetByPrivateAddressAsync(It.IsAny<string>(), null))
                .ReturnsAsync(new WalletLinkingRequestEntity{PublicAddress = FakeCustomerWalletAddress, Signature = FakeSignature, IsConfirmedInPrivate = false, IsConfirmedInPublic = false});
            
            _completedPublisherMock
                .Setup(x => x.PublishAsync(It.IsAny<WalletLinkingStatusChangeCompletedEvent>()))
                .Returns(Task.CompletedTask)
                .Verifiable();
            
            var sut = CreateSutInstance();

            var result = await sut.ConfirmInPublicAsync(FakeCustomerWalletAddress);
            
            _completedPublisherMock.Verify(x => x.PublishAsync(It.IsAny<WalletLinkingStatusChangeCompletedEvent>()), Times.Never);
            
            Assert.Equal(ConfirmationError.None, result.Error);
        }

        [Fact]
        public async Task CreateLinkRequestAsync_CustomerDoesNotExist_ErrorReturned()
        {
            _wmClientMock.Setup(x => x.Api.GetCustomerWalletBlockStateAsync(FakeCustomerId))
                .ReturnsAsync(new CustomerWalletBlockStatusResponse
                {
                    Error = CustomerWalletBlockStatusError.CustomerNotFound
                });

            var sut = CreateSutInstance();

            var result = await sut.CreateLinkRequestAsync(Guid.Parse(FakeCustomerId));

            Assert.Equal(LinkingError.CustomerDoesNotExist, result.Error);
        }

        [Fact]
        public async Task CreateLinkRequestAsync_CustomerWalletIsBlocked_ErrorReturned()
        {
            _wmClientMock.Setup(x => x.Api.GetCustomerWalletBlockStateAsync(FakeCustomerId))
                .ReturnsAsync(new CustomerWalletBlockStatusResponse
                {
                    Error = CustomerWalletBlockStatusError.None,
                    Status = CustomerWalletActivityStatus.Blocked
                });

            var sut = CreateSutInstance();

            var result = await sut.CreateLinkRequestAsync(Guid.Parse(FakeCustomerId));

            Assert.Equal(LinkingError.CustomerWalletBlocked, result.Error);
        }

        [Fact]
        public async Task UnlinkAsync_CustomerDoesNotExist_ErrorReturned()
        {
            _wmClientMock.Setup(x => x.Api.GetCustomerWalletBlockStateAsync(FakeCustomerId))
                .ReturnsAsync(new CustomerWalletBlockStatusResponse
                {
                    Error = CustomerWalletBlockStatusError.CustomerNotFound
                });

            _requestsRepositoryMock
                .Setup(x => x.GetByIdAsync(FakeCustomerId, null))
                .ReturnsAsync(new WalletLinkingRequestEntity
                {
                    Signature = string.Empty,
                    PublicAddress = string.Empty,
                    IsConfirmedInPrivate = false,
                    IsConfirmedInPublic = false,
                    CustomerId = FakeCustomerId,
                });

            var sut = CreateSutInstance();

            var result = await sut.CreateLinkRequestAsync(Guid.Parse(FakeCustomerId));

            Assert.Equal(LinkingError.CustomerDoesNotExist, result.Error);
        }

        [Fact]
        public async Task UnlinkAsync_CustomerWalletIsBlocked_ErrorReturned()
        {
            _wmClientMock.Setup(x => x.Api.GetCustomerWalletBlockStateAsync(FakeCustomerId))
                .ReturnsAsync(new CustomerWalletBlockStatusResponse
                {
                    Error = CustomerWalletBlockStatusError.None,
                    Status = CustomerWalletActivityStatus.Blocked
                });

            _requestsRepositoryMock
                .Setup(x => x.GetByIdAsync(FakeCustomerId, null))
                .ReturnsAsync(new WalletLinkingRequestEntity
                {
                    Signature = string.Empty,
                    PublicAddress = string.Empty,
                    IsConfirmedInPrivate = false,
                    IsConfirmedInPublic = false,
                    CustomerId = FakeCustomerId,
                });

            var sut = CreateSutInstance();

            var result = await sut.CreateLinkRequestAsync(Guid.Parse(FakeCustomerId));

            Assert.Equal(LinkingError.CustomerWalletBlocked, result.Error);
        }

        [Fact]
        public async Task ApproveLinkRequestAsync_CustomerDoesNotExist_ReturnsError()
        {
            _requestsRepositoryMock
                .Setup(x => x.GetByPrivateAddressAsync(It.IsAny<string>(), null))
                .ReturnsAsync(new WalletLinkingRequestEntity { CustomerId = FakeCustomerId });

            _signatureValidatorMock
                .Setup(x => x.Validate(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(false);

            _wmClientMock.Setup(x => x.Api.GetCustomerWalletBlockStateAsync(FakeCustomerId))
                .ReturnsAsync(new CustomerWalletBlockStatusResponse { Error = CustomerWalletBlockStatusError.CustomerNotFound});

            var sut = CreateSutInstance();

            var result = await sut.ApproveLinkRequestAsync(FakeCustomerWalletAddress,
                FakeCustomerWalletAddress, FakeSignature);

            Assert.Equal(LinkingError.CustomerDoesNotExist, result.Error);
        }

        [Fact]
        public async Task ApproveLinkRequestAsync_CustomerWalletIsBlocked_ReturnsError()
        {
            _requestsRepositoryMock
                .Setup(x => x.GetByPrivateAddressAsync(It.IsAny<string>(), null))
                .ReturnsAsync(new WalletLinkingRequestEntity { CustomerId = FakeCustomerId });

            _signatureValidatorMock
                .Setup(x => x.Validate(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(false);

            _wmClientMock.Setup(x => x.Api.GetCustomerWalletBlockStateAsync(FakeCustomerId))
                .ReturnsAsync(new CustomerWalletBlockStatusResponse { Status = CustomerWalletActivityStatus.Blocked});

            var sut = CreateSutInstance();

            var result = await sut.ApproveLinkRequestAsync(FakeCustomerWalletAddress,
                FakeCustomerWalletAddress, FakeSignature);

            Assert.Equal(LinkingError.CustomerWalletBlocked, result.Error);
        }

        private IWalletLinker CreateSutInstance()
        {
            return new WalletLinker(_requestsRepositoryMock.Object,
                EmptyLogFactory.Instance,
                LinkCodeLength,
                _pbfClientMock.Object,
                _settingsServiceMock.Object,
                _requestedPublisherMock.Object,
                _completedPublisherMock.Object,
                _finalizedPublisherMock.Object,
                _signatureValidatorMock.Object,
                _countersRepositoryMock.Object,
                _customersServiceMock.Object,
                _wmClientMock.Object,
                new SqlContextFactoryFake<WalletLinkingContext>(x => new WalletLinkingContext(x)));
        }
    }
}
