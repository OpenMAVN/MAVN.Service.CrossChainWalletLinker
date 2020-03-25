using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Lykke.Service.CrossChainWalletLinker.Domain.Models
{
    public class LinkingApprovalResultModel
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public LinkingError Error { get; private set; }
        
        public static LinkingApprovalResultModel Succeeded()
        {
            return new LinkingApprovalResultModel
            {
                Error = LinkingError.None
            };
        }

        public static LinkingApprovalResultModel Failed(LinkingError error)
        {
            return new LinkingApprovalResultModel
            {
                Error = error
            };       
        }
    }
}
