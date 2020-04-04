using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Lykke.Service.PrivateBlockchainFacade.Client;

namespace MAVN.Service.CrossChainWalletLinker.Tests
{
    public class CustomerWalletAddressErrorsTestData : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            var errorsOnly = Enum.GetValues(typeof(CustomerWalletAddressError))
                .Cast<CustomerWalletAddressError>()
                .Except(new[] {CustomerWalletAddressError.None});

            foreach (var customerWalletAddressError in errorsOnly)
            {
                yield return new object[] {customerWalletAddressError};
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
