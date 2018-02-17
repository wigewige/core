using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace GenesisVision.Core.Migrations
{
    public partial class ProfitDistributionTransactions : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ProfitDistributionTransactions",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    PeriodId = table.Column<Guid>(nullable: false),
                    WalletTransactionId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProfitDistributionTransactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProfitDistributionTransactions_Periods_PeriodId",
                        column: x => x.PeriodId,
                        principalTable: "Periods",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProfitDistributionTransactions_WalletTransactions_WalletTransactionId",
                        column: x => x.WalletTransactionId,
                        principalTable: "WalletTransactions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProfitDistributionTransactions_PeriodId",
                table: "ProfitDistributionTransactions",
                column: "PeriodId");

            migrationBuilder.CreateIndex(
                name: "IX_ProfitDistributionTransactions_WalletTransactionId",
                table: "ProfitDistributionTransactions",
                column: "WalletTransactionId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProfitDistributionTransactions");
        }
    }
}
