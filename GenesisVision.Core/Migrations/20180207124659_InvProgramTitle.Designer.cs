﻿// <auto-generated />
using GenesisVision.DataModel;
using GenesisVision.DataModel.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.Internal;
using System;

namespace GenesisVision.Core.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20180207124659_InvProgramTitle")]
    partial class InvProgramTitle
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn)
                .HasAnnotation("ProductVersion", "2.0.1-rtm-125");

            modelBuilder.Entity("GenesisVision.DataModel.Models.ApplicationRole", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<string>("Name")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedName")
                        .HasMaxLength(256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasName("RoleNameIndex");

                    b.ToTable("AspNetRoles");
                });

            modelBuilder.Entity("GenesisVision.DataModel.Models.ApplicationUser", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("AccessFailedCount");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<string>("Email")
                        .HasMaxLength(256);

                    b.Property<bool>("EmailConfirmed");

                    b.Property<bool>("IsEnabled");

                    b.Property<bool>("LockoutEnabled");

                    b.Property<DateTimeOffset?>("LockoutEnd");

                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(256);

                    b.Property<string>("PasswordHash");

                    b.Property<string>("PhoneNumber");

                    b.Property<bool>("PhoneNumberConfirmed");

                    b.Property<string>("SecurityStamp");

                    b.Property<bool>("TwoFactorEnabled");

                    b.Property<int>("Type");

                    b.Property<string>("UserName")
                        .HasMaxLength(256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasName("UserNameIndex");

                    b.ToTable("AspNetUsers");
                });

            modelBuilder.Entity("GenesisVision.DataModel.Models.BrokersAccounts", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Description");

                    b.Property<bool>("IsEnabled");

                    b.Property<string>("Logo");

                    b.Property<string>("Name");

                    b.Property<DateTime>("RegistrationDate");

                    b.Property<Guid>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.HasIndex("UserId")
                        .IsUnique();

                    b.ToTable("BrokersAccounts");
                });

            modelBuilder.Entity("GenesisVision.DataModel.Models.BrokerTradeServers", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid>("BrokerId");

                    b.Property<string>("Host");

                    b.Property<bool>("IsEnabled");

                    b.Property<string>("Name");

                    b.Property<DateTime>("RegistrationDate");

                    b.Property<int>("Type");

                    b.HasKey("Id");

                    b.HasIndex("BrokerId");

                    b.HasIndex("Host")
                        .IsUnique();

                    b.ToTable("BrokerTradeServers");
                });

            modelBuilder.Entity("GenesisVision.DataModel.Models.InvestmentPrograms", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("DateFrom");

                    b.Property<DateTime?>("DateTo");

                    b.Property<string>("Description");

                    b.Property<decimal>("FeeEntrance");

                    b.Property<decimal>("FeeManagement");

                    b.Property<decimal>("FeeSuccess");

                    b.Property<decimal?>("InvestMaxAmount");

                    b.Property<decimal>("InvestMinAmount");

                    b.Property<bool>("IsEnabled");

                    b.Property<string>("Logo");

                    b.Property<Guid>("ManagerAccountId");

                    b.Property<Guid>("ManagerTokensId");

                    b.Property<int>("OrdersCount");

                    b.Property<int>("Period");

                    b.Property<decimal>("Rating");

                    b.Property<string>("Title");

                    b.Property<decimal>("TotalProfit");

                    b.HasKey("Id");

                    b.HasIndex("ManagerAccountId")
                        .IsUnique();

                    b.HasIndex("ManagerTokensId")
                        .IsUnique();

                    b.HasIndex("Title")
                        .IsUnique();

                    b.ToTable("InvestmentPrograms");
                });

            modelBuilder.Entity("GenesisVision.DataModel.Models.InvestmentRequests", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<decimal>("Amount");

                    b.Property<DateTime>("Date");

                    b.Property<Guid>("InvestmentProgramtId");

                    b.Property<Guid>("PeriodId");

                    b.Property<int>("Status");

                    b.Property<int>("Type");

                    b.Property<Guid>("UserId");

                    b.Property<Guid?>("WalletTransactionId");

                    b.HasKey("Id");

                    b.HasIndex("InvestmentProgramtId");

                    b.HasIndex("PeriodId");

                    b.HasIndex("UserId");

                    b.HasIndex("WalletTransactionId")
                        .IsUnique();

                    b.ToTable("InvestmentRequests");
                });

            modelBuilder.Entity("GenesisVision.DataModel.Models.InvestorAccounts", b =>
                {
                    b.Property<Guid>("UserId");

                    b.HasKey("UserId");

                    b.ToTable("InvestorAccounts");
                });

            modelBuilder.Entity("GenesisVision.DataModel.Models.IOTransactions", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<decimal>("Amount");

                    b.Property<DateTime>("ConfirmationDate");

                    b.Property<DateTime>("CreationDate");

                    b.Property<string>("Currency");

                    b.Property<int>("Status");

                    b.Property<int>("Type");

                    b.Property<Guid>("UserId");

                    b.Property<Guid>("WalletId");

                    b.Property<Guid>("WalletTransactionId");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.HasIndex("WalletId");

                    b.HasIndex("WalletTransactionId")
                        .IsUnique();

                    b.ToTable("IOTransactions");
                });

            modelBuilder.Entity("GenesisVision.DataModel.Models.ManagerAccounts", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid>("BrokerTradeServerId");

                    b.Property<string>("Currency");

                    b.Property<string>("IpfsHash");

                    b.Property<bool>("IsConfirmed");

                    b.Property<string>("Login");

                    b.Property<DateTime>("RegistrationDate");

                    b.Property<Guid>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("BrokerTradeServerId");

                    b.HasIndex("UserId");

                    b.ToTable("ManagersAccounts");
                });

            modelBuilder.Entity("GenesisVision.DataModel.Models.ManagerRequests", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid>("BrokerTradeServerId");

                    b.Property<DateTime>("Date");

                    b.Property<DateTime>("DateFrom");

                    b.Property<DateTime?>("DateTo");

                    b.Property<decimal>("DepositAmount");

                    b.Property<string>("Description");

                    b.Property<decimal>("FeeEntrance");

                    b.Property<decimal>("FeeManagement");

                    b.Property<decimal>("FeeSuccess");

                    b.Property<decimal?>("InvestMaxAmount");

                    b.Property<decimal>("InvestMinAmount");

                    b.Property<string>("Logo");

                    b.Property<int>("Period");

                    b.Property<int>("Status");

                    b.Property<string>("Title");

                    b.Property<string>("TokenName");

                    b.Property<string>("TokenSymbol");

                    b.Property<string>("TradePlatformCurrency");

                    b.Property<string>("TradePlatformPassword");

                    b.Property<int>("Type");

                    b.Property<Guid>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("BrokerTradeServerId");

                    b.HasIndex("Title")
                        .IsUnique();

                    b.HasIndex("TokenName")
                        .IsUnique();

                    b.HasIndex("TokenSymbol")
                        .IsUnique();

                    b.HasIndex("UserId");

                    b.ToTable("ManagerRequests");
                });

            modelBuilder.Entity("GenesisVision.DataModel.Models.ManagersAccountsStatistics", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<decimal>("CurrentBalance");

                    b.Property<DateTime>("Date");

                    b.Property<Guid>("InvestmentProgramId");

                    b.Property<Guid>("ManagerAccountId");

                    b.Property<Guid>("PeriodId");

                    b.Property<decimal>("Profit");

                    b.Property<Guid>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("InvestmentProgramId");

                    b.HasIndex("ManagerAccountId");

                    b.HasIndex("PeriodId");

                    b.HasIndex("UserId");

                    b.ToTable("ManagersAccountsStatistics");
                });

            modelBuilder.Entity("GenesisVision.DataModel.Models.ManagerTokens", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid?>("PortfoliosInvestorAccountId");

                    b.Property<string>("TokenAddress");

                    b.Property<string>("TokenName");

                    b.Property<string>("TokenSymbol");

                    b.HasKey("Id");

                    b.HasIndex("PortfoliosInvestorAccountId");

                    b.HasIndex("TokenName")
                        .IsUnique();

                    b.HasIndex("TokenSymbol")
                        .IsUnique();

                    b.ToTable("ManagerTokens");
                });

            modelBuilder.Entity("GenesisVision.DataModel.Models.Periods", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("DateFrom");

                    b.Property<DateTime>("DateTo");

                    b.Property<Guid>("InvestmentProgramId");

                    b.Property<int>("Number");

                    b.Property<decimal>("StartBalance");

                    b.Property<int>("Status");

                    b.HasKey("Id");

                    b.HasIndex("InvestmentProgramId");

                    b.ToTable("Periods");
                });

            modelBuilder.Entity("GenesisVision.DataModel.Models.Portfolios", b =>
                {
                    b.Property<Guid>("InvestorAccountId");

                    b.HasKey("InvestorAccountId");

                    b.ToTable("Portfolios");
                });

            modelBuilder.Entity("GenesisVision.DataModel.Models.Profiles", b =>
                {
                    b.Property<Guid>("UserId");

                    b.Property<string>("Address");

                    b.Property<string>("Avatar");

                    b.Property<DateTime>("Birthday");

                    b.Property<string>("City");

                    b.Property<string>("Country");

                    b.Property<string>("DocumentNumber");

                    b.Property<string>("DocumentType");

                    b.Property<string>("FirstName");

                    b.Property<bool>("Gender");

                    b.Property<string>("LastName");

                    b.Property<string>("MiddleName");

                    b.Property<string>("Phone");

                    b.Property<string>("UserName");

                    b.HasKey("UserId");

                    b.HasIndex("UserName")
                        .IsUnique();

                    b.ToTable("Profiles");
                });

            modelBuilder.Entity("GenesisVision.DataModel.Models.Wallets", b =>
                {
                    b.Property<Guid>("UserId");

                    b.Property<string>("Address");

                    b.Property<decimal>("Amount");

                    b.HasKey("UserId");

                    b.ToTable("Wallets");
                });

            modelBuilder.Entity("GenesisVision.DataModel.Models.WalletTransactions", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<decimal>("Amount");

                    b.Property<DateTime>("Date");

                    b.Property<int>("Type");

                    b.Property<Guid>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("WalletTransactions");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<System.Guid>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<Guid>("RoleId");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<System.Guid>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<Guid>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<System.Guid>", b =>
                {
                    b.Property<string>("LoginProvider");

                    b.Property<string>("ProviderKey");

                    b.Property<string>("ProviderDisplayName");

                    b.Property<Guid>("UserId");

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserLogins");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<System.Guid>", b =>
                {
                    b.Property<Guid>("UserId");

                    b.Property<Guid>("RoleId");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetUserRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<System.Guid>", b =>
                {
                    b.Property<Guid>("UserId");

                    b.Property<string>("LoginProvider");

                    b.Property<string>("Name");

                    b.Property<string>("Value");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens");
                });

            modelBuilder.Entity("GenesisVision.DataModel.Models.BrokersAccounts", b =>
                {
                    b.HasOne("GenesisVision.DataModel.Models.ApplicationUser", "User")
                        .WithOne("BrokersAccount")
                        .HasForeignKey("GenesisVision.DataModel.Models.BrokersAccounts", "UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("GenesisVision.DataModel.Models.BrokerTradeServers", b =>
                {
                    b.HasOne("GenesisVision.DataModel.Models.BrokersAccounts", "Broker")
                        .WithMany("BrokerTradeServers")
                        .HasForeignKey("BrokerId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("GenesisVision.DataModel.Models.InvestmentPrograms", b =>
                {
                    b.HasOne("GenesisVision.DataModel.Models.ManagerAccounts", "ManagerAccount")
                        .WithOne("InvestmentProgram")
                        .HasForeignKey("GenesisVision.DataModel.Models.InvestmentPrograms", "ManagerAccountId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("GenesisVision.DataModel.Models.ManagerTokens", "Token")
                        .WithOne("InvestmentProgram")
                        .HasForeignKey("GenesisVision.DataModel.Models.InvestmentPrograms", "ManagerTokensId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("GenesisVision.DataModel.Models.InvestmentRequests", b =>
                {
                    b.HasOne("GenesisVision.DataModel.Models.InvestmentPrograms", "InvestmentProgram")
                        .WithMany("InvestmentRequests")
                        .HasForeignKey("InvestmentProgramtId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("GenesisVision.DataModel.Models.Periods", "Period")
                        .WithMany("InvestmentRequests")
                        .HasForeignKey("PeriodId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("GenesisVision.DataModel.Models.ApplicationUser", "User")
                        .WithMany("InvestmentRequests")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("GenesisVision.DataModel.Models.InvestorAccounts", "InvestorAccount")
                        .WithMany("InvestmentRequests")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("GenesisVision.DataModel.Models.WalletTransactions", "WalletTransaction")
                        .WithOne("InvestmentRequest")
                        .HasForeignKey("GenesisVision.DataModel.Models.InvestmentRequests", "WalletTransactionId");
                });

            modelBuilder.Entity("GenesisVision.DataModel.Models.InvestorAccounts", b =>
                {
                    b.HasOne("GenesisVision.DataModel.Models.ApplicationUser", "User")
                        .WithOne("InvestorAccount")
                        .HasForeignKey("GenesisVision.DataModel.Models.InvestorAccounts", "UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("GenesisVision.DataModel.Models.IOTransactions", b =>
                {
                    b.HasOne("GenesisVision.DataModel.Models.ApplicationUser", "User")
                        .WithMany("IOTransactions")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("GenesisVision.DataModel.Models.Wallets", "Wallet")
                        .WithMany("IOTransactions")
                        .HasForeignKey("WalletId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("GenesisVision.DataModel.Models.WalletTransactions", "WalletTransaction")
                        .WithOne("IOTransaction")
                        .HasForeignKey("GenesisVision.DataModel.Models.IOTransactions", "WalletTransactionId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("GenesisVision.DataModel.Models.ManagerAccounts", b =>
                {
                    b.HasOne("GenesisVision.DataModel.Models.BrokerTradeServers", "BrokerTradeServer")
                        .WithMany("ManagerAccounts")
                        .HasForeignKey("BrokerTradeServerId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("GenesisVision.DataModel.Models.ApplicationUser", "User")
                        .WithMany("ManagerAccounts")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("GenesisVision.DataModel.Models.ManagerRequests", b =>
                {
                    b.HasOne("GenesisVision.DataModel.Models.BrokerTradeServers", "BrokerTradeServers")
                        .WithMany("ManagerAccountRequests")
                        .HasForeignKey("BrokerTradeServerId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("GenesisVision.DataModel.Models.ApplicationUser", "User")
                        .WithMany("ManagerAccountRequests")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("GenesisVision.DataModel.Models.ManagersAccountsStatistics", b =>
                {
                    b.HasOne("GenesisVision.DataModel.Models.InvestmentPrograms", "InvestmentProgram")
                        .WithMany("ManagersAccountsStatistics")
                        .HasForeignKey("InvestmentProgramId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("GenesisVision.DataModel.Models.ManagerAccounts", "ManagerAccount")
                        .WithMany("ManagersAccountsStatistics")
                        .HasForeignKey("ManagerAccountId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("GenesisVision.DataModel.Models.Periods", "Period")
                        .WithMany("ManagersAccountsStatistics")
                        .HasForeignKey("PeriodId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("GenesisVision.DataModel.Models.ApplicationUser", "User")
                        .WithMany("ManagersAccountsStatistics")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("GenesisVision.DataModel.Models.ManagerTokens", b =>
                {
                    b.HasOne("GenesisVision.DataModel.Models.Portfolios")
                        .WithMany("ManagerTokens")
                        .HasForeignKey("PortfoliosInvestorAccountId");
                });

            modelBuilder.Entity("GenesisVision.DataModel.Models.Periods", b =>
                {
                    b.HasOne("GenesisVision.DataModel.Models.InvestmentPrograms", "InvestmentProgram")
                        .WithMany("Periods")
                        .HasForeignKey("InvestmentProgramId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("GenesisVision.DataModel.Models.Portfolios", b =>
                {
                    b.HasOne("GenesisVision.DataModel.Models.InvestorAccounts", "InvestorAccount")
                        .WithOne("Portfolio")
                        .HasForeignKey("GenesisVision.DataModel.Models.Portfolios", "InvestorAccountId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("GenesisVision.DataModel.Models.Profiles", b =>
                {
                    b.HasOne("GenesisVision.DataModel.Models.ApplicationUser", "User")
                        .WithOne("Profile")
                        .HasForeignKey("GenesisVision.DataModel.Models.Profiles", "UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("GenesisVision.DataModel.Models.Wallets", b =>
                {
                    b.HasOne("GenesisVision.DataModel.Models.ApplicationUser", "User")
                        .WithOne("Wallet")
                        .HasForeignKey("GenesisVision.DataModel.Models.Wallets", "UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("GenesisVision.DataModel.Models.WalletTransactions", b =>
                {
                    b.HasOne("GenesisVision.DataModel.Models.ApplicationUser", "User")
                        .WithMany("WalletTransactions")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("GenesisVision.DataModel.Models.Wallets", "Wallet")
                        .WithMany("WalletTransactions")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<System.Guid>", b =>
                {
                    b.HasOne("GenesisVision.DataModel.Models.ApplicationRole")
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<System.Guid>", b =>
                {
                    b.HasOne("GenesisVision.DataModel.Models.ApplicationUser")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<System.Guid>", b =>
                {
                    b.HasOne("GenesisVision.DataModel.Models.ApplicationUser")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<System.Guid>", b =>
                {
                    b.HasOne("GenesisVision.DataModel.Models.ApplicationRole")
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("GenesisVision.DataModel.Models.ApplicationUser")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<System.Guid>", b =>
                {
                    b.HasOne("GenesisVision.DataModel.Models.ApplicationUser")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
