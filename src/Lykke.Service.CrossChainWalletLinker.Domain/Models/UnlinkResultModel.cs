using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Lykke.Service.CrossChainWalletLinker.Domain.Models
{
    public class UnlinkResultModel
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public LinkingError Error { get; private set; }
        
        public static UnlinkResultModel Succeeded()
        {
            return new UnlinkResultModel
            {
                Error = LinkingError.None
            };
        }

        public static UnlinkResultModel Failed(LinkingError error)
        {
            return new UnlinkResultModel
            {
                Error = error
            };       
        }
    }
}
