using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace GenesisVision.Core.Migrations
{
    public partial class AuthFixes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InvestmentRequests_AspNetUsers_UserId",
                table: "InvestmentRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_InvestorAccounts_AspNetUsers_UserId",
                table: "InvestorAccounts");

            migrationBuilder.DropForeignKey(
                name: "FK_IOTransactions_AspNetUsers_UserId",
                table: "IOTransactions");

            migrationBuilder.DropForeignKey(
                name: "FK_ManagerRequests_AspNetUsers_UserId",
                table: "ManagerRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_ManagersAccounts_AspNetUsers_UserId",
                table: "ManagersAccounts");

            migrationBuilder.AddForeignKey(
                name: "FK_InvestmentRequests_AspNetUsers_UserId",
                table: "InvestmentRequests",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_InvestorAccounts_AspNetUsers_UserId",
                table: "InvestorAccounts",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_IOTransactions_AspNetUsers_UserId",
                table: "IOTransactions",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ManagerRequests_AspNetUsers_UserId",
                table: "ManagerRequests",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ManagersAccounts_AspNetUsers_UserId",
                table: "ManagersAccounts",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InvestmentRequests_AspNetUsers_UserId",
                table: "InvestmentRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_InvestorAccounts_AspNetUsers_UserId",
                table: "InvestorAccounts");

            migrationBuilder.DropForeignKey(
                name: "FK_IOTransactions_AspNetUsers_UserId",
                table: "IOTransactions");

            migrationBuilder.DropForeignKey(
                name: "FK_ManagerRequests_AspNetUsers_UserId",
                table: "ManagerRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_ManagersAccounts_AspNetUsers_UserId",
                table: "ManagersAccounts");

            migrationBuilder.AddForeignKey(
                name: "FK_InvestmentRequests_AspNetUsers_UserId",
                table: "InvestmentRequests",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_InvestorAccounts_AspNetUsers_UserId",
                table: "InvestorAccounts",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_IOTransactions_AspNetUsers_UserId",
                table: "IOTransactions",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ManagerRequests_AspNetUsers_UserId",
                table: "ManagerRequests",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ManagersAccounts_AspNetUsers_UserId",
                table: "ManagersAccounts",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
