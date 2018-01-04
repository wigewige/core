using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace GenesisVision.Core.Migrations
{
    public partial class UserWallets : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ConfirmationDate",
                table: "IOTransactions",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "CreationDate",
                table: "IOTransactions",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<Guid>(
                name: "UserWalletId",
                table: "IOTransactions",
                nullable: true);

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
                name: "IX_IOTransactions_UserWalletId",
                table: "IOTransactions",
                column: "UserWalletId");

            migrationBuilder.AddForeignKey(
                name: "FK_IOTransactions_UserWallets_UserWalletId",
                table: "IOTransactions",
                column: "UserWalletId",
                principalTable: "UserWallets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_IOTransactions_UserWallets_UserWalletId",
                table: "IOTransactions");

            migrationBuilder.DropTable(
                name: "UserWallets");

            migrationBuilder.DropIndex(
                name: "IX_IOTransactions_UserWalletId",
                table: "IOTransactions");

            migrationBuilder.DropColumn(
                name: "ConfirmationDate",
                table: "IOTransactions");

            migrationBuilder.DropColumn(
                name: "CreationDate",
                table: "IOTransactions");

            migrationBuilder.DropColumn(
                name: "UserWalletId",
                table: "IOTransactions");
        }
    }
}
