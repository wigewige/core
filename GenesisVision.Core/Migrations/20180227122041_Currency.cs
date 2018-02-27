using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace GenesisVision.Core.Migrations
{
    public partial class Currency : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Currency2",
                table: "ManagersAccounts",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TradePlatformCurrency2",
                table: "ManagerRequests",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Currency2",
                table: "BlockchainAddresses",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Currency2",
                table: "ManagersAccounts");

            migrationBuilder.DropColumn(
                name: "TradePlatformCurrency2",
                table: "ManagerRequests");

            migrationBuilder.DropColumn(
                name: "Currency2",
                table: "BlockchainAddresses");
        }
    }
}
