using GenesisVision.Core.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace GenesisVision.Core.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options)
            : base(options)
        {
        }

        public DbSet<AspNetUsers> AspNetUsers { get; set; }
        public DbSet<Brokers> Brokers { get; set; }
        public DbSet<BrokerTradeServers> BrokerTradeServers { get; set; }
        public DbSet<ManagerAccounts> ManagersAccounts { get; set; }
        public DbSet<ManagerAccountRequests> ManagerRequests { get; set; }
        public DbSet<InvestmentPrograms> InvestmentPrograms { get; set; }
        public DbSet<InvestmentRequests> InvestmentRequests { get; set; }
        
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<ManagerAccounts>()
                   .HasOne(x => x.User)
                   .WithMany(x => x.ManagerAccounts)
                   .HasForeignKey(x => x.UserId);

            builder.Entity<BrokerTradeServers>()
                   .HasOne(x => x.Broker)
                   .WithMany(x => x.BrokerTradeServers)
                   .HasForeignKey(x => x.BrokerId);

            builder.Entity<ManagerAccounts>()
                   .HasOne(x => x.BrokerTradeServer)
                   .WithMany(x => x.ManagerAccounts)
                   .HasForeignKey(x => x.BrokerTradeServerId);

            builder.Entity<InvestmentPrograms>()
                   .HasOne(x => x.ManagersAccount)
                   .WithMany(x => x.InvestmentPrograms)
                   .HasForeignKey(x => x.ManagersAccountId);

            builder.Entity<InvestmentRequests>()
                   .HasOne(x => x.User)
                   .WithMany(x => x.InvestmentRequests)
                   .HasForeignKey(x => x.UserId);

            builder.Entity<InvestmentRequests>()
                   .HasOne(x => x.InvestmentProgram)
                   .WithMany(x => x.InvestmentRequests)
                   .HasForeignKey(x => x.InvestmentProgramtId);

            builder.Entity<ManagerAccountRequests>()
                   .HasOne(x => x.BrokerTradeServers)
                   .WithMany(x => x.ManagerAccountRequests)
                   .HasForeignKey(x => x.BrokerTradeServerId);

            builder.Entity<ManagerAccountRequests>()
                   .HasOne(x => x.User)
                   .WithMany(x => x.ManagerAccountRequests)
                   .HasForeignKey(x => x.UserId);
        }
    }
}
