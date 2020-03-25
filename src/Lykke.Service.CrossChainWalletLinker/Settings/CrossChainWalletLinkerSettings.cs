using JetBrains.Annotations;

namespace Lykke.Service.CrossChainWalletLinker.Settings
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class CrossChainWalletLinkerSettings
    {
        public DbSettings Db { get; set; }
        
        public RabbitMqSettings RabbitMq { get; set; }
        
        public int LinkCodeLength { get; set; }
        
        public string PrivateBlockchainGatewayContractAddress { get; set; }

        public string MasterWalletAddress { get; set; }

        public NotificationsSettings.NotificationsSettings Notifications { get; set; }
    }
}
