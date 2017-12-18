using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace GenesisVision.Core.Migrations
{
    public partial class ManagerRequests : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ManagerRequests_BrokerTradeServers_BrokerTradeServersId",
                table: "ManagerRequests");

            migrationBuilder.RenameColumn(
                name: "Password",
                table: "ManagerRequests",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "Login",
                table: "ManagerRequests",
                newName: "Description");

            migrationBuilder.RenameColumn(
                name: "BrokerTradeServersId",
                table: "ManagerRequests",
                newName: "BrokerTradeServerId");

            migrationBuilder.RenameIndex(
                name: "IX_ManagerRequests_BrokerTradeServersId",
                table: "ManagerRequests",
                newName: "IX_ManagerRequests_BrokerTradeServerId");

            migrationBuilder.AddColumn<string>(
                name: "Avatar",
                table: "ManagerRequests",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Currency",
                table: "ManagerRequests",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ManagerRequests_BrokerTradeServers_BrokerTradeServerId",
                table: "ManagerRequests",
                column: "BrokerTradeServerId",
                principalTable: "BrokerTradeServers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ManagerRequests_BrokerTradeServers_BrokerTradeServerId",
                table: "ManagerRequests");

            migrationBuilder.DropColumn(
                name: "Avatar",
                table: "ManagerRequests");

            migrationBuilder.DropColumn(
                name: "Currency",
                table: "ManagerRequests");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "ManagerRequests",
                newName: "Password");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "ManagerRequests",
                newName: "Login");

            migrationBuilder.RenameColumn(
                name: "BrokerTradeServerId",
                table: "ManagerRequests",
                newName: "BrokerTradeServersId");

            migrationBuilder.RenameIndex(
                name: "IX_ManagerRequests_BrokerTradeServerId",
                table: "ManagerRequests",
                newName: "IX_ManagerRequests_BrokerTradeServersId");

            migrationBuilder.AddForeignKey(
                name: "FK_ManagerRequests_BrokerTradeServers_BrokerTradeServersId",
                table: "ManagerRequests",
                column: "BrokerTradeServersId",
                principalTable: "BrokerTradeServers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
