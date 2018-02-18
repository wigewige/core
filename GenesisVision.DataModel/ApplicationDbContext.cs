using GenesisVision.DataModel.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;

namespace GenesisVision.DataModel
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>
    {
        public ApplicationDbContext(DbContextOptions options)
            : base(options)
        {
        }
        
        public DbSet<Profiles> Profiles { get; set; }
        public DbSet<BrokersAccounts> BrokersAccounts { get; set; }
        public DbSet<BrokerTradeServers> BrokerTradeServers { get; set; }
        public DbSet<ManagerAccounts> ManagersAccounts { get; set; }
        public DbSet<ManagersAccountsStatistics> ManagersAccountsStatistics { get; set; }
        public DbSet<InvestorAccounts> InvestorAccounts { get; set; }
        public DbSet<ManagerRequests> ManagerRequests { get; set; }
        public DbSet<InvestmentPrograms> InvestmentPrograms { get; set; }
        public DbSet<Periods> Periods { get; set; }
        public DbSet<InvestmentRequests> InvestmentRequests { get; set; }
        public DbSet<ManagerTokens> ManagerTokens { get; set; }
        public DbSet<Portfolios> Portfolios { get; set; }
        public DbSet<Wallets> Wallets { get; set; }
        public DbSet<IOTransactions> IOTransactions { get; set; }
        public DbSet<WalletTransactions> WalletTransactions { get; set; }
        public DbSet<BlockchainAddresses> EthAddresses { get; set; }
        public DbSet<PaymentTransactions> PaymentTransactions { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            
            builder.Entity<ManagerAccounts>()
                   .HasOne(x => x.User)
                   .WithMany(x => x.ManagerAccounts)
                   .HasForeignKey(x => x.UserId);


            builder.Entity<BrokersAccounts>()
                   .HasIndex(x => x.Name)
                   .IsUnique();

            builder.Entity<ApplicationUser>()
                   .HasOne(x => x.BrokersAccount)
                   .WithOne(x => x.User)
                   .HasForeignKey<BrokersAccounts>(x => x.UserId);

            builder.Entity<ApplicationUser>()
                   .HasOne(x => x.Profile)
                   .WithOne(x => x.User)
                   .HasForeignKey<Profiles>(x => x.UserId);

            builder.Entity<Profiles>()
                   .HasKey(x => x.UserId);

            builder.Entity<Profiles>()
                   .HasIndex(x => x.UserName)
                   .IsUnique();

            builder.Entity<ApplicationUser>()
                   .HasOne(x => x.Wallet)
                   .WithOne(x => x.User)
                   .HasForeignKey<Wallets>(x => x.UserId);

            builder.Entity<Wallets>()
                   .HasKey(x => x.UserId);

            builder.Entity<BrokerTradeServers>()
                   .HasOne(x => x.Broker)
                   .WithMany(x => x.BrokerTradeServers)
                   .HasForeignKey(x => x.BrokerId);

            builder.Entity<BrokerTradeServers>()
                   .HasIndex(x => x.Host)
                   .IsUnique();


            builder.Entity<ManagerAccounts>()
                   .HasOne(x => x.BrokerTradeServer)
                   .WithMany(x => x.ManagerAccounts)
                   .HasForeignKey(x => x.BrokerTradeServerId);


            builder.Entity<ManagersAccountsStatistics>()
                   .HasOne(x => x.User)
                   .WithMany(x => x.ManagersAccountsStatistics)
                   .HasForeignKey(x => x.UserId);

            builder.Entity<ManagersAccountsStatistics>()
                   .HasOne(x => x.ManagerAccount)
                   .WithMany(x => x.ManagersAccountsStatistics)
                   .HasForeignKey(x => x.ManagerAccountId);

            builder.Entity<ManagersAccountsStatistics>()
                   .HasOne(x => x.Period)
                   .WithMany(x => x.ManagersAccountsStatistics)
                   .HasForeignKey(x => x.PeriodId);

            builder.Entity<ManagersAccountsStatistics>()
                   .HasOne(x => x.InvestmentProgram)
                   .WithMany(x => x.ManagersAccountsStatistics)
                   .HasForeignKey(x => x.InvestmentProgramId);

            builder.Entity<ManagerTokens>()
                   .HasIndex(x => x.TokenName)
                   .IsUnique();

            builder.Entity<ManagerTokens>()
                   .HasIndex(x => x.TokenSymbol)
                   .IsUnique();

            builder.Entity<ManagerAccounts>()
                   .HasOne(x => x.InvestmentProgram)
                   .WithOne(x => x.ManagerAccount)
                   .HasForeignKey<InvestmentPrograms>(x => x.ManagerAccountId);


            builder.Entity<InvestmentPrograms>()
                   .HasIndex(x => x.Title)
                   .IsUnique();

            builder.Entity<InvestmentRequests>()
                   .HasOne(x => x.User)
                   .WithMany(x => x.InvestmentRequests)
                   .HasForeignKey(x => x.UserId);

            builder.Entity<InvestmentRequests>()
                   .HasOne(x => x.InvestorAccount)
                   .WithMany(x => x.InvestmentRequests)
                   .HasForeignKey(x => x.UserId);

            builder.Entity<InvestmentRequests>()
                   .HasOne(x => x.InvestmentProgram)
                   .WithMany(x => x.InvestmentRequests)
                   .HasForeignKey(x => x.InvestmentProgramtId);

            builder.Entity<InvestmentRequests>()
                   .HasOne(x => x.Period)
                   .WithMany(x => x.InvestmentRequests)
                   .HasForeignKey(x => x.PeriodId);


            builder.Entity<ManagerRequests>()
                   .HasOne(x => x.BrokerTradeServers)
                   .WithMany(x => x.ManagerAccountRequests)
                   .HasForeignKey(x => x.BrokerTradeServerId);

            builder.Entity<ManagerRequests>()
                   .HasOne(x => x.User)
                   .WithMany(x => x.ManagerAccountRequests)
                   .HasForeignKey(x => x.UserId);

            builder.Entity<ManagerRequests>()
                   .HasIndex(x => x.TokenName)
                   .IsUnique();

            builder.Entity<ManagerRequests>()
                   .HasIndex(x => x.TokenSymbol)
                   .IsUnique();

            builder.Entity<ManagerRequests>()
                   .HasIndex(x => x.Title)
                   .IsUnique();


            builder.Entity<Periods>()
                   .HasOne(x => x.InvestmentProgram)
                   .WithMany(x => x.Periods)
                   .HasForeignKey(x => x.InvestmentProgramId);


            builder.Entity<InvestorAccounts>()
                   .HasKey(x => x.UserId);

            builder.Entity<ApplicationUser>()
                   .HasOne(x => x.InvestorAccount)
                   .WithOne(x => x.User)
                   .HasForeignKey<InvestorAccounts>(x => x.UserId);
            
            //builder.Entity<InvestorAccounts>()
            //       .HasOne(x => x.Portfolios)
            //       .WithMany(x => x.InvestorAccount)
            //       .HasForeignKey<Portfolios>(x => x.InvestorAccountId);

            builder.Entity<Portfolios>()
                   .HasKey(x => x.InvestorAccountId);

            builder.Entity<ManagerTokens>()
                   .HasOne(x => x.InvestmentProgram)
                   .WithOne(x => x.Token)
                   .HasForeignKey<InvestmentPrograms>(x => x.ManagerTokensId);

            builder.Entity<IOTransactions>()
                   .HasOne(x => x.User)
                   .WithMany(x => x.IOTransactions)
                   .HasForeignKey(x => x.UserId);

            builder.Entity<IOTransactions>()
                   .HasOne(x => x.Wallet)
                   .WithMany(x => x.IOTransactions)
                   .HasForeignKey(x => x.WalletId);

            builder.Entity<WalletTransactions>()
                   .HasOne(x => x.IOTransaction)
                   .WithOne(x => x.WalletTransaction)
                   .HasForeignKey<IOTransactions>(x => x.WalletTransactionId);

            builder.Entity<WalletTransactions>()
                   .HasOne(x => x.User)
                   .WithMany(x => x.WalletTransactions)
                   .HasForeignKey(x => x.UserId);

            builder.Entity<WalletTransactions>()
                   .HasOne(x => x.Wallet)
                   .WithMany(x => x.WalletTransactions)
                   .HasForeignKey(x => x.UserId);

            builder.Entity<WalletTransactions>()
                   .HasOne(x => x.InvestmentRequest)
                   .WithOne(x => x.WalletTransaction)
                   .HasForeignKey<InvestmentRequests>(x => x.WalletTransactionId);
        }
    }
}
