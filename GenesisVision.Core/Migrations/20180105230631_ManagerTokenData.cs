using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace GenesisVision.Core.Migrations
{
    public partial class ManagerTokenData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Confirmed",
                table: "ManagersAccounts",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "TokenName",
                table: "ManagersAccounts",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TokenSymbol",
                table: "ManagersAccounts",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TokenName",
                table: "ManagerRequests",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TokenSymbol",
                table: "ManagerRequests",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Confirmed",
                table: "ManagersAccounts");

            migrationBuilder.DropColumn(
                name: "TokenName",
                table: "ManagersAccounts");

            migrationBuilder.DropColumn(
                name: "TokenSymbol",
                table: "ManagersAccounts");

            migrationBuilder.DropColumn(
                name: "TokenName",
                table: "ManagerRequests");

            migrationBuilder.DropColumn(
                name: "TokenSymbol",
                table: "ManagerRequests");
        }
    }
}
