using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MAVN.Service.CrossChainWalletLinker.MsSqlRepositories.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "cross_chain_wallet_linker");

            migrationBuilder.CreateTable(
                name: "configuration_items",
                schema: "cross_chain_wallet_linker",
                columns: table => new
                {
                    id = table.Column<string>(nullable: false),
                    type = table.Column<string>(nullable: false),
                    value = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_configuration_items", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "wallet_linking_requests",
                schema: "cross_chain_wallet_linker",
                columns: table => new
                {
                    customer_id = table.Column<string>(nullable: false),
                    public_address = table.Column<string>(nullable: true),
                    private_address = table.Column<string>(nullable: false),
                    link_code = table.Column<string>(nullable: false),
                    is_confirmed_in_private = table.Column<bool>(nullable: false),
                    is_confirmed_in_public = table.Column<bool>(nullable: false),
                    signature = table.Column<string>(nullable: true),
                    created_on = table.Column<DateTime>(nullable: false),
                    timestamp = table.Column<DateTime>(nullable: false),
                    fee = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_wallet_linking_requests", x => x.customer_id);
                });

            migrationBuilder.CreateTable(
                name: "wallet_linking_requests_counter",
                schema: "cross_chain_wallet_linker",
                columns: table => new
                {
                    customer_id = table.Column<string>(nullable: false),
                    approvals_counter = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_wallet_linking_requests_counter", x => x.customer_id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_configuration_items_type",
                schema: "cross_chain_wallet_linker",
                table: "configuration_items",
                column: "type",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_wallet_linking_requests_private_address",
                schema: "cross_chain_wallet_linker",
                table: "wallet_linking_requests",
                column: "private_address",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "configuration_items",
                schema: "cross_chain_wallet_linker");

            migrationBuilder.DropTable(
                name: "wallet_linking_requests",
                schema: "cross_chain_wallet_linker");

            migrationBuilder.DropTable(
                name: "wallet_linking_requests_counter",
                schema: "cross_chain_wallet_linker");
        }
    }
}
