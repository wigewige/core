using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace GenesisVision.Core.Migrations
{
    public partial class InvAccountFix : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InvestmentRequests_InvestorAccounts_InvestorAccountId",
                table: "InvestmentRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_Portfolios_InvestorAccounts_InvestorAccountId",
                table: "Portfolios");

            migrationBuilder.DropPrimaryKey(
                name: "PK_InvestorAccounts",
                table: "InvestorAccounts");

            migrationBuilder.DropIndex(
                name: "IX_InvestorAccounts_UserId",
                table: "InvestorAccounts");

            migrationBuilder.DropIndex(
                name: "IX_InvestmentRequests_InvestorAccountId",
                table: "InvestmentRequests");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "InvestorAccounts");

            migrationBuilder.DropColumn(
                name: "GvtBalance",
                table: "InvestorAccounts");

            migrationBuilder.DropColumn(
                name: "InvestorAccountId",
                table: "InvestmentRequests");

            migrationBuilder.AddPrimaryKey(
                name: "PK_InvestorAccounts",
                table: "InvestorAccounts",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_InvestmentRequests_InvestorAccounts_UserId",
                table: "InvestmentRequests",
                column: "UserId",
                principalTable: "InvestorAccounts",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Portfolios_InvestorAccounts_InvestorAccountId",
                table: "Portfolios",
                column: "InvestorAccountId",
                principalTable: "InvestorAccounts",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InvestmentRequests_InvestorAccounts_UserId",
                table: "InvestmentRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_Portfolios_InvestorAccounts_InvestorAccountId",
                table: "Portfolios");

            migrationBuilder.DropPrimaryKey(
                name: "PK_InvestorAccounts",
                table: "InvestorAccounts");

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                table: "InvestorAccounts",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<decimal>(
                name: "GvtBalance",
                table: "InvestorAccounts",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<Guid>(
                name: "InvestorAccountId",
                table: "InvestmentRequests",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddPrimaryKey(
                name: "PK_InvestorAccounts",
                table: "InvestorAccounts",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_InvestorAccounts_UserId",
                table: "InvestorAccounts",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_InvestmentRequests_InvestorAccountId",
                table: "InvestmentRequests",
                column: "InvestorAccountId");

            migrationBuilder.AddForeignKey(
                name: "FK_InvestmentRequests_InvestorAccounts_InvestorAccountId",
                table: "InvestmentRequests",
                column: "InvestorAccountId",
                principalTable: "InvestorAccounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Portfolios_InvestorAccounts_InvestorAccountId",
                table: "Portfolios",
                column: "InvestorAccountId",
                principalTable: "InvestorAccounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
