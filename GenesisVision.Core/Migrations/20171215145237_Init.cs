using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace GenesisVision.Core.Migrations
{
    public partial class Init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    AccessFailedCount = table.Column<int>(nullable: false),
                    ConcurrencyStamp = table.Column<string>(nullable: true),
                    Email = table.Column<string>(nullable: true),
                    EmailConfirmed = table.Column<bool>(nullable: false),
                    LockoutEnabled = table.Column<bool>(nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(nullable: true),
                    NormalizedEmail = table.Column<string>(nullable: true),
                    NormalizedUserName = table.Column<string>(nullable: true),
                    PasswordHash = table.Column<string>(nullable: true),
                    PhoneNumber = table.Column<string>(nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(nullable: false),
                    SecurityStamp = table.Column<string>(nullable: true),
                    TwoFactorEnabled = table.Column<bool>(nullable: false),
                    UserName = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Brokers",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Brokers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BrokerTradeServers",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    BrokerId = table.Column<Guid>(nullable: false),
                    Host = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    Type = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BrokerTradeServers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BrokerTradeServers_Brokers_BrokerId",
                        column: x => x.BrokerId,
                        principalTable: "Brokers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ManagerRequests",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    BrokerTradeServersId = table.Column<Guid>(nullable: false),
                    Date = table.Column<DateTime>(nullable: false),
                    Login = table.Column<string>(nullable: true),
                    Password = table.Column<string>(nullable: true),
                    Type = table.Column<int>(nullable: false),
                    UserId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ManagerRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ManagerRequests_BrokerTradeServers_BrokerTradeServersId",
                        column: x => x.BrokerTradeServersId,
                        principalTable: "BrokerTradeServers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ManagerRequests_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ManagersAccounts",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Avatar = table.Column<string>(nullable: true),
                    BrokerTradeServerId = table.Column<Guid>(nullable: false),
                    Currency = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    IsEnabled = table.Column<bool>(nullable: false),
                    Login = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    Rating = table.Column<decimal>(nullable: false),
                    RegistrationDate = table.Column<DateTime>(nullable: false),
                    UserId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ManagersAccounts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ManagersAccounts_BrokerTradeServers_BrokerTradeServerId",
                        column: x => x.BrokerTradeServerId,
                        principalTable: "BrokerTradeServers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ManagersAccounts_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InvestmentPrograms",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    DateFrom = table.Column<DateTime>(nullable: false),
                    DateTo = table.Column<DateTime>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    FeeEntrance = table.Column<decimal>(nullable: false),
                    FeeManagement = table.Column<decimal>(nullable: false),
                    FeeSuccess = table.Column<decimal>(nullable: false),
                    InvestMaxAmount = table.Column<decimal>(nullable: true),
                    InvestMinAmount = table.Column<decimal>(nullable: false),
                    IsEnabled = table.Column<bool>(nullable: false),
                    ManagersAccountId = table.Column<Guid>(nullable: false),
                    Period = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvestmentPrograms", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InvestmentPrograms_ManagersAccounts_ManagersAccountId",
                        column: x => x.ManagersAccountId,
                        principalTable: "ManagersAccounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InvestmentRequests",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Amount = table.Column<decimal>(nullable: false),
                    Date = table.Column<DateTime>(nullable: false),
                    InvestmentProgramtId = table.Column<Guid>(nullable: false),
                    Status = table.Column<int>(nullable: false),
                    Type = table.Column<int>(nullable: false),
                    UserId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvestmentRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InvestmentRequests_InvestmentPrograms_InvestmentProgramtId",
                        column: x => x.InvestmentProgramtId,
                        principalTable: "InvestmentPrograms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InvestmentRequests_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BrokerTradeServers_BrokerId",
                table: "BrokerTradeServers",
                column: "BrokerId");

            migrationBuilder.CreateIndex(
                name: "IX_InvestmentPrograms_ManagersAccountId",
                table: "InvestmentPrograms",
                column: "ManagersAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_InvestmentRequests_InvestmentProgramtId",
                table: "InvestmentRequests",
                column: "InvestmentProgramtId");

            migrationBuilder.CreateIndex(
                name: "IX_InvestmentRequests_UserId",
                table: "InvestmentRequests",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ManagerRequests_BrokerTradeServersId",
                table: "ManagerRequests",
                column: "BrokerTradeServersId");

            migrationBuilder.CreateIndex(
                name: "IX_ManagerRequests_UserId",
                table: "ManagerRequests",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ManagersAccounts_BrokerTradeServerId",
                table: "ManagersAccounts",
                column: "BrokerTradeServerId");

            migrationBuilder.CreateIndex(
                name: "IX_ManagersAccounts_UserId",
                table: "ManagersAccounts",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InvestmentRequests");

            migrationBuilder.DropTable(
                name: "ManagerRequests");

            migrationBuilder.DropTable(
                name: "InvestmentPrograms");

            migrationBuilder.DropTable(
                name: "ManagersAccounts");

            migrationBuilder.DropTable(
                name: "BrokerTradeServers");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "Brokers");
        }
    }
}
