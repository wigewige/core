using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace GenesisVision.Core.Migrations
{
    public partial class PeriodsProperty : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "PeriodId",
                table: "InvestmentRequests",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_InvestmentRequests_PeriodId",
                table: "InvestmentRequests",
                column: "PeriodId");

            migrationBuilder.AddForeignKey(
                name: "FK_InvestmentRequests_Periods_PeriodId",
                table: "InvestmentRequests",
                column: "PeriodId",
                principalTable: "Periods",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InvestmentRequests_Periods_PeriodId",
                table: "InvestmentRequests");

            migrationBuilder.DropIndex(
                name: "IX_InvestmentRequests_PeriodId",
                table: "InvestmentRequests");

            migrationBuilder.DropColumn(
                name: "PeriodId",
                table: "InvestmentRequests");
        }
    }
}
