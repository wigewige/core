using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace GenesisVision.Core.Migrations
{
    public partial class Investments : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InvestmentPrograms_ManagersAccounts_ManagersAccountId",
                table: "InvestmentPrograms");

            migrationBuilder.DropForeignKey(
                name: "FK_IOTransactions_UserWallets_UserWalletId",
                table: "IOTransactions");

            migrationBuilder.DropForeignKey(
                name: "FK_ManagerTokens_Portfolios_PortfoliosId",
                table: "ManagerTokens");

            migrationBuilder.DropTable(
                name: "UserWallets");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Portfolios",
                table: "Portfolios");

            migrationBuilder.DropIndex(
                name: "IX_Portfolios_InvestorAccountId",
                table: "Portfolios");

            migrationBuilder.DropIndex(
                name: "IX_InvestmentPrograms_ManagersAccountId",
                table: "InvestmentPrograms");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "Portfolios");

            migrationBuilder.DropColumn(
                name: "Avatar",
                table: "ManagersAccounts");

            migrationBuilder.DropColumn(
                name: "Confirmed",
                table: "ManagersAccounts");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "ManagersAccounts");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "ManagersAccounts");

            migrationBuilder.DropColumn(
                name: "Rating",
                table: "ManagersAccounts");

            migrationBuilder.DropColumn(
                name: "TokenName",
                table: "ManagersAccounts");

            migrationBuilder.DropColumn(
                name: "TokenSymbol",
                table: "ManagersAccounts");

            migrationBuilder.RenameColumn(
                name: "PortfoliosId",
                table: "ManagerTokens",
                newName: "PortfoliosInvestorAccountId");

            migrationBuilder.RenameIndex(
                name: "IX_ManagerTokens_PortfoliosId",
                table: "ManagerTokens",
                newName: "IX_ManagerTokens_PortfoliosInvestorAccountId");

            migrationBuilder.RenameColumn(
                name: "IsEnabled",
                table: "ManagersAccounts",
                newName: "IsConfirmed");

            migrationBuilder.RenameColumn(
                name: "ManagersAccountId",
                table: "InvestmentPrograms",
                newName: "ManagerAccountId");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "ManagerRequests",
                newName: "TradePlatformPassword");

            migrationBuilder.RenameColumn(
                name: "Currency",
                table: "ManagerRequests",
                newName: "TradePlatformCurrency");

            migrationBuilder.RenameColumn(
                name: "Avatar",
                table: "ManagerRequests",
                newName: "Logo");

            migrationBuilder.AddColumn<string>(
                name: "TokenName",
                table: "ManagerTokens",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Logo",
                table: "InvestmentPrograms",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Rating",
                table: "InvestmentPrograms",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateFrom",
                table: "ManagerRequests",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DateTo",
                table: "ManagerRequests",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "DepositAmount",
                table: "ManagerRequests",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "FeeEntrance",
                table: "ManagerRequests",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "FeeManagement",
                table: "ManagerRequests",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "FeeSuccess",
                table: "ManagerRequests",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "InvestMaxAmount",
                table: "ManagerRequests",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "InvestMinAmount",
                table: "ManagerRequests",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "Period",
                table: "ManagerRequests",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Portfolios",
                table: "Portfolios",
                column: "InvestorAccountId");

            migrationBuilder.CreateTable(
                name: "Wallets",
                columns: table => new
                {
                    UserId = table.Column<Guid>(nullable: false),
                    Address = table.Column<string>(nullable: true),
                    Amount = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Wallets", x => x.UserId);
                    table.ForeignKey(
                        name: "FK_Wallets_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ManagerTokens_TokenName",
                table: "ManagerTokens",
                column: "TokenName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ManagerTokens_TokenSymbol",
                table: "ManagerTokens",
                column: "TokenSymbol",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_InvestmentPrograms_ManagerAccountId",
                table: "InvestmentPrograms",
                column: "ManagerAccountId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ManagerRequests_TokenName",
                table: "ManagerRequests",
                column: "TokenName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ManagerRequests_TokenSymbol",
                table: "ManagerRequests",
                column: "TokenSymbol",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_InvestmentPrograms_ManagersAccounts_ManagerAccountId",
                table: "InvestmentPrograms",
                column: "ManagerAccountId",
                principalTable: "ManagersAccounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_IOTransactions_Wallets_UserWalletId",
                table: "IOTransactions",
                column: "UserWalletId",
                principalTable: "Wallets",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ManagerTokens_Portfolios_PortfoliosInvestorAccountId",
                table: "ManagerTokens",
                column: "PortfoliosInvestorAccountId",
                principalTable: "Portfolios",
                principalColumn: "InvestorAccountId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InvestmentPrograms_ManagersAccounts_ManagerAccountId",
                table: "InvestmentPrograms");

            migrationBuilder.DropForeignKey(
                name: "FK_IOTransactions_Wallets_UserWalletId",
                table: "IOTransactions");

            migrationBuilder.DropForeignKey(
                name: "FK_ManagerTokens_Portfolios_PortfoliosInvestorAccountId",
                table: "ManagerTokens");

            migrationBuilder.DropTable(
                name: "Wallets");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Portfolios",
                table: "Portfolios");

            migrationBuilder.DropIndex(
                name: "IX_ManagerTokens_TokenName",
                table: "ManagerTokens");

            migrationBuilder.DropIndex(
                name: "IX_ManagerTokens_TokenSymbol",
                table: "ManagerTokens");

            migrationBuilder.DropIndex(
                name: "IX_InvestmentPrograms_ManagerAccountId",
                table: "InvestmentPrograms");

            migrationBuilder.DropIndex(
                name: "IX_ManagerRequests_TokenName",
                table: "ManagerRequests");

            migrationBuilder.DropIndex(
                name: "IX_ManagerRequests_TokenSymbol",
                table: "ManagerRequests");

            migrationBuilder.DropColumn(
                name: "TokenName",
                table: "ManagerTokens");

            migrationBuilder.DropColumn(
                name: "Logo",
                table: "InvestmentPrograms");

            migrationBuilder.DropColumn(
                name: "Rating",
                table: "InvestmentPrograms");

            migrationBuilder.DropColumn(
                name: "DateFrom",
                table: "ManagerRequests");

            migrationBuilder.DropColumn(
                name: "DateTo",
                table: "ManagerRequests");

            migrationBuilder.DropColumn(
                name: "DepositAmount",
                table: "ManagerRequests");

            migrationBuilder.DropColumn(
                name: "FeeEntrance",
                table: "ManagerRequests");

            migrationBuilder.DropColumn(
                name: "FeeManagement",
                table: "ManagerRequests");

            migrationBuilder.DropColumn(
                name: "FeeSuccess",
                table: "ManagerRequests");

            migrationBuilder.DropColumn(
                name: "InvestMaxAmount",
                table: "ManagerRequests");

            migrationBuilder.DropColumn(
                name: "InvestMinAmount",
                table: "ManagerRequests");

            migrationBuilder.DropColumn(
                name: "Period",
                table: "ManagerRequests");

            migrationBuilder.RenameColumn(
                name: "PortfoliosInvestorAccountId",
                table: "ManagerTokens",
                newName: "PortfoliosId");

            migrationBuilder.RenameIndex(
                name: "IX_ManagerTokens_PortfoliosInvestorAccountId",
                table: "ManagerTokens",
                newName: "IX_ManagerTokens_PortfoliosId");

            migrationBuilder.RenameColumn(
                name: "IsConfirmed",
                table: "ManagersAccounts",
                newName: "IsEnabled");

            migrationBuilder.RenameColumn(
                name: "ManagerAccountId",
                table: "InvestmentPrograms",
                newName: "ManagersAccountId");

            migrationBuilder.RenameColumn(
                name: "TradePlatformPassword",
                table: "ManagerRequests",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "TradePlatformCurrency",
                table: "ManagerRequests",
                newName: "Currency");

            migrationBuilder.RenameColumn(
                name: "Logo",
                table: "ManagerRequests",
                newName: "Avatar");

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                table: "Portfolios",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "Avatar",
                table: "ManagersAccounts",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Confirmed",
                table: "ManagersAccounts",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "ManagersAccounts",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "ManagersAccounts",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Rating",
                table: "ManagersAccounts",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "TokenName",
                table: "ManagersAccounts",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TokenSymbol",
                table: "ManagersAccounts",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Portfolios",
                table: "Portfolios",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "UserWallets",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Address = table.Column<string>(nullable: true),
                    WalletType = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserWallets", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Portfolios_InvestorAccountId",
                table: "Portfolios",
                column: "InvestorAccountId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_InvestmentPrograms_ManagersAccountId",
                table: "InvestmentPrograms",
                column: "ManagersAccountId");

            migrationBuilder.AddForeignKey(
                name: "FK_InvestmentPrograms_ManagersAccounts_ManagersAccountId",
                table: "InvestmentPrograms",
                column: "ManagersAccountId",
                principalTable: "ManagersAccounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_IOTransactions_UserWallets_UserWalletId",
                table: "IOTransactions",
                column: "UserWalletId",
                principalTable: "UserWallets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ManagerTokens_Portfolios_PortfoliosId",
                table: "ManagerTokens",
                column: "PortfoliosId",
                principalTable: "Portfolios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
