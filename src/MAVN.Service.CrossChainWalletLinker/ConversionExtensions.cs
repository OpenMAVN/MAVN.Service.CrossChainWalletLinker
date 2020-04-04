using System;
using MAVN.Service.CrossChainWalletLinker.Domain.Enums;

namespace MAVN.Service.CrossChainWalletLinker
{
    public static class ConversionExtensions
    {
        public static ConfigurationItemType? ToDomain(this Client.Models.ConfigurationItemType src)
        {
            try
            {
                return Enum.Parse<ConfigurationItemType>(src.ToString());
            }
            catch (ArgumentException)
            {
                return null;
            }
        }
    }
}
