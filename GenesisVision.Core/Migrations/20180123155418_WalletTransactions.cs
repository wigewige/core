using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace GenesisVision.Core.Migrations
{
    public partial class WalletTransactions : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_IOTransactions_Wallets_UserWalletId",
                table: "IOTransactions");

            migrationBuilder.DropIndex(
                name: "IX_IOTransactions_UserWalletId",
                table: "IOTransactions");

            migrationBuilder.DropColumn(
                name: "UserWalletId",
                table: "IOTransactions");

            migrationBuilder.AddColumn<Guid>(
                name: "WalletId",
                table: "IOTransactions",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "WalletTransactionId",
                table: "IOTransactions",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "WalletTransactionId",
                table: "InvestmentRequests",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "WalletTransactions",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Amount = table.Column<decimal>(nullable: false),
                    Date = table.Column<DateTime>(nullable: false),
                    Type = table.Column<int>(nullable: false),
                    UserId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WalletTransactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WalletTransactions_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WalletTransactions_Wallets_UserId",
                        column: x => x.UserId,
                        principalTable: "Wallets",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_IOTransactions_WalletId",
                table: "IOTransactions",
                column: "WalletId");

            migrationBuilder.CreateIndex(
                name: "IX_IOTransactions_WalletTransactionId",
                table: "IOTransactions",
                column: "WalletTransactionId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_InvestmentRequests_WalletTransactionId",
                table: "InvestmentRequests",
                column: "WalletTransactionId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_WalletTransactions_UserId",
                table: "WalletTransactions",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_InvestmentRequests_WalletTransactions_WalletTransactionId",
                table: "InvestmentRequests",
                column: "WalletTransactionId",
                principalTable: "WalletTransactions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_IOTransactions_Wallets_WalletId",
                table: "IOTransactions",
                column: "WalletId",
                principalTable: "Wallets",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_IOTransactions_WalletTransactions_WalletTransactionId",
                table: "IOTransactions",
                column: "WalletTransactionId",
                principalTable: "WalletTransactions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InvestmentRequests_WalletTransactions_WalletTransactionId",
                table: "InvestmentRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_IOTransactions_Wallets_WalletId",
                table: "IOTransactions");

            migrationBuilder.DropForeignKey(
                name: "FK_IOTransactions_WalletTransactions_WalletTransactionId",
                table: "IOTransactions");

            migrationBuilder.DropTable(
                name: "WalletTransactions");

            migrationBuilder.DropIndex(
                name: "IX_IOTransactions_WalletId",
                table: "IOTransactions");

            migrationBuilder.DropIndex(
                name: "IX_IOTransactions_WalletTransactionId",
                table: "IOTransactions");

            migrationBuilder.DropIndex(
                name: "IX_InvestmentRequests_WalletTransactionId",
                table: "InvestmentRequests");

            migrationBuilder.DropColumn(
                name: "WalletId",
                table: "IOTransactions");

            migrationBuilder.DropColumn(
                name: "WalletTransactionId",
                table: "IOTransactions");

            migrationBuilder.DropColumn(
                name: "WalletTransactionId",
                table: "InvestmentRequests");

            migrationBuilder.AddColumn<Guid>(
                name: "UserWalletId",
                table: "IOTransactions",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_IOTransactions_UserWalletId",
                table: "IOTransactions",
                column: "UserWalletId");

            migrationBuilder.AddForeignKey(
                name: "FK_IOTransactions_Wallets_UserWalletId",
                table: "IOTransactions",
                column: "UserWalletId",
                principalTable: "Wallets",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
