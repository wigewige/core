using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace GenesisVision.Core.Migrations
{
    public partial class InvProgramTitle : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "ManagerRequests",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "InvestmentPrograms",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ManagerRequests_Title",
                table: "ManagerRequests",
                column: "Title",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_InvestmentPrograms_Title",
                table: "InvestmentPrograms",
                column: "Title",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ManagerRequests_Title",
                table: "ManagerRequests");

            migrationBuilder.DropIndex(
                name: "IX_InvestmentPrograms_Title",
                table: "InvestmentPrograms");

            migrationBuilder.DropColumn(
                name: "Title",
                table: "ManagerRequests");

            migrationBuilder.DropColumn(
                name: "Title",
                table: "InvestmentPrograms");
        }
    }
}
