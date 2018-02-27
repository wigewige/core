using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace GenesisVision.Core.Migrations
{
    public partial class CurrencyFix : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Currency",
                table: "ManagersAccounts");

            migrationBuilder.DropColumn(
                name: "TradePlatformCurrency",
                table: "ManagerRequests");

            migrationBuilder.DropColumn(
                name: "Currency",
                table: "BlockchainAddresses");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Currency",
                table: "ManagersAccounts",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TradePlatformCurrency",
                table: "ManagerRequests",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Currency",
                table: "BlockchainAddresses",
                nullable: true);
        }
    }
}
