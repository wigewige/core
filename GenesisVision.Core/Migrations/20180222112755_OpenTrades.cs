using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace GenesisVision.Core.Migrations
{
    public partial class OpenTrades : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ManagersAccountsOpenTrades",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    DateOpenOrder = table.Column<DateTime>(nullable: false),
                    DateUpdateFromTradePlatform = table.Column<DateTime>(nullable: false),
                    Direction = table.Column<int>(nullable: false),
                    ManagerAccountId = table.Column<Guid>(nullable: false),
                    Price = table.Column<decimal>(nullable: false),
                    Profit = table.Column<decimal>(nullable: false),
                    Symbol = table.Column<string>(nullable: true),
                    Ticket = table.Column<long>(nullable: false),
                    Volume = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ManagersAccountsOpenTrades", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ManagersAccountsOpenTrades_ManagersAccounts_ManagerAccountId",
                        column: x => x.ManagerAccountId,
                        principalTable: "ManagersAccounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ManagersAccountsOpenTrades_ManagerAccountId",
                table: "ManagersAccountsOpenTrades",
                column: "ManagerAccountId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ManagersAccountsOpenTrades");
        }
    }
}
