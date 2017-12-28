using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace GenesisVision.Core.Migrations
{
    public partial class InvestmentsRelations : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "InvestorAccountId",
                table: "InvestmentRequests",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

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
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InvestmentRequests_InvestorAccounts_InvestorAccountId",
                table: "InvestmentRequests");

            migrationBuilder.DropIndex(
                name: "IX_InvestmentRequests_InvestorAccountId",
                table: "InvestmentRequests");

            migrationBuilder.DropColumn(
                name: "InvestorAccountId",
                table: "InvestmentRequests");
        }
    }
}
