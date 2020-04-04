using System;

namespace MAVN.Service.CrossChainWalletLinker.Domain.Exceptions
{
    public class LinkingRequestAlreadyExistsException : Exception
    {
        public LinkingRequestAlreadyExistsException(string customerId, string message = null) : base(message ?? "Linking request already exists")
        {
            CustomerId = customerId;
        }

        public string CustomerId { get; }
    }
}
