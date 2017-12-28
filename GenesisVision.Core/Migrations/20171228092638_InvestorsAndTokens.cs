using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace GenesisVision.Core.Migrations
{
    public partial class InvestorsAndTokens : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ManagerTokensId",
                table: "InvestmentPrograms",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "InvestorAccounts",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    GvtBalance = table.Column<decimal>(nullable: false),
                    UserId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvestorAccounts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InvestorAccounts_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Portfolios",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    InvestorAccountId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Portfolios", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Portfolios_InvestorAccounts_InvestorAccountId",
                        column: x => x.InvestorAccountId,
                        principalTable: "InvestorAccounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ManagerTokens",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    PortfoliosId = table.Column<Guid>(nullable: true),
                    TokenAddress = table.Column<string>(nullable: true),
                    TokenSymbol = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ManagerTokens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ManagerTokens_Portfolios_PortfoliosId",
                        column: x => x.PortfoliosId,
                        principalTable: "Portfolios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_InvestmentPrograms_ManagerTokensId",
                table: "InvestmentPrograms",
                column: "ManagerTokensId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BrokerTradeServers_Host",
                table: "BrokerTradeServers",
                column: "Host",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Brokers_Name",
                table: "Brokers",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_InvestorAccounts_UserId",
                table: "InvestorAccounts",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ManagerTokens_PortfoliosId",
                table: "ManagerTokens",
                column: "PortfoliosId");

            migrationBuilder.CreateIndex(
                name: "IX_Portfolios_InvestorAccountId",
                table: "Portfolios",
                column: "InvestorAccountId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_InvestmentPrograms_ManagerTokens_ManagerTokensId",
                table: "InvestmentPrograms",
                column: "ManagerTokensId",
                principalTable: "ManagerTokens",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InvestmentPrograms_ManagerTokens_ManagerTokensId",
                table: "InvestmentPrograms");

            migrationBuilder.DropTable(
                name: "ManagerTokens");

            migrationBuilder.DropTable(
                name: "Portfolios");

            migrationBuilder.DropTable(
                name: "InvestorAccounts");

            migrationBuilder.DropIndex(
                name: "IX_InvestmentPrograms_ManagerTokensId",
                table: "InvestmentPrograms");

            migrationBuilder.DropIndex(
                name: "IX_BrokerTradeServers_Host",
                table: "BrokerTradeServers");

            migrationBuilder.DropIndex(
                name: "IX_Brokers_Name",
                table: "Brokers");

            migrationBuilder.DropColumn(
                name: "ManagerTokensId",
                table: "InvestmentPrograms");
        }
    }
}
