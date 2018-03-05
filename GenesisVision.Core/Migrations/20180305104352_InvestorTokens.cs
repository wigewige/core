using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace GenesisVision.Core.Migrations
{
    public partial class InvestorTokens : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InvestmentPrograms_ManagerTokens_ManagerTokensId",
                table: "InvestmentPrograms");

            migrationBuilder.DropTable(
                name: "Portfolios");

            migrationBuilder.RenameColumn(
                name: "ManagerTokensId",
                table: "InvestmentPrograms",
                newName: "ManagerTokenId");

            migrationBuilder.RenameIndex(
                name: "IX_InvestmentPrograms_ManagerTokensId",
                table: "InvestmentPrograms",
                newName: "IX_InvestmentPrograms_ManagerTokenId");

            migrationBuilder.CreateTable(
                name: "InvestorTokens",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Amount = table.Column<decimal>(nullable: false),
                    InvestorAccountId = table.Column<Guid>(nullable: false),
                    ManagerTokenId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvestorTokens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InvestorTokens_InvestorAccounts_InvestorAccountId",
                        column: x => x.InvestorAccountId,
                        principalTable: "InvestorAccounts",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InvestorTokens_ManagerTokens_ManagerTokenId",
                        column: x => x.ManagerTokenId,
                        principalTable: "ManagerTokens",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_InvestorTokens_InvestorAccountId",
                table: "InvestorTokens",
                column: "InvestorAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_InvestorTokens_ManagerTokenId",
                table: "InvestorTokens",
                column: "ManagerTokenId");

            migrationBuilder.AddForeignKey(
                name: "FK_InvestmentPrograms_ManagerTokens_ManagerTokenId",
                table: "InvestmentPrograms",
                column: "ManagerTokenId",
                principalTable: "ManagerTokens",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InvestmentPrograms_ManagerTokens_ManagerTokenId",
                table: "InvestmentPrograms");

            migrationBuilder.DropTable(
                name: "InvestorTokens");

            migrationBuilder.RenameColumn(
                name: "ManagerTokenId",
                table: "InvestmentPrograms",
                newName: "ManagerTokensId");

            migrationBuilder.RenameIndex(
                name: "IX_InvestmentPrograms_ManagerTokenId",
                table: "InvestmentPrograms",
                newName: "IX_InvestmentPrograms_ManagerTokensId");

            migrationBuilder.CreateTable(
                name: "Portfolios",
                columns: table => new
                {
                    InvestorAccountId = table.Column<Guid>(nullable: false),
                    Amount = table.Column<decimal>(nullable: false),
                    InvestorAccountUserId = table.Column<Guid>(nullable: true),
                    ManagerTokenId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Portfolios", x => x.InvestorAccountId);
                    table.ForeignKey(
                        name: "FK_Portfolios_InvestorAccounts_InvestorAccountUserId",
                        column: x => x.InvestorAccountUserId,
                        principalTable: "InvestorAccounts",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Portfolios_ManagerTokens_ManagerTokenId",
                        column: x => x.ManagerTokenId,
                        principalTable: "ManagerTokens",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Portfolios_InvestorAccountUserId",
                table: "Portfolios",
                column: "InvestorAccountUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Portfolios_ManagerTokenId",
                table: "Portfolios",
                column: "ManagerTokenId");

            migrationBuilder.AddForeignKey(
                name: "FK_InvestmentPrograms_ManagerTokens_ManagerTokensId",
                table: "InvestmentPrograms",
                column: "ManagerTokensId",
                principalTable: "ManagerTokens",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
