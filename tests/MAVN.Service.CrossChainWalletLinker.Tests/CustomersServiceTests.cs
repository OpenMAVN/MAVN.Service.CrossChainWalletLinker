using System;
using System.Threading.Tasks;
using MAVN.Numerics;
using MAVN.Common.MsSql;
using Lykke.Logs;
using MAVN.Service.CrossChainWalletLinker.Domain.Enums;
using MAVN.Service.CrossChainWalletLinker.Domain.Models;
using MAVN.Service.CrossChainWalletLinker.Domain.Repositories;
using MAVN.Service.CrossChainWalletLinker.Domain.Services;
using MAVN.Service.CrossChainWalletLinker.DomainServices;
using MAVN.Service.CrossChainWalletLinker.MsSqlRepositories.Entities;
using Moq;
using Xunit;

namespace MAVN.Service.CrossChainWalletLinker.Tests
{
    public class CustomersServiceTests
    {
        private readonly Mock<IWalletLinkingRequestsRepository> _linkRequestRepositoryMock = new Mock<IWalletLinkingRequestsRepository>();
        
        private readonly Mock<IWalletLinkingRequestsCounterRepository> _linkCountersRepositoryMock = new Mock<IWalletLinkingRequestsCounterRepository>();
        
        private readonly Mock<IConfigurationItemsRepository> _configurationItemsRepositoryMock = new Mock<IConfigurationItemsRepository>();
        
        private const string FakeCustomerId = "3f1443e2-b848-4567-8fb5-ebe7337a87e9";
        
        private const string FakeCustomerWalletAddress = "0x7d275eb17ceaae04b17768d4459741bae062ee09";

        [Fact]
        public async Task GetPublicAddressAsync_InvalidParameters_ReturnsError()
        {
            var sut = CreateSutInstance();

            var result = await sut.GetPublicAddressAsync(Guid.Empty);
            
            Assert.Equal(PublicAddressError.InvalidCustomerId, result.Error);
            Assert.Null(result.PublicAddress);
        }

        [Fact]
        public async Task GetPublicAddressAsync_NoLinkingRequest_ReturnsNotLinked()
        {
            _linkRequestRepositoryMock
                .Setup(x => x.GetByIdAsync(It.IsAny<string>(), null))
                .ReturnsAsync((IWalletLinkingRequest) null);

            var sut = CreateSutInstance();

            var result = await sut.GetPublicAddressAsync(Guid.Parse(FakeCustomerId));
            
            Assert.Equal(PublicAddressError.None, result.Error);
            Assert.True(string.IsNullOrEmpty(result.PublicAddress));
            Assert.Equal(PublicAddressStatus.NotLinked, result.Status);
        }

        [Fact]
        public async Task GetPublicAddressAsync_RequestHasNotBeenApprovedYet_ReturnsPendingApproval()
        {
            _linkRequestRepositoryMock
                .Setup(x => x.GetByIdAsync(It.IsAny<string>(), null))
                .ReturnsAsync(new WalletLinkingRequestEntity
                {
                    PublicAddress = null,
                    Signature = null,
                    IsConfirmedInPrivate = false,
                    IsConfirmedInPublic = false
                });
            
            var sut = CreateSutInstance();

            var result = await sut.GetPublicAddressAsync(Guid.Parse(FakeCustomerId));
            
            Assert.Equal(PublicAddressError.None, result.Error);
            Assert.Equal(PublicAddressStatus.PendingCustomerApproval, result.Status);
            Assert.True(string.IsNullOrEmpty(result.PublicAddress));
        }

