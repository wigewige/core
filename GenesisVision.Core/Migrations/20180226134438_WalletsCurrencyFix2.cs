using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace GenesisVision.Core.Migrations
{
    public partial class WalletsCurrencyFix2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Currency2",
                table: "Wallets");

            migrationBuilder.AddColumn<int>(
                name: "Currency",
                table: "Wallets",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Currency",
                table: "Wallets");

            migrationBuilder.AddColumn<int>(
                name: "Currency2",
                table: "Wallets",
                nullable: false,
                defaultValue: 0);
        }
    }
}
