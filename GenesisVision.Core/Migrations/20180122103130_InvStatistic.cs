using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace GenesisVision.Core.Migrations
{
    public partial class InvStatistic : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ManagersAccountsStatistics",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CurrentBalance = table.Column<decimal>(nullable: false),
                    Date = table.Column<DateTime>(nullable: false),
                    InvestmentProgramId = table.Column<Guid>(nullable: false),
                    ManagerAccountId = table.Column<Guid>(nullable: false),
                    PeriodId = table.Column<Guid>(nullable: false),
                    Profit = table.Column<decimal>(nullable: false),
                    UserId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ManagersAccountsStatistics", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ManagersAccountsStatistics_InvestmentPrograms_InvestmentProgramId",
                        column: x => x.InvestmentProgramId,
                        principalTable: "InvestmentPrograms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ManagersAccountsStatistics_ManagersAccounts_ManagerAccountId",
                        column: x => x.ManagerAccountId,
                        principalTable: "ManagersAccounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ManagersAccountsStatistics_Periods_PeriodId",
                        column: x => x.PeriodId,
                        principalTable: "Periods",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ManagersAccountsStatistics_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ManagersAccountsStatistics_InvestmentProgramId",
                table: "ManagersAccountsStatistics",
                column: "InvestmentProgramId");

            migrationBuilder.CreateIndex(
                name: "IX_ManagersAccountsStatistics_ManagerAccountId",
                table: "ManagersAccountsStatistics",
                column: "ManagerAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_ManagersAccountsStatistics_PeriodId",
                table: "ManagersAccountsStatistics",
                column: "PeriodId");

            migrationBuilder.CreateIndex(
                name: "IX_ManagersAccountsStatistics_UserId",
                table: "ManagersAccountsStatistics",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ManagersAccountsStatistics");
        }
    }
}
