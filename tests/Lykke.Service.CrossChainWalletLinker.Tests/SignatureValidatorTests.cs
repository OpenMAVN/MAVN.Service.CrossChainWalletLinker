using System;
using Lykke.Logs;
using Lykke.Service.CrossChainWalletLinker.Domain.Services;
using Lykke.Service.CrossChainWalletLinker.DomainServices;
using Xunit;

namespace Lykke.Service.CrossChainWalletLinker.Tests
{
    public class SignatureValidatorTests
    {
        #region Consts
        
        private const string ValidCustomerWalletAddress = "0x39a740bc7443cb5fd32d9a097bf75d07a5023c9c";

        private const string ValidSignature = "0x58f55436f0121a742812a74a7de671ae76e6194b40aebb49c0de468bbda2afee3939993ab5e0bcb697f8b01440a0a83f4752691f7294a1b1cfec3d1dbe2a94571c";

        private const string InvalidSignature = "0x80ed2f72bbd511c12fa1fc8fa66ac594be30fd1b3263524832cf2524e16587151efe3778b77bb984b714d80f599627006ab3996cdb9081f2fc9c241820607f6c1b";

        private const string TextToSign = "Using Nethereum for message signing";
        
        #endregion
        
        [Theory]
        [InlineData("", ValidSignature, ValidCustomerWalletAddress)]
        [InlineData(null, ValidSignature, ValidCustomerWalletAddress)]
        [InlineData(TextToSign, "", ValidCustomerWalletAddress)]
        [InlineData(TextToSign, null, ValidCustomerWalletAddress)]
        [InlineData(TextToSign, ValidSignature, "")]
        [InlineData(TextToSign, ValidSignature, null)]
        [InlineData("", "", "")]
        [InlineData(null, null, null)]
        public void Validate_InvalidParameters_RaisesException(string plainMessage, 
            string signature, string publicAddress)
        {
            var sut = CreateSutInstance();

            Assert.Throws<ArgumentNullException>(() => sut.Validate(plainMessage, signature, publicAddress));
        }

        [Fact]
        public void Validate_ValidSignature_ReturnsSuccess()
        {
            var sut = CreateSutInstance();

            var result = sut.Validate(TextToSign, ValidSignature, ValidCustomerWalletAddress);
            
            Assert.True(result);
        }

        [Fact]
        public void Validate_InvalidSignature_ReturnsFail()
        {
            var sut = CreateSutInstance();

            var result = sut.Validate(TextToSign, InvalidSignature, ValidCustomerWalletAddress);
            
            Assert.False(result);
        }

        private ISignatureValidator CreateSutInstance()
        {
            return new SignatureValidator(EmptyLogFactory.Instance);
        }
    }
}
