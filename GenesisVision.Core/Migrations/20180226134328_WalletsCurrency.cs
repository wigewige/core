using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace GenesisVision.Core.Migrations
{
    public partial class WalletsCurrency : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BlockchainAddresses_Wallets_UserId",
                table: "BlockchainAddresses");

            migrationBuilder.DropForeignKey(
                name: "FK_WalletTransactions_AspNetUsers_UserId",
                table: "WalletTransactions");

            migrationBuilder.DropForeignKey(
                name: "FK_WalletTransactions_Wallets_UserId",
                table: "WalletTransactions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Wallets",
                table: "Wallets");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "WalletTransactions",
                newName: "WalletId");

            migrationBuilder.RenameIndex(
                name: "IX_WalletTransactions_UserId",
                table: "WalletTransactions",
                newName: "IX_WalletTransactions_WalletId");

            migrationBuilder.AddColumn<Guid>(
                name: "ApplicationUserId",
                table: "WalletTransactions",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Currency2",
                table: "Wallets",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "DestAddress",
                table: "PaymentTransactions",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "PaymentTransactions",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.Sql("UPDATE \"Wallets\" SET \"Id\" = \"UserId\"");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Wallets",
                table: "Wallets",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_WalletTransactions_ApplicationUserId",
                table: "WalletTransactions",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Wallets_UserId",
                table: "Wallets",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_WalletTransactions_AspNetUsers_ApplicationUserId",
                table: "WalletTransactions",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_WalletTransactions_Wallets_WalletId",
                table: "WalletTransactions",
                column: "WalletId",
                principalTable: "Wallets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WalletTransactions_AspNetUsers_ApplicationUserId",
                table: "WalletTransactions");

            migrationBuilder.DropForeignKey(
                name: "FK_WalletTransactions_Wallets_WalletId",
                table: "WalletTransactions");

            migrationBuilder.DropIndex(
                name: "IX_WalletTransactions_ApplicationUserId",
                table: "WalletTransactions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Wallets",
                table: "Wallets");

            migrationBuilder.DropIndex(
                name: "IX_Wallets_UserId",
                table: "Wallets");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                table: "WalletTransactions");

            migrationBuilder.DropColumn(
                name: "Currency2",
                table: "Wallets");

            migrationBuilder.DropColumn(
                name: "DestAddress",
                table: "PaymentTransactions");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "PaymentTransactions");

            migrationBuilder.RenameColumn(
                name: "WalletId",
                table: "WalletTransactions",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_WalletTransactions_WalletId",
                table: "WalletTransactions",
                newName: "IX_WalletTransactions_UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Wallets",
                table: "Wallets",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_BlockchainAddresses_Wallets_UserId",
                table: "BlockchainAddresses",
                column: "UserId",
                principalTable: "Wallets",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_WalletTransactions_AspNetUsers_UserId",
                table: "WalletTransactions",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_WalletTransactions_Wallets_UserId",
                table: "WalletTransactions",
                column: "UserId",
                principalTable: "Wallets",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
