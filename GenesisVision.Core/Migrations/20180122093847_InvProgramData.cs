using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace GenesisVision.Core.Migrations
{
    public partial class InvProgramData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "OrdersCount",
                table: "InvestmentPrograms",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalProfit",
                table: "InvestmentPrograms",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OrdersCount",
                table: "InvestmentPrograms");

            migrationBuilder.DropColumn(
                name: "TotalProfit",
                table: "InvestmentPrograms");
        }
    }
}
