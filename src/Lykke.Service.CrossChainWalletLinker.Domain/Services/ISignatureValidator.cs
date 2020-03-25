namespace Lykke.Service.CrossChainWalletLinker.Domain.Services
{
    public interface ISignatureValidator
    {
        bool Validate(string plainMessage, string signature, string publicAddress);
    }
}
