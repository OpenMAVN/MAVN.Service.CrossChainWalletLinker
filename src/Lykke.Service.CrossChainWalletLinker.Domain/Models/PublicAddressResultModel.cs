using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Lykke.Service.CrossChainWalletLinker.Domain.Models
{
    public class PublicAddressResultModel
    {
        public string PublicAddress { get; private set; }
        
        [JsonConverter(typeof(StringEnumConverter))]
        public PublicAddressStatus Status { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public PublicAddressError Error { get; private set; }
        
        public static PublicAddressResultModel Succeeded(string address, PublicAddressStatus status)
        {
            return new PublicAddressResultModel
            {
                Error = PublicAddressError.None,
                PublicAddress = address,
                Status = status
            };
        }

        public static PublicAddressResultModel Failed(PublicAddressError error, PublicAddressStatus status = PublicAddressStatus.NotLinked)
        {
            return new PublicAddressResultModel
            {
                Error = error,
                PublicAddress = null,
                Status = status
            };       
        }
    }
}
