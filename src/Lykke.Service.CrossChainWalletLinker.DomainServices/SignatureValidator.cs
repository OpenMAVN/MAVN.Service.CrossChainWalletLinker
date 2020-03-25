using System;
using Common.Log;
using Lykke.Common.Log;
using Lykke.Service.CrossChainWalletLinker.Domain.Services;
using Nethereum.Signer;

namespace Lykke.Service.CrossChainWalletLinker.DomainServices
{
    public class SignatureValidator : ISignatureValidator
    {
        private readonly EthereumMessageSigner _signer;
        private readonly ILog _log;

        public SignatureValidator(ILogFactory logFactory)
        {
            _log = logFactory.CreateLog(this);
            _signer = new EthereumMessageSigner();
        }

        public bool Validate(string plainMessage, string signature, string publicAddress)
        {
            if (string.IsNullOrEmpty(plainMessage))
                throw new ArgumentNullException(nameof(plainMessage));
            
            if (string.IsNullOrEmpty(signature))
                throw new ArgumentNullException(nameof(signature));
            
            if (string.IsNullOrEmpty(publicAddress))
                throw new ArgumentNullException(nameof(publicAddress));

            string recoveredAddress;
            try
            {
                recoveredAddress = _signer.EncodeUTF8AndEcRecover(plainMessage, signature);
            }
            catch (FormatException e)
            {
                _log.Warning(e.Message, e, new {plainMessage, signature, publicAddress});
                return false;
            }

            return string.Equals(recoveredAddress, publicAddress, StringComparison.CurrentCultureIgnoreCase);
        }
    }
}
