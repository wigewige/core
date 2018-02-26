using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace GenesisVision.Core.Migrations
{
    public partial class PeriodTokenfieldsadded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Currency",
                table: "Wallets",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                table: "Wallets",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<decimal>(
                name: "ManagerStartBalance",
                table: "Periods",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "ManagerStartShare",
                table: "Periods",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "InitialPrice",
                table: "ManagerTokens",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Currency",
                table: "Wallets");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "Wallets");

            migrationBuilder.DropColumn(
                name: "ManagerStartBalance",
                table: "Periods");

            migrationBuilder.DropColumn(
                name: "ManagerStartShare",
                table: "Periods");

            migrationBuilder.DropColumn(
                name: "InitialPrice",
                table: "ManagerTokens");
        }
    }
}
