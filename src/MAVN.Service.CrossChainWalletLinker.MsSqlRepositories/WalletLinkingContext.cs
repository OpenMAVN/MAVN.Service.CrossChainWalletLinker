using System.Data.Common;
using MAVN.Persistence.PostgreSQL.Legacy;
using MAVN.Service.CrossChainWalletLinker.Domain.Enums;
using MAVN.Service.CrossChainWalletLinker.MsSqlRepositories.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace MAVN.Service.CrossChainWalletLinker.MsSqlRepositories
{
    public class WalletLinkingContext : PostgreSQLContext
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

        protected override void OnMAVNModelCreating(ModelBuilder modelBuilder)
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
