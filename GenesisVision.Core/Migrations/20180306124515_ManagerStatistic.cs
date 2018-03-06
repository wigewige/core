using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace GenesisVision.Core.Migrations
{
    public partial class ManagerStatistic : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ManagersAccountsStatistics_InvestmentPrograms_InvestmentProgramId",
                table: "ManagersAccountsStatistics");

            migrationBuilder.DropForeignKey(
                name: "FK_ManagersAccountsStatistics_AspNetUsers_UserId",
                table: "ManagersAccountsStatistics");

            migrationBuilder.DropIndex(
                name: "IX_ManagersAccountsStatistics_InvestmentProgramId",
                table: "ManagersAccountsStatistics");

            migrationBuilder.DropIndex(
                name: "IX_ManagersAccountsStatistics_UserId",
                table: "ManagersAccountsStatistics");

            migrationBuilder.DropColumn(
                name: "InvestmentProgramId",
                table: "ManagersAccountsStatistics");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "ManagersAccountsStatistics");

            migrationBuilder.DropColumn(
                name: "OrdersCount",
                table: "InvestmentPrograms");

            migrationBuilder.DropColumn(
                name: "TotalProfit",
                table: "InvestmentPrograms");

            migrationBuilder.RenameColumn(
                name: "CurrentBalance",
                table: "ManagersAccountsStatistics",
                newName: "Volume");

            migrationBuilder.AddColumn<decimal>(
                name: "Fund",
                table: "ManagersAccountsStatistics",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Loss",
                table: "ManagersAccountsStatistics",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalProfit",
                table: "ManagersAccountsStatistics",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Balance",
                table: "ManagersAccounts",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "OrdersCount",
                table: "ManagersAccounts",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "ProfitAvg",
                table: "ManagersAccounts",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "ProfitTotal",
                table: "ManagersAccounts",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "VolumeAvg",
                table: "ManagersAccounts",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "VolumeTotal",
                table: "ManagersAccounts",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Fund",
                table: "ManagersAccountsStatistics");

            migrationBuilder.DropColumn(
                name: "Loss",
                table: "ManagersAccountsStatistics");

            migrationBuilder.DropColumn(
                name: "TotalProfit",
                table: "ManagersAccountsStatistics");

            migrationBuilder.DropColumn(
                name: "Balance",
                table: "ManagersAccounts");

            migrationBuilder.DropColumn(
                name: "OrdersCount",
                table: "ManagersAccounts");

            migrationBuilder.DropColumn(
                name: "ProfitAvg",
                table: "ManagersAccounts");

            migrationBuilder.DropColumn(
                name: "ProfitTotal",
                table: "ManagersAccounts");

            migrationBuilder.DropColumn(
                name: "VolumeAvg",
                table: "ManagersAccounts");

            migrationBuilder.DropColumn(
                name: "VolumeTotal",
                table: "ManagersAccounts");

            migrationBuilder.RenameColumn(
                name: "Volume",
                table: "ManagersAccountsStatistics",
                newName: "CurrentBalance");

            migrationBuilder.AddColumn<Guid>(
                name: "InvestmentProgramId",
                table: "ManagersAccountsStatistics",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "ManagersAccountsStatistics",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

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

            migrationBuilder.CreateIndex(
                name: "IX_ManagersAccountsStatistics_InvestmentProgramId",
                table: "ManagersAccountsStatistics",
                column: "InvestmentProgramId");

            migrationBuilder.CreateIndex(
                name: "IX_ManagersAccountsStatistics_UserId",
                table: "ManagersAccountsStatistics",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_ManagersAccountsStatistics_InvestmentPrograms_InvestmentProgramId",
                table: "ManagersAccountsStatistics",
                column: "InvestmentProgramId",
                principalTable: "InvestmentPrograms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ManagersAccountsStatistics_AspNetUsers_UserId",
                table: "ManagersAccountsStatistics",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
