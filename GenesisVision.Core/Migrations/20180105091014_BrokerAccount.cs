using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace GenesisVision.Core.Migrations
{
    public partial class BrokerAccount : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BrokerTradeServers_Brokers_BrokerId",
                table: "BrokerTradeServers");

            migrationBuilder.DropTable(
                name: "Brokers");

            migrationBuilder.AddColumn<bool>(
                name: "IsEnabled",
                table: "AspNetUsers",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "BrokersAccounts",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    IsEnabled = table.Column<bool>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    RegistrationDate = table.Column<DateTime>(nullable: false),
                    UserId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BrokersAccounts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BrokersAccounts_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BrokersAccounts_Name",
                table: "BrokersAccounts",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BrokersAccounts_UserId",
                table: "BrokersAccounts",
                column: "UserId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_BrokerTradeServers_BrokersAccounts_BrokerId",
                table: "BrokerTradeServers",
                column: "BrokerId",
                principalTable: "BrokersAccounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BrokerTradeServers_BrokersAccounts_BrokerId",
                table: "BrokerTradeServers");

            migrationBuilder.DropTable(
                name: "BrokersAccounts");

            migrationBuilder.DropColumn(
                name: "IsEnabled",
                table: "AspNetUsers");

            migrationBuilder.CreateTable(
                name: "Brokers",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    IsEnabled = table.Column<bool>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    RegistrationDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Brokers", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Brokers_Name",
                table: "Brokers",
                column: "Name",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_BrokerTradeServers_Brokers_BrokerId",
                table: "BrokerTradeServers",
                column: "BrokerId",
                principalTable: "Brokers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
