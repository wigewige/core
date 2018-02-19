using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace GenesisVision.Core.Migrations
{
    public partial class Tx : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ManagerTokens_Portfolios_PortfoliosInvestorAccountId",
                table: "ManagerTokens");

            migrationBuilder.DropForeignKey(
                name: "FK_Portfolios_InvestorAccounts_InvestorAccountId",
                table: "Portfolios");

            migrationBuilder.DropTable(
                name: "IOTransactions");

            migrationBuilder.DropIndex(
                name: "IX_ManagerTokens_PortfoliosInvestorAccountId",
                table: "ManagerTokens");

            migrationBuilder.DropColumn(
                name: "Address",
                table: "Wallets");

            migrationBuilder.DropColumn(
                name: "PortfoliosInvestorAccountId",
                table: "ManagerTokens");

            migrationBuilder.AddColumn<decimal>(
                name: "Amount",
                table: "Portfolios",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<Guid>(
                name: "InvestorAccountUserId",
                table: "Portfolios",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ManagerTokenId",
                table: "Portfolios",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "BlockchainAddresses",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Address = table.Column<string>(nullable: true),
                    Currency = table.Column<string>(nullable: true),
                    IsDefault = table.Column<bool>(nullable: false),
                    UserId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BlockchainAddresses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BlockchainAddresses_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BlockchainAddresses_Wallets_UserId",
                        column: x => x.UserId,
                        principalTable: "Wallets",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProfitDistributionTransactions",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    PeriodId = table.Column<Guid>(nullable: false),
                    WalletTransactionId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProfitDistributionTransactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProfitDistributionTransactions_Periods_PeriodId",
                        column: x => x.PeriodId,
                        principalTable: "Periods",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProfitDistributionTransactions_WalletTransactions_WalletTransactionId",
                        column: x => x.WalletTransactionId,
                        principalTable: "WalletTransactions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PaymentTransactions",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Amount = table.Column<decimal>(nullable: false),
                    BlockchainAddressId = table.Column<Guid>(nullable: false),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    ExtraData = table.Column<string>(nullable: true),
                    Fee = table.Column<decimal>(nullable: false),
                    Hash = table.Column<string>(nullable: true),
                    LastUpdated = table.Column<DateTime>(nullable: true),
                    PaymentTxDate = table.Column<DateTime>(nullable: true),
                    PayoutMinerFee = table.Column<decimal>(nullable: true),
                    PayoutServiceFee = table.Column<decimal>(nullable: true),
                    PayoutStatus = table.Column<int>(nullable: false),
                    PayoutTxHash = table.Column<string>(nullable: true),
                    Status = table.Column<int>(nullable: false),
                    WalletTransactionId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentTransactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PaymentTransactions_BlockchainAddresses_BlockchainAddressId",
                        column: x => x.BlockchainAddressId,
                        principalTable: "BlockchainAddresses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PaymentTransactions_WalletTransactions_WalletTransactionId",
                        column: x => x.WalletTransactionId,
                        principalTable: "WalletTransactions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Portfolios_InvestorAccountUserId",
                table: "Portfolios",
                column: "InvestorAccountUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Portfolios_ManagerTokenId",
                table: "Portfolios",
                column: "ManagerTokenId");

            migrationBuilder.CreateIndex(
                name: "IX_BlockchainAddresses_UserId",
                table: "BlockchainAddresses",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentTransactions_BlockchainAddressId",
                table: "PaymentTransactions",
                column: "BlockchainAddressId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentTransactions_WalletTransactionId",
                table: "PaymentTransactions",
                column: "WalletTransactionId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProfitDistributionTransactions_PeriodId",
                table: "ProfitDistributionTransactions",
                column: "PeriodId");

            migrationBuilder.CreateIndex(
                name: "IX_ProfitDistributionTransactions_WalletTransactionId",
                table: "ProfitDistributionTransactions",
                column: "WalletTransactionId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Portfolios_InvestorAccounts_InvestorAccountUserId",
                table: "Portfolios",
                column: "InvestorAccountUserId",
                principalTable: "InvestorAccounts",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Portfolios_ManagerTokens_ManagerTokenId",
                table: "Portfolios",
                column: "ManagerTokenId",
                principalTable: "ManagerTokens",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Portfolios_InvestorAccounts_InvestorAccountUserId",
                table: "Portfolios");

            migrationBuilder.DropForeignKey(
                name: "FK_Portfolios_ManagerTokens_ManagerTokenId",
                table: "Portfolios");

            migrationBuilder.DropTable(
                name: "PaymentTransactions");

            migrationBuilder.DropTable(
                name: "ProfitDistributionTransactions");

            migrationBuilder.DropTable(
                name: "BlockchainAddresses");

            migrationBuilder.DropIndex(
                name: "IX_Portfolios_InvestorAccountUserId",
                table: "Portfolios");

            migrationBuilder.DropIndex(
                name: "IX_Portfolios_ManagerTokenId",
                table: "Portfolios");

            migrationBuilder.DropColumn(
                name: "Amount",
                table: "Portfolios");

            migrationBuilder.DropColumn(
                name: "InvestorAccountUserId",
                table: "Portfolios");

            migrationBuilder.DropColumn(
                name: "ManagerTokenId",
                table: "Portfolios");

            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "Wallets",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "PortfoliosInvestorAccountId",
                table: "ManagerTokens",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "IOTransactions",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Amount = table.Column<decimal>(nullable: false),
                    ConfirmationDate = table.Column<DateTime>(nullable: false),
                    CreationDate = table.Column<DateTime>(nullable: false),
                    Currency = table.Column<string>(nullable: true),
                    Status = table.Column<int>(nullable: false),
                    Type = table.Column<int>(nullable: false),
                    UserId = table.Column<Guid>(nullable: false),
                    WalletId = table.Column<Guid>(nullable: false),
                    WalletTransactionId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IOTransactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IOTransactions_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_IOTransactions_Wallets_WalletId",
                        column: x => x.WalletId,
                        principalTable: "Wallets",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_IOTransactions_WalletTransactions_WalletTransactionId",
                        column: x => x.WalletTransactionId,
                        principalTable: "WalletTransactions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ManagerTokens_PortfoliosInvestorAccountId",
                table: "ManagerTokens",
                column: "PortfoliosInvestorAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_IOTransactions_UserId",
                table: "IOTransactions",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_IOTransactions_WalletId",
                table: "IOTransactions",
                column: "WalletId");

            migrationBuilder.CreateIndex(
                name: "IX_IOTransactions_WalletTransactionId",
                table: "IOTransactions",
                column: "WalletTransactionId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ManagerTokens_Portfolios_PortfoliosInvestorAccountId",
                table: "ManagerTokens",
                column: "PortfoliosInvestorAccountId",
                principalTable: "Portfolios",
                principalColumn: "InvestorAccountId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Portfolios_InvestorAccounts_InvestorAccountId",
                table: "Portfolios",
                column: "InvestorAccountId",
                principalTable: "InvestorAccounts",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
