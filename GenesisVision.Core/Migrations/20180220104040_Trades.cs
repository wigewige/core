using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace GenesisVision.Core.Migrations
{
    public partial class Trades : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ManagersAccountsTrades",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Date = table.Column<DateTime>(nullable: true),
                    DateClose = table.Column<DateTime>(nullable: true),
                    DateOpen = table.Column<DateTime>(nullable: true),
                    Direction = table.Column<int>(nullable: false),
                    Entry = table.Column<int>(nullable: true),
                    ManagerAccountId = table.Column<Guid>(nullable: false),
                    Price = table.Column<decimal>(nullable: true),
                    PriceClose = table.Column<decimal>(nullable: true),
                    PriceOpen = table.Column<decimal>(nullable: true),
                    Profit = table.Column<decimal>(nullable: false),
                    Symbol = table.Column<string>(nullable: true),
                    Ticket = table.Column<long>(nullable: false),
                    Volume = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ManagersAccountsTrades", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ManagersAccountsTrades_ManagersAccounts_ManagerAccountId",
                        column: x => x.ManagerAccountId,
                        principalTable: "ManagersAccounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ManagersAccountsTrades_ManagerAccountId",
                table: "ManagersAccountsTrades",
                column: "ManagerAccountId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ManagersAccountsTrades");
        }
    }
}
