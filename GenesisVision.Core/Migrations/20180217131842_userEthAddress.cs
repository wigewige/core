using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace GenesisVision.Core.Migrations
{
    public partial class userEthAddress : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Address",
                table: "Wallets");

            migrationBuilder.AddColumn<Guid>(
                name: "CurrentAddressId",
                table: "Wallets",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CurrentAddressId",
                table: "Wallets");

            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "Wallets",
                nullable: true);
        }
    }
}
