using Microsoft.EntityFrameworkCore;
namespace GV.Payment.DataModel
{
	public partial class PaymentContext : DbContext
    {
        public virtual DbSet<PaymentTransaction> PaymentTransaction { get; set; }

		public virtual DbSet<AddressStorage> AddressStorage { get; set; }
		public virtual DbSet<Wallet> Wallet { get; set; }

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			//if (!optionsBuilder.IsConfigured)
			//{
			//	optionsBuilder.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=EFProviders.InMemory;Trusted_Connection=True;");
			//}
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
			//	modelBuilder.Entity<AspNetUsers>(entity =>
			//{
			//	entity.HasIndex(e => e.NormalizedEmail)
			//		.HasName("EmailIndex");

			//	entity.HasIndex(e => e.NormalizedUserName)
			//		.HasName("UserNameIndex")
			//		.IsUnique();

			//	entity.Property(e => e.Id).HasMaxLength(450);

			//	entity.Property(e => e.Email).HasMaxLength(256);

			//	entity.Property(e => e.NormalizedEmail).HasMaxLength(256);

			//	entity.Property(e => e.NormalizedUserName)
			//		.IsRequired()
			//		.HasMaxLength(256);

			//	entity.Property(e => e.UserName).HasMaxLength(256);
			//});

			modelBuilder.Entity<PaymentTransaction>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Amount).HasColumnType("decimal");

				entity.Property(e => e.Fee).HasColumnType("decimal");

				entity.Property(e => e.Currency)
                    .IsRequired()
                    .HasColumnType("varchar(10)");

                entity.Property(e => e.DateCreated).HasDefaultValueSql("getutcdate()");

                entity.Property(e => e.GatewayCode)
                    .IsRequired()
                    .HasColumnType("varchar(50)");

                entity.Property(e => e.Hash).HasColumnType("varchar(250)");

                entity.Property(e => e.PayoutMinerFee).HasColumnType("decimal");

                entity.Property(e => e.PayoutServiceFee).HasColumnType("decimal");

                entity.Property(e => e.PayoutStatus).HasDefaultValueSql("0");

                entity.Property(e => e.PayoutTxHash).HasColumnType("varchar(100)");

                entity.Property(e => e.Timestamp)
                    .HasColumnType("timestamp")
                    .ValueGeneratedOnAddOrUpdate();

                entity.HasOne(d => d.Wallet)
                    .WithMany(p => p.PaymentTransaction)
                    .HasForeignKey(d => d.WalletId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_Transaction_Wallet");
            });
			
			modelBuilder.Entity<AddressStorage>(entity =>
			{
				entity.Property(e => e.DateCreated).HasDefaultValueSql("getutcdate()");

				entity.Property(e => e.Address)
					.IsRequired()
					.HasColumnType("varchar(200)");

				entity.Property(e => e.Currency)
					.IsRequired()
					.HasColumnType("varchar(10)");

				entity.Property(e => e.GatewayKey)
					.IsRequired()
					.HasColumnType("varchar(100)");
			});

			modelBuilder.Entity<Wallet>(entity =>
            {
                entity.Property(e => e.Id).HasDefaultValueSql("newid()");

                entity.Property(e => e.Address)
                    .IsRequired()
                    .HasColumnType("varchar(150)");

                entity.Property(e => e.Currency)
                    .IsRequired()
                    .HasColumnType("varchar(10)");

                entity.Property(e => e.CustomKey).HasColumnType("varchar(100)");

                entity.Property(e => e.DateCreated).HasDefaultValueSql("getutcdate()");

                entity.Property(e => e.PayoutAddress)
                    .IsRequired()
                    .HasColumnType("varchar(100)");

                entity.Property(e => e.GatewayCode)
                    .IsRequired()
                    .HasColumnType("varchar(50)");

                entity.Property(e => e.GatewayInvoice).HasColumnType("varchar(250)");

                entity.Property(e => e.GatewayKey).HasColumnType("varchar(250)");

                entity.Property(e => e.UserId)
                    .IsRequired()
                    .HasColumnType("varchar(450)");
            });
        }
    }
}