        [Theory]
        [InlineData(false, false)]
        [InlineData(true, false)]
        [InlineData(false, true)]
        public async Task GetPublicAddressAsync_NotConfirmedInBlockchainYet_ReturnsPendingConfirmation(bool confirmedInPrivate, bool confirmedInPublic)
        {
            _linkRequestRepositoryMock
                .Setup(x => x.GetByIdAsync(It.IsAny<string>(), null))
                .ReturnsAsync(new WalletLinkingRequestEntity
                {
                    PublicAddress = FakeCustomerWalletAddress,
                    Signature = "whatever",
                    IsConfirmedInPrivate = confirmedInPrivate, 
                    IsConfirmedInPublic = confirmedInPublic
                });
            
            var sut = CreateSutInstance();

            var result = await sut.GetPublicAddressAsync(Guid.Parse(FakeCustomerId));
            
            Assert.Equal(PublicAddressError.None, result.Error);
            Assert.Equal(PublicAddressStatus.PendingConfirmation, result.Status);
            Assert.False(string.IsNullOrEmpty(result.PublicAddress));
        }

        [Fact]
        public async Task GetPublicAddressAsync_WalletIsLinked_ReturnsAddress()
        {
            _linkRequestRepositoryMock
                .Setup(x => x.GetByIdAsync(It.IsAny<string>(), null))
                .ReturnsAsync(new WalletLinkingRequestEntity
                {
                    IsConfirmedInPrivate = true, 
                    IsConfirmedInPublic = true,
                    PublicAddress = FakeCustomerWalletAddress
                });

            var sut = CreateSutInstance();

            var result = await sut.GetPublicAddressAsync(Guid.Parse(FakeCustomerId));
            
            Assert.Equal(PublicAddressError.None, result.Error);
            Assert.Equal(FakeCustomerWalletAddress, result.PublicAddress);
        }

        [Fact]
        public async Task GetNextWalletLinkingFeeAsync_InvalidParameters_RaisesException()
        {
            var sut = CreateSutInstance();

            await Assert.ThrowsAsync<ArgumentNullException>(() => sut.GetNextWalletLinkingFeeAsync(Guid.Empty));
        }

        [Theory]
        [InlineData(0, ConfigurationItemType.FirstTimeLinkingFee)]
        [InlineData(1, ConfigurationItemType.SubsequentLinkingFee)]
        [InlineData(10, ConfigurationItemType.SubsequentLinkingFee)]
        public async Task GetNextWalletLinkingFeeAsync_UsesCorrespondingConfigurationItemType(int approvalsCounter,
            ConfigurationItemType expectedConfigurationItemType)
        {
            _linkCountersRepositoryMock
                .Setup(x => x.GetAsync(It.IsAny<string>(), null))
                .ReturnsAsync(new WalletLinkingRequestsCounterEntity {ApprovalsCounter = approvalsCounter});

            var configurationItemType = ConfigurationItemType.FirstTimeLinkingFee;

            _configurationItemsRepositoryMock
                .Setup(x => x.GetAsync(It.IsAny<ConfigurationItemType>(), null))
                .Callback<ConfigurationItemType, TransactionContext>((type, txContext) => configurationItemType = type)
                .ReturnsAsync(new ConfigurationItemEntity {Value = "whatever"});

            var sut = CreateSutInstance();

            await sut.GetNextWalletLinkingFeeAsync(Guid.Parse(FakeCustomerId));

            Assert.Equal(expectedConfigurationItemType, configurationItemType);
        }

        [Theory]
        [InlineData("", 0)]
        [InlineData("whatever", 0)]
        [InlineData("100", 100)]
        public async Task GetNextWalletLinkingFeeAsync_CannotParseConfigurationValue_ReturnsZero(string configurationValue, decimal expectedParsedValue)
        {
            _configurationItemsRepositoryMock
                .Setup(x => x.GetAsync(It.IsAny<ConfigurationItemType>(), null))
                .ReturnsAsync(new ConfigurationItemEntity {Value = configurationValue});
            
            var sut = CreateSutInstance();

            var result = await sut.GetNextWalletLinkingFeeAsync(Guid.Parse(FakeCustomerId));
            
            Assert.Equal(Money18.Create(expectedParsedValue), result);
        }

        private ICustomersService CreateSutInstance()
        {
            return new CustomersService(_linkRequestRepositoryMock.Object,
                _linkCountersRepositoryMock.Object,
                _configurationItemsRepositoryMock.Object,
                EmptyLogFactory.Instance);
        }
    }
}
