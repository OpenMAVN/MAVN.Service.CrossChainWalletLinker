using System.Data.Common;
using Lykke.Common.MsSql;
using Lykke.Service.CrossChainWalletLinker.Domain.Enums;
using Lykke.Service.CrossChainWalletLinker.MsSqlRepositories.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Lykke.Service.CrossChainWalletLinker.MsSqlRepositories
{
    public class WalletLinkingContext : MsSqlContext
    {
        private const string Schema = "cross_chain_wallet_linker";

        internal DbSet<WalletLinkingRequestEntity> LinkingRequests { get; set; }
        
        internal DbSet<WalletLinkingRequestsCounterEntity> LinkingRequestsCounter { get; set; }
        
        internal DbSet<ConfigurationItemEntity> ConfigurationItems { get; set; }

        public WalletLinkingContext() 
            : base(Schema)
        {
        }

        public WalletLinkingContext(string connectionString, bool isTraceEnabled) 
            : base(Schema, connectionString, isTraceEnabled)
        {
        }
        
        public WalletLinkingContext(DbConnection dbConnection) : base(Schema, dbConnection)
        {
        }

        public WalletLinkingContext(DbContextOptions contextOptions) : base(Schema, contextOptions)
        {
        }

        protected override void OnLykkeConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
        }

        protected override void OnLykkeModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<WalletLinkingRequestEntity>()
                .HasIndex(e => e.PrivateAddress)
                .IsUnique();

            modelBuilder.Entity<ConfigurationItemEntity>()
                .HasIndex(e => e.Type).IsUnique();
            
            modelBuilder.Entity<ConfigurationItemEntity>()
                .Property(e => e.Type)
                .HasConversion(new EnumToStringConverter<ConfigurationItemType>());
        }

    }
}
