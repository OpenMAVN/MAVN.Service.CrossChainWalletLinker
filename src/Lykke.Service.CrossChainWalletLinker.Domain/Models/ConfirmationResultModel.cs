using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Lykke.Service.CrossChainWalletLinker.Domain.Models
{
    public class ConfirmationResultModel
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public ConfirmationError Error { get; private set; }

        public static ConfirmationResultModel Succeeded()
        {
            return new ConfirmationResultModel
            {
                Error = ConfirmationError.None
            };
        }

        public static ConfirmationResultModel Failed(ConfirmationError error)
        {
            return new ConfirmationResultModel
            {
                Error = error
            };       
        }
    }
}
