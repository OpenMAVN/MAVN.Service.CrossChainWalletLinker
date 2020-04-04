using Lykke.SettingsReader.Attributes;

namespace MAVN.Service.CrossChainWalletLinker.Settings
{
    public class RabbitMqSettings
    {
        [AmqpCheck]
        public string RabbitMqConnectionString { get; set; }

        [AmqpCheck]
        public string NotificationRabbitMqConnectionString { get; set; }
    }
}
