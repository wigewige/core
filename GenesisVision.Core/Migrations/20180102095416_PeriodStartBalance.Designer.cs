﻿// <auto-generated />
using GenesisVision.DataModel;
using GenesisVision.DataModel.Models;
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
    [Migration("20180102095416_PeriodStartBalance")]
    partial class PeriodStartBalance
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn)
                .HasAnnotation("ProductVersion", "2.0.1-rtm-125");

            modelBuilder.Entity("GenesisVision.DataModel.Models.AspNetUsers", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("AccessFailedCount");

                    b.Property<string>("ConcurrencyStamp");

                    b.Property<string>("Email");

                    b.Property<bool>("EmailConfirmed");

                    b.Property<bool>("LockoutEnabled");

                    b.Property<DateTimeOffset?>("LockoutEnd");

                    b.Property<string>("NormalizedEmail");

                    b.Property<string>("NormalizedUserName");

                    b.Property<string>("PasswordHash");

                    b.Property<string>("PhoneNumber");

                    b.Property<bool>("PhoneNumberConfirmed");

                    b.Property<string>("SecurityStamp");

                    b.Property<bool>("TwoFactorEnabled");

                    b.Property<string>("UserName");

                    b.HasKey("Id");

                    b.ToTable("AspNetUsers");
                });

            modelBuilder.Entity("GenesisVision.DataModel.Models.Brokers", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Description");

                    b.Property<bool>("IsEnabled");

                    b.Property<string>("Name");

                    b.Property<DateTime>("RegistrationDate");

                    b.HasKey("Id");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.ToTable("Brokers");
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

                    b.Property<Guid>("ManagerTokensId");

                    b.Property<Guid>("ManagersAccountId");

                    b.Property<int>("Period");

                    b.HasKey("Id");

                    b.HasIndex("ManagerTokensId")
                        .IsUnique();

                    b.HasIndex("ManagersAccountId");

                    b.ToTable("InvestmentPrograms");
                });

            modelBuilder.Entity("GenesisVision.DataModel.Models.InvestmentRequests", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<decimal>("Amount");

                    b.Property<DateTime>("Date");

                    b.Property<Guid>("InvestmentProgramtId");

                    b.Property<Guid>("InvestorAccountId");

                    b.Property<Guid>("PeriodId");

                    b.Property<int>("Status");

                    b.Property<int>("Type");

                    b.Property<Guid>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("InvestmentProgramtId");

                    b.HasIndex("InvestorAccountId");

                    b.HasIndex("PeriodId");

                    b.HasIndex("UserId");

                    b.ToTable("InvestmentRequests");
                });

            modelBuilder.Entity("GenesisVision.DataModel.Models.InvestorAccounts", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<decimal>("GvtBalance");

                    b.Property<Guid>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("UserId")
                        .IsUnique();

                    b.ToTable("InvestorAccounts");
                });

            modelBuilder.Entity("GenesisVision.DataModel.Models.ManagerAccountRequests", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Avatar");

                    b.Property<Guid>("BrokerTradeServerId");

                    b.Property<string>("Currency");

                    b.Property<DateTime>("Date");

                    b.Property<string>("Description");

                    b.Property<string>("Name");

                    b.Property<int>("Status");

                    b.Property<int>("Type");

                    b.Property<Guid>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("BrokerTradeServerId");

                    b.HasIndex("UserId");

                    b.ToTable("ManagerRequests");
                });

            modelBuilder.Entity("GenesisVision.DataModel.Models.ManagerAccounts", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Avatar");

                    b.Property<Guid>("BrokerTradeServerId");

                    b.Property<string>("Currency");

                    b.Property<string>("Description");

                    b.Property<string>("IpfsHash");

                    b.Property<bool>("IsEnabled");

                    b.Property<string>("Login");

                    b.Property<string>("Name");

                    b.Property<decimal>("Rating");

                    b.Property<DateTime>("RegistrationDate");

                    b.Property<Guid>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("BrokerTradeServerId");

                    b.HasIndex("UserId");

                    b.ToTable("ManagersAccounts");
                });

            modelBuilder.Entity("GenesisVision.DataModel.Models.ManagerTokens", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid?>("PortfoliosId");

                    b.Property<string>("TokenAddress");

                    b.Property<string>("TokenSymbol");

                    b.HasKey("Id");

                    b.HasIndex("PortfoliosId");

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
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid>("InvestorAccountId");

                    b.HasKey("Id");

                    b.HasIndex("InvestorAccountId")
                        .IsUnique();

                    b.ToTable("Portfolios");
                });

            modelBuilder.Entity("GenesisVision.DataModel.Models.BrokerTradeServers", b =>
                {
                    b.HasOne("GenesisVision.DataModel.Models.Brokers", "Broker")
                        .WithMany("BrokerTradeServers")
                        .HasForeignKey("BrokerId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("GenesisVision.DataModel.Models.InvestmentPrograms", b =>
                {
                    b.HasOne("GenesisVision.DataModel.Models.ManagerTokens", "Tokens")
                        .WithOne("InvestmentProgram")
                        .HasForeignKey("GenesisVision.DataModel.Models.InvestmentPrograms", "ManagerTokensId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("GenesisVision.DataModel.Models.ManagerAccounts", "ManagersAccount")
                        .WithMany("InvestmentPrograms")
                        .HasForeignKey("ManagersAccountId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("GenesisVision.DataModel.Models.InvestmentRequests", b =>
                {
                    b.HasOne("GenesisVision.DataModel.Models.InvestmentPrograms", "InvestmentProgram")
                        .WithMany("InvestmentRequests")
                        .HasForeignKey("InvestmentProgramtId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("GenesisVision.DataModel.Models.InvestorAccounts", "InvestorAccount")
                        .WithMany("InvestmentRequestses")
                        .HasForeignKey("InvestorAccountId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("GenesisVision.DataModel.Models.Periods", "Period")
                        .WithMany("InvestmentRequests")
                        .HasForeignKey("PeriodId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("GenesisVision.DataModel.Models.AspNetUsers", "User")
                        .WithMany("InvestmentRequests")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("GenesisVision.DataModel.Models.InvestorAccounts", b =>
                {
                    b.HasOne("GenesisVision.DataModel.Models.AspNetUsers", "User")
                        .WithOne("InvestorAccount")
                        .HasForeignKey("GenesisVision.DataModel.Models.InvestorAccounts", "UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("GenesisVision.DataModel.Models.ManagerAccountRequests", b =>
                {
                    b.HasOne("GenesisVision.DataModel.Models.BrokerTradeServers", "BrokerTradeServers")
                        .WithMany("ManagerAccountRequests")
                        .HasForeignKey("BrokerTradeServerId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("GenesisVision.DataModel.Models.AspNetUsers", "User")
                        .WithMany("ManagerAccountRequests")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("GenesisVision.DataModel.Models.ManagerAccounts", b =>
                {
                    b.HasOne("GenesisVision.DataModel.Models.BrokerTradeServers", "BrokerTradeServer")
                        .WithMany("ManagerAccounts")
                        .HasForeignKey("BrokerTradeServerId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("GenesisVision.DataModel.Models.AspNetUsers", "User")
                        .WithMany("ManagerAccounts")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("GenesisVision.DataModel.Models.ManagerTokens", b =>
                {
                    b.HasOne("GenesisVision.DataModel.Models.Portfolios")
                        .WithMany("ManagerTokens")
                        .HasForeignKey("PortfoliosId");
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
#pragma warning restore 612, 618
        }
    }
}
