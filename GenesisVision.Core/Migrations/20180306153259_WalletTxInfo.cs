using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace GenesisVision.Core.Migrations
{
    public partial class WalletTxInfo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "InvestmentProgramtId",
                table: "WalletTransactions",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_WalletTransactions_InvestmentProgramtId",
                table: "WalletTransactions",
                column: "InvestmentProgramtId");

            migrationBuilder.AddForeignKey(
                name: "FK_WalletTransactions_InvestmentPrograms_InvestmentProgramtId",
                table: "WalletTransactions",
                column: "InvestmentProgramtId",
                principalTable: "InvestmentPrograms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WalletTransactions_InvestmentPrograms_InvestmentProgramtId",
                table: "WalletTransactions");

            migrationBuilder.DropIndex(
                name: "IX_WalletTransactions_InvestmentProgramtId",
                table: "WalletTransactions");

            migrationBuilder.DropColumn(
                name: "InvestmentProgramtId",
                table: "WalletTransactions");
        }
    }
}
