using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace MAVN.Service.CrossChainWalletLinker.Domain.Models
{
    public class LinkingRequestResultModel
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public LinkingError Error { get; private set; }
        
        public string LinkCode { get; set; }
        
        public static LinkingRequestResultModel Succeeded(string linkCode)
        {
            return new LinkingRequestResultModel
            {
                Error = LinkingError.None,
                LinkCode = linkCode
            };
        }

        public static LinkingRequestResultModel Failed(LinkingError error)
        {
            return new LinkingRequestResultModel
            {
                Error = error,
                LinkCode = null
            };       
        }
    }
}